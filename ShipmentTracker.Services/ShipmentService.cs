using AutoMapper;
using FluentValidation;
using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.Entities;
using ShipmentTracker.Core.Enums;
using ShipmentTracker.Core.Interfaces;
using ShipmentTracker.Core.Interfaces.Services;
using ShipmentTracker.Services.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Services
{
    public class ShipmentService: IShipmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<StatusTransitionContext> _transitionValidator;

        public ShipmentService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<StatusTransitionContext> transitionValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _transitionValidator = transitionValidator;
        }

        public async Task<IEnumerable<ShipmentDto>> GetShipmentsAsync(ShipmentStatus? status = null)
        {
            IEnumerable<Shipment> shipments;

            if (status.HasValue)
            {
                shipments = await _unitOfWork.ShipmentRepository.GetAsync(x => x.Status == status.Value);
            }
            else
            {
                shipments = await _unitOfWork.ShipmentRepository.GetAllAsync();
            }

            return shipments.Select(s => _mapper.Map<ShipmentDto>(s));
        }

        public async Task<ShipmentDto?> GetShipmentByTrackingNumberAsync(string trackingNumber)
        {
            var shipment = await _unitOfWork.ShipmentRepository
                .SingleOrDefaultAsync(x => x.TrackingNumber == trackingNumber);

            if (shipment == null)
                return null;

            return _mapper.Map<ShipmentDto>(shipment);
        }

        public async Task<ShipmentDto> CreateShipmentAsync(CreateShipmentDto createShipmentDto)
        {
            string newTrackingNumber = $"TRK-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";

            var newShipment = new Shipment
            {
                TrackingNumber = newTrackingNumber,
                Recipient = createShipmentDto.Recipient,
                Status = ShipmentStatus.Collected,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ShipmentRepository.AddAsync(newShipment);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ShipmentDto>(newShipment);
        }

        public async Task<bool> UpdateShipmentStatusAsync(string trackingNumber, ShipmentStatus newStatus)
        {
            var shipmentToUpdate = await _unitOfWork.ShipmentRepository
                .SingleOrDefaultAsync(x => x.TrackingNumber == trackingNumber);

            if (shipmentToUpdate == null)
                return false;

            var transitionContext = new StatusTransitionContext
            {
                CurrentStatus = shipmentToUpdate.Status,
                NewStatus = newStatus
            };

            var validationResult = await _transitionValidator.ValidateAsync(transitionContext);

            if (!validationResult.IsValid)
            {
                throw new InvalidOperationException(validationResult.Errors.First().ErrorMessage);
            }

            shipmentToUpdate.Status = newStatus;

            
            if (newStatus.ToString() == "Delivered" && shipmentToUpdate.DeliveredAt == null)
            {
                shipmentToUpdate.DeliveredAt = DateTime.UtcNow;
            }

            await _unitOfWork.ShipmentRepository.Update(shipmentToUpdate);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
