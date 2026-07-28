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
        public ShipmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

            return shipments.Select(s => new ShipmentDto
            {
                Id = s.Id,
                TrackingNumber = s.TrackingNumber,
                Recipient = s.Recipient,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                DeliveredAt = s.DeliveredAt
            });
        }

        public async Task<ShipmentDto?> GetShipmentByTrackingNumberAsync(string trackingNumber)
        {
            var shipment = await _unitOfWork.ShipmentRepository
                .SingleOrDefaultAsync(x => x.TrackingNumber == trackingNumber);

            if (shipment == null)
                return null;

            return new ShipmentDto
            {
                Id = shipment.Id,
                TrackingNumber = shipment.TrackingNumber,
                Recipient = shipment.Recipient,
                Status = shipment.Status,
                CreatedAt = shipment.CreatedAt,
                DeliveredAt = shipment.DeliveredAt
            };
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

            return new ShipmentDto
            {
                Id = newShipment.Id,
                TrackingNumber = newShipment.TrackingNumber,
                Recipient = newShipment.Recipient,
                Status = newShipment.Status,
                CreatedAt = newShipment.CreatedAt,
                DeliveredAt = newShipment.DeliveredAt
            };
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

            var validator = new ShipmentTransitionValidator();
            var validationResult = await validator.ValidateAsync(transitionContext);

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
