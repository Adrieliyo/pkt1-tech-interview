using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.DTOs.Orders;
using ShipmentTracker.Core.Entities;
using ShipmentTracker.Core.Enums;
using ShipmentTracker.Core.Interfaces;
using ShipmentTracker.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ShipmentTracker.Services
{
    /// <summary>
    /// Servicio de dominio para el ciclo de vida de las órdenes de envío: creación, listado,
    /// consulta, actualización, confirmación, cancelación y conversión a Shipment.
    /// </summary>
    public class OrderService : IOrderService
    {
        private const int MaxPageSize = 50;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateOrderDto> _createValidator;
        private readonly IValidator<UpdateOrderDto> _updateValidator;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateOrderDto> createValidator, IValidator<UpdateOrderDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
        {
            TrimOrderStrings(dto);

            var structuralResult = await _createValidator.ValidateAsync(dto);
            var failures = new List<ValidationFailure>(structuralResult.Errors);

            failures.AddRange(await ValidateBusinessRulesAsync(dto.CustomerId, dto.OriginBranchId, dto.PickupType));

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            var order = new Order
            {
                OrderNumber = await GenerateOrderNumberAsync(),
                CustomerId = dto.CustomerId,
                OriginBranchId = dto.OriginBranchId,
                Status = OrderStatus.Pending,
                ServiceType = dto.ServiceType!.Value,
                PickupType = dto.PickupType!.Value,
                PickupAddress = dto.PickupAddress,
                PickupScheduledAt = dto.PickupScheduledAt,
                RecipientName = dto.RecipientName,
                RecipientPhone = dto.RecipientPhone,
                RecipientAddress = dto.RecipientAddress,
                RecipientCity = dto.RecipientCity,
                RecipientState = dto.RecipientState,
                RecipientZipCode = dto.RecipientZipCode,
                DeclaredWeightKg = dto.DeclaredWeightKg,
                DeclaredWidthCm = dto.DeclaredWidthCm,
                DeclaredHeightCm = dto.DeclaredHeightCm,
                DeclaredLengthCm = dto.DeclaredLengthCm,
                QuotedPrice = dto.QuotedPrice,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<PagedResult<OrderDto>> GetOrdersAsync(int? customerId = null, OrderStatus? status = null, string? orderNumberContains = null, int page = 1, int pageSize = 5)
        {
            var hasCustomerId = customerId.HasValue;
            var hasStatus = status.HasValue;
            var hasOrderNumberContains = !string.IsNullOrWhiteSpace(orderNumberContains);

            Expression<Func<Order, bool>> filter = null;
            if (hasCustomerId && hasStatus && hasOrderNumberContains)
            {
                filter = x => x.CustomerId == customerId!.Value && x.Status == status!.Value && x.OrderNumber.Contains(orderNumberContains!);
            }
            else if (hasCustomerId && hasStatus)
            {
                filter = x => x.CustomerId == customerId!.Value && x.Status == status!.Value;
            }
            else if (hasCustomerId && hasOrderNumberContains)
            {
                filter = x => x.CustomerId == customerId!.Value && x.OrderNumber.Contains(orderNumberContains!);
            }
            else if (hasStatus && hasOrderNumberContains)
            {
                filter = x => x.Status == status!.Value && x.OrderNumber.Contains(orderNumberContains!);
            }
            else if (hasCustomerId)
            {
                filter = x => x.CustomerId == customerId!.Value;
            }
            else if (hasStatus)
            {
                filter = x => x.Status == status!.Value;
            }
            else if (hasOrderNumberContains)
            {
                filter = x => x.OrderNumber.Contains(orderNumberContains!);
            }

            var effectivePageSize = Math.Min(pageSize, MaxPageSize);
            long skip = (long)(page - 1) * effectivePageSize;

            IEnumerable<Order> orders;
            if (skip > int.MaxValue)
            {
                orders = Enumerable.Empty<Order>();
            }
            else
            {
                orders = await _unitOfWork.OrderRepository.GetAsync(
                    filter,
                    orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                    skip: (int)skip,
                    take: effectivePageSize);
            }

            var totalCount = await _unitOfWork.OrderRepository.CountAsync(filter);

            return new PagedResult<OrderDto>
            {
                Items = orders.Select(o => _mapper.Map<OrderDto>(o)),
                Page = page,
                PageSize = effectivePageSize,
                TotalCount = totalCount
            };
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);

            if (order == null)
                return null;

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto?> GetOrderByNumberAsync(string orderNumber)
        {
            var order = await _unitOfWork.OrderRepository.SingleOrDefaultAsync(x => x.OrderNumber == orderNumber);

            if (order == null)
                return null;

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);

            if (order == null)
                return null;

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Only pending orders can be edited.");
            }

            TrimOrderStrings(dto);

            var structuralResult = await _updateValidator.ValidateAsync(dto);
            var failures = new List<ValidationFailure>(structuralResult.Errors);

            failures.AddRange(await ValidateBusinessRulesAsync(order.CustomerId, dto.OriginBranchId, dto.PickupType));

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            order.OriginBranchId = dto.OriginBranchId;
            order.ServiceType = dto.ServiceType!.Value;
            order.PickupType = dto.PickupType!.Value;
            order.PickupAddress = dto.PickupAddress;
            order.PickupScheduledAt = dto.PickupScheduledAt;
            order.RecipientName = dto.RecipientName;
            order.RecipientPhone = dto.RecipientPhone;
            order.RecipientAddress = dto.RecipientAddress;
            order.RecipientCity = dto.RecipientCity;
            order.RecipientState = dto.RecipientState;
            order.RecipientZipCode = dto.RecipientZipCode;
            order.DeclaredWeightKg = dto.DeclaredWeightKg;
            order.DeclaredWidthCm = dto.DeclaredWidthCm;
            order.DeclaredHeightCm = dto.DeclaredHeightCm;
            order.DeclaredLengthCm = dto.DeclaredLengthCm;
            order.QuotedPrice = dto.QuotedPrice;
            order.Notes = dto.Notes;
            order.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto?> ConfirmOrderAsync(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);

            if (order == null)
                return null;

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Only pending orders can be confirmed.");
            }

            order.Status = OrderStatus.Confirmed;
            order.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);

            if (order == null)
                return false;

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Only pending orders can be cancelled.");
            }

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<ConvertOrderResultDto?> ConvertToShipmentAsync(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);

            if (order == null)
                return null;

            if (order.Status != OrderStatus.Confirmed)
            {
                throw new InvalidOperationException("Only confirmed orders can be converted to a shipment.");
            }

            var shipment = new Shipment
            {
                TrackingNumber = await GenerateTrackingNumberAsync(),
                Recipient = order.RecipientName,
                Status = ShipmentStatus.Collected,
                CreatedAt = DateTime.UtcNow,
                OrderId = order.Id
            };

            var shipmentEvent = new ShipmentEvent
            {
                Shipment = shipment,
                EventType = ShipmentEventType.OrderConverted,
                StatusSnapshot = ShipmentStatus.Collected,
                OccurredAt = DateTime.UtcNow
            };

            order.Status = OrderStatus.Converted;
            order.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ShipmentRepository.AddAsync(shipment);
            await _unitOfWork.ShipmentEventRepository.AddAsync(shipmentEvent);
            await _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CommitAsync();

            return new ConvertOrderResultDto
            {
                ShipmentId = shipment.Id,
                TrackingNumber = shipment.TrackingNumber
            };
        }

        /// <summary>
        /// Valida que el Customer referenciado exista y esté activo y que, para órdenes DropOff,
        /// la Branch de origen exista y esté activa. Errores acumulados para fusionarse con los estructurales.
        /// </summary>
        private async Task<List<ValidationFailure>> ValidateBusinessRulesAsync(int customerId, int? originBranchId, PickupType? pickupType)
        {
            var failures = new List<ValidationFailure>();

            var customer = await _unitOfWork.CustomerRepository.SingleOrDefaultAsync(x => x.Id == customerId);
            if (customer == null || !customer.IsActive)
            {
                failures.Add(new ValidationFailure(nameof(Order.CustomerId), "The specified customer does not exist or is not active."));
            }

            if (pickupType == PickupType.DropOff && originBranchId.HasValue)
            {
                var branch = await _unitOfWork.BranchRepository.SingleOrDefaultAsync(x => x.Id == originBranchId.Value);
                if (branch == null || !branch.IsActive)
                {
                    failures.Add(new ValidationFailure(nameof(Order.OriginBranchId), "The specified origin branch does not exist or is not active."));
                }
            }

            return failures;
        }

        /// <summary>
        /// Genera el número de orden (ORD-YYYYMMDD-XXXX) contando las órdenes creadas hoy (UTC).
        /// </summary>
        private async Task<string> GenerateOrderNumberAsync()
        {
            var todayUtc = DateTime.UtcNow.Date;
            var tomorrowUtc = todayUtc.AddDays(1);
            var count = await _unitOfWork.OrderRepository.CountAsync(x => x.CreatedAt >= todayUtc && x.CreatedAt < tomorrowUtc);
            return $"ORD-{todayUtc:yyyyMMdd}-{count + 1:D4}";
        }

        /// <summary>
        /// Genera el número de guía (TRK-YYYYMMDD-XXXX) contando los Shipments creados hoy (UTC),
        /// con una secuencia independiente de la de las órdenes.
        /// </summary>
        private async Task<string> GenerateTrackingNumberAsync()
        {
            var todayUtc = DateTime.UtcNow.Date;
            var tomorrowUtc = todayUtc.AddDays(1);
            var count = await _unitOfWork.ShipmentRepository.CountAsync(x => x.CreatedAt >= todayUtc && x.CreatedAt < tomorrowUtc);
            return $"TRK-{todayUtc:yyyyMMdd}-{count + 1:D4}";
        }

        private void TrimOrderStrings(CreateOrderDto dto)
        {
            dto.RecipientName = dto.RecipientName?.Trim() ?? string.Empty;
            dto.RecipientPhone = dto.RecipientPhone?.Trim() ?? string.Empty;
            dto.RecipientAddress = dto.RecipientAddress?.Trim() ?? string.Empty;
            dto.RecipientCity = dto.RecipientCity?.Trim() ?? string.Empty;
            dto.RecipientState = dto.RecipientState?.Trim() ?? string.Empty;
            dto.RecipientZipCode = dto.RecipientZipCode?.Trim() ?? string.Empty;
            // PickupAddress/Notes are genuinely nullable (must stay null for DropOff / when omitted) —
            // trim only when present, never force null into "".
            dto.PickupAddress = dto.PickupAddress?.Trim();
            dto.Notes = dto.Notes?.Trim();
        }

        private void TrimOrderStrings(UpdateOrderDto dto)
        {
            dto.RecipientName = dto.RecipientName?.Trim() ?? string.Empty;
            dto.RecipientPhone = dto.RecipientPhone?.Trim() ?? string.Empty;
            dto.RecipientAddress = dto.RecipientAddress?.Trim() ?? string.Empty;
            dto.RecipientCity = dto.RecipientCity?.Trim() ?? string.Empty;
            dto.RecipientState = dto.RecipientState?.Trim() ?? string.Empty;
            dto.RecipientZipCode = dto.RecipientZipCode?.Trim() ?? string.Empty;
            dto.PickupAddress = dto.PickupAddress?.Trim();
            dto.Notes = dto.Notes?.Trim();
        }
    }
}