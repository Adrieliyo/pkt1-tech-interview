using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using ShipmentTracker.Core.Constants;
using ShipmentTracker.Core.DTOs.ShipmentEvents;
using ShipmentTracker.Core.Entities;
using ShipmentTracker.Core.Enums;
using ShipmentTracker.Core.Interfaces;
using ShipmentTracker.Core.Interfaces.Services;
using ShipmentTracker.Services.Validators.Shipments;

namespace ShipmentTracker.Services
{
    /// <summary>
    /// Servicio de dominio para el registro y consulta de eventos de Shipment, incluidos los
    /// intentos de entrega y la vista pública de tracking.
    /// </summary>
    public class ShipmentEventService : IShipmentEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<RegisterEventDto> _registerEventValidator;
        private readonly IValidator<RegisterDeliveryAttemptDto> _registerDeliveryAttemptValidator;
        private readonly IValidator<StatusTransitionContext> _transitionValidator;

        public ShipmentEventService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<RegisterEventDto> registerEventValidator,
            IValidator<RegisterDeliveryAttemptDto> registerDeliveryAttemptValidator,
            IValidator<StatusTransitionContext> transitionValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _registerEventValidator = registerEventValidator;
            _registerDeliveryAttemptValidator = registerDeliveryAttemptValidator;
            _transitionValidator = transitionValidator;
        }

        // Regla de negocio compartida entre RegisterEventAsync y RegisterDeliveryAttemptAsync
        // (research.md Decisión 10): el Employee, si se proporciona, debe existir y estar activo;
        // cuando requireDriver es true, además es obligatorio y debe tener Role = Driver.
        private async Task<List<ValidationFailure>> ValidateEmployeeAsync(int? employeeId, bool requireDriver)
        {
            var failures = new List<ValidationFailure>();

            if (requireDriver && !employeeId.HasValue)
            {
                failures.Add(new ValidationFailure(nameof(RegisterEventDto.EmployeeId), "EmployeeId is required for OutForDelivery events."));
                return failures;
            }

            if (!employeeId.HasValue)
            {
                return failures;
            }

            var employee = await _unitOfWork.EmployeeRepository.SingleOrDefaultAsync(x => x.Id == employeeId.Value);
            if (employee == null || !employee.IsActive)
            {
                failures.Add(new ValidationFailure(nameof(RegisterEventDto.EmployeeId), "The specified employee does not exist or is not active."));
            }
            else if (requireDriver && employee.Role != EmployeeRole.Driver)
            {
                failures.Add(new ValidationFailure(nameof(RegisterEventDto.EmployeeId), "The specified employee must have the Driver role for OutForDelivery events."));
            }

            return failures;
        }

        // Tipos que un caller WarehouseStaff puede registrar vía el endpoint genérico (spec 008 FR-015).
        private static readonly HashSet<ShipmentEventType> WarehouseStaffAllowedTypes = new()
        {
            ShipmentEventType.ReceivedAtBranch,
            ShipmentEventType.DepartedFromBranch,
            ShipmentEventType.InTransit
        };

        public async Task<ShipmentEventDto?> RegisterEventAsync(int shipmentId, RegisterEventDto dto, string? callerRole = null, int? callerEmployeeId = null)
        {
            var shipment = await _unitOfWork.ShipmentRepository.GetByIdAsync(shipmentId);

            if (shipment == null)
                return null;

            // Gate de rol (module 008, research.md Decisiones 8/9): un WarehouseStaff solo puede
            // registrar los tres tipos de evento de almacén, no cualquier tipo permitido por el
            // endpoint genérico. No es una regla de transición, es un gate directo — InvalidOperationException.
            if (callerRole == Roles.WarehouseStaff && dto.EventType.HasValue && !WarehouseStaffAllowedTypes.Contains(dto.EventType.Value))
            {
                throw new InvalidOperationException("WarehouseStaff can only register ReceivedAtBranch, DepartedFromBranch, or InTransit events.");
            }

            var structuralResult = await _registerEventValidator.ValidateAsync(dto);
            var failures = new List<ValidationFailure>(structuralResult.Errors);

            var requireDriver = dto.EventType == ShipmentEventType.OutForDelivery;
            failures.AddRange(await ValidateEmployeeAsync(dto.EmployeeId, requireDriver));

            var newStatus = dto.EventType == ShipmentEventType.OutForDelivery
                ? ShipmentStatus.OutForDelivery
                : shipment.Status;

            var transitionResult = await _transitionValidator.ValidateAsync(new StatusTransitionContext
            {
                CurrentStatus = shipment.Status,
                NewStatus = newStatus
            });
            failures.AddRange(transitionResult.Errors);

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            shipment.Status = newStatus;

            var shipmentEvent = new ShipmentEvent
            {
                ShipmentId = shipmentId,
                EventType = dto.EventType!.Value,
                StatusSnapshot = newStatus,
                EmployeeId = dto.EmployeeId,
                LocationLabel = dto.LocationLabel,
                Notes = dto.Notes,
                OccurredAt = dto.OccurredAt,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ShipmentRepository.Update(shipment);
            await _unitOfWork.ShipmentEventRepository.AddAsync(shipmentEvent);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ShipmentEventDto>(shipmentEvent);
        }

        public async Task<ShipmentEventDto?> RegisterDeliveryAttemptAsync(int shipmentId, RegisterDeliveryAttemptDto dto)
        {
            var shipment = await _unitOfWork.ShipmentRepository.GetByIdAsync(shipmentId);

            if (shipment == null)
                return null;

            var structuralResult = await _registerDeliveryAttemptValidator.ValidateAsync(dto);
            var failures = new List<ValidationFailure>(structuralResult.Errors);

            failures.AddRange(await ValidateEmployeeAsync(dto.EmployeeId, requireDriver: false));

            // Comprobación de igualdad simple, no se enruta por el validador de transición: un
            // intento de entrega nunca cambia el estado del Shipment (research.md Decisión 4).
            if (shipment.Status != ShipmentStatus.OutForDelivery)
            {
                failures.Add(new ValidationFailure(nameof(Shipment.Status), "Only shipments that are currently out for delivery can have a delivery attempt logged."));
            }

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            var shipmentEvent = new ShipmentEvent
            {
                ShipmentId = shipmentId,
                EventType = ShipmentEventType.DeliveryAttempted,
                StatusSnapshot = shipment.Status,
                EmployeeId = dto.EmployeeId,
                LocationLabel = dto.LocationLabel,
                Notes = dto.Notes,
                OccurredAt = dto.OccurredAt,
                CreatedAt = DateTime.UtcNow
            };

            var attemptNumber = await _unitOfWork.DeliveryAttemptRepository.CountAsync(x => x.ShipmentEvent.ShipmentId == shipmentId) + 1;

            var deliveryAttempt = new DeliveryAttempt
            {
                ShipmentEvent = shipmentEvent,
                AttemptNumber = attemptNumber,
                FailureReason = dto.FailureReason!.Value,
                NextAttemptAt = dto.NextAttemptAt
            };

            await _unitOfWork.ShipmentEventRepository.AddAsync(shipmentEvent);
            await _unitOfWork.DeliveryAttemptRepository.AddAsync(deliveryAttempt);
            await _unitOfWork.CommitAsync();

            var eventDto = _mapper.Map<ShipmentEventDto>(shipmentEvent);
            eventDto.DeliveryAttempt = new DeliveryAttemptDetailDto
            {
                AttemptNumber = deliveryAttempt.AttemptNumber,
                FailureReason = deliveryAttempt.FailureReason,
                NextAttemptAt = deliveryAttempt.NextAttemptAt
            };

            return eventDto;
        }

        public async Task<IEnumerable<ShipmentEventDto>?> GetEventsByShipmentAsync(int shipmentId, string? callerRole = null, int? callerEmployeeId = null)
        {
            var shipment = await _unitOfWork.ShipmentRepository.GetByIdAsync(shipmentId);

            if (shipment == null)
                return null;

            // Gate de asignación (module 008, research.md Decisión 4): un Driver solo puede leer
            // Shipments en los que haya registrado al menos un OutForDelivery/DeliveryAttempted.
            // Igualdad simple, no es una transición de estado — InvalidOperationException.
            if (callerRole == Roles.Driver)
            {
                var isAssigned = await _unitOfWork.ShipmentEventRepository.CountAsync(
                    x => x.ShipmentId == shipmentId
                      && x.EmployeeId == callerEmployeeId
                      && (x.EventType == ShipmentEventType.OutForDelivery || x.EventType == ShipmentEventType.DeliveryAttempted)) > 0;

                if (!isAssigned)
                {
                    throw new InvalidOperationException("You are not assigned to this shipment.");
                }
            }

            var events = await _unitOfWork.ShipmentEventRepository.GetAsync(
                x => x.ShipmentId == shipmentId,
                orderBy: q => q.OrderBy(x => x.OccurredAt));

            var result = new List<ShipmentEventDto>();
            foreach (var shipmentEvent in events)
            {
                var dto = _mapper.Map<ShipmentEventDto>(shipmentEvent);
                if (shipmentEvent.EventType == ShipmentEventType.DeliveryAttempted)
                {
                    dto.DeliveryAttempt = await MapDeliveryAttemptDetailAsync(shipmentEvent.Id);
                }
                result.Add(dto);
            }

            return result;
        }

        public async Task<ShipmentTrackingDto?> GetTrackingAsync(string trackingNumber)
        {
            var shipment = await _unitOfWork.ShipmentRepository.SingleOrDefaultAsync(x => x.TrackingNumber == trackingNumber);

            if (shipment == null)
                return null;

            var events = await _unitOfWork.ShipmentEventRepository.GetAsync(
                x => x.ShipmentId == shipment.Id,
                orderBy: q => q.OrderBy(x => x.OccurredAt));

            var trackingDto = _mapper.Map<ShipmentTrackingDto>(shipment);

            foreach (var shipmentEvent in events)
            {
                var eventDto = _mapper.Map<TrackingEventDto>(shipmentEvent);
                if (shipmentEvent.EventType == ShipmentEventType.DeliveryAttempted)
                {
                    eventDto.DeliveryAttempt = await MapDeliveryAttemptDetailAsync(shipmentEvent.Id);
                }
                trackingDto.Events.Add(eventDto);
            }

            return trackingDto;
        }

        private async Task<DeliveryAttemptDetailDto?> MapDeliveryAttemptDetailAsync(int shipmentEventId)
        {
            var attempt = await _unitOfWork.DeliveryAttemptRepository.SingleOrDefaultAsync(x => x.ShipmentEventId == shipmentEventId);

            if (attempt == null)
                return null;

            return new DeliveryAttemptDetailDto
            {
                AttemptNumber = attempt.AttemptNumber,
                FailureReason = attempt.FailureReason,
                NextAttemptAt = attempt.NextAttemptAt
            };
        }
    }
}
