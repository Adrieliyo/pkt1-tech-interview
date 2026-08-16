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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Services
{
    public class ShipmentService: IShipmentService
    {
        private const int MaxPageSize = 50;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<StatusTransitionContext> _transitionValidator;

        public ShipmentService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<StatusTransitionContext> transitionValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _transitionValidator = transitionValidator;
        }

        public async Task<PagedResult<ShipmentDto>> GetShipmentsAsync(ShipmentStatus? status = null, int page = 1, int pageSize = 5)
        {
            Expression<Func<Shipment, bool>> filter = null;
            if (status.HasValue)
            {
                filter = x => x.Status == status.Value;
            }

            var effectivePageSize = Math.Min(pageSize, MaxPageSize);
            long skip = (long)(page - 1) * effectivePageSize;

            IEnumerable<Shipment> shipments;
            if (skip > int.MaxValue)
            {
                shipments = Enumerable.Empty<Shipment>();
            }
            else
            {
                shipments = await _unitOfWork.ShipmentRepository.GetAsync(
                    filter,
                    orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                    skip: (int)skip,
                    take: effectivePageSize);
            }

            var totalCount = await _unitOfWork.ShipmentRepository.CountAsync(filter);

            return new PagedResult<ShipmentDto>
            {
                Items = shipments.Select(s => _mapper.Map<ShipmentDto>(s)),
                Page = page,
                PageSize = effectivePageSize,
                TotalCount = totalCount
            };
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
