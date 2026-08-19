using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.DTOs.Vehicles;
using ShipmentTracker.Core.Entities;
using ShipmentTracker.Core.Interfaces;
using ShipmentTracker.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ShipmentTracker.Services
{
    public class VehicleService : IVehicleService
    {
        private const int MaxPageSize = 50;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateVehicleDto> _createValidator;
        private readonly IValidator<UpdateVehicleDto> _updateValidator;

        public VehicleService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateVehicleDto> createValidator, IValidator<UpdateVehicleDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        // Reglas dependientes de base de datos (sucursal activa + unicidad global de Plate,
        // incluso contra registros inactivos) — compartidas entre Create y Update. currentId = 0
        // en Create (nunca coincide con un id real), id real en Update.
        private async Task<List<ValidationFailure>> ValidateBusinessRulesAsync(int branchId, string plate, int currentId)
        {
            var failures = new List<ValidationFailure>();

            var branch = await _unitOfWork.BranchRepository.SingleOrDefaultAsync(x => x.Id == branchId);
            if (branch == null || !branch.IsActive)
            {
                failures.Add(new ValidationFailure(nameof(Vehicle.BranchId), "La sucursal indicada no existe o no está activa."));
            }

            var plateInUse = await _unitOfWork.VehicleRepository.SingleOrDefaultAsync(x => x.Plate == plate && x.Id != currentId);
            if (plateInUse != null)
            {
                failures.Add(new ValidationFailure(nameof(Vehicle.Plate), "La placa ya está en uso por otro vehículo."));
            }

            return failures;
        }

        public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto)
        {
            dto.Plate = dto.Plate?.Trim() ?? string.Empty;
            dto.Brand = dto.Brand?.Trim() ?? string.Empty;
            dto.Model = dto.Model?.Trim() ?? string.Empty;

            var structuralResult = await _createValidator.ValidateAsync(dto);
            var failures = new List<ValidationFailure>(structuralResult.Errors);

            failures.AddRange(await ValidateBusinessRulesAsync(dto.BranchId, dto.Plate, currentId: 0));

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            var vehicle = new Vehicle
            {
                BranchId = dto.BranchId,
                Plate = dto.Plate,
                Type = dto.Type!.Value,
                Brand = dto.Brand,
                Model = dto.Model,
                Year = dto.Year,
                MaxWeightKg = dto.MaxWeightKg,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.VehicleRepository.AddAsync(vehicle);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<VehicleDto>(vehicle);
        }

        public async Task<PagedResult<VehicleDto>> GetVehiclesAsync(bool onlyActive = true, int? branchId = null, int page = 1, int pageSize = 5)
        {
            Expression<Func<Vehicle, bool>> filter = branchId.HasValue
                ? x => x.IsActive == onlyActive && x.BranchId == branchId.Value
                : x => x.IsActive == onlyActive;

            var effectivePageSize = Math.Min(pageSize, MaxPageSize);
            long skip = (long)(page - 1) * effectivePageSize;

            IEnumerable<Vehicle> vehicles;
            if (skip > int.MaxValue)
            {
                vehicles = Enumerable.Empty<Vehicle>();
            }
            else
            {
                vehicles = await _unitOfWork.VehicleRepository.GetAsync(
                    filter,
                    orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                    skip: (int)skip,
                    take: effectivePageSize);
            }

            var totalCount = await _unitOfWork.VehicleRepository.CountAsync(filter);

            return new PagedResult<VehicleDto>
            {
                Items = vehicles.Select(v => _mapper.Map<VehicleDto>(v)),
                Page = page,
                PageSize = effectivePageSize,
                TotalCount = totalCount
            };
        }

        public async Task<VehicleDto?> GetVehicleByIdAsync(int id)
        {
            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(id);

            if (vehicle == null)
                return null;

            return _mapper.Map<VehicleDto>(vehicle);
        }

        public async Task<VehicleDto?> UpdateVehicleAsync(int id, UpdateVehicleDto dto)
        {
            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(id);

            if (vehicle == null)
                return null;

            dto.Plate = dto.Plate?.Trim() ?? string.Empty;
            dto.Brand = dto.Brand?.Trim() ?? string.Empty;
            dto.Model = dto.Model?.Trim() ?? string.Empty;

            var structuralResult = await _updateValidator.ValidateAsync(dto);
            var failures = new List<ValidationFailure>(structuralResult.Errors);

            failures.AddRange(await ValidateBusinessRulesAsync(dto.BranchId, dto.Plate, currentId: id));

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            vehicle.BranchId = dto.BranchId;
            vehicle.Plate = dto.Plate;
            vehicle.Type = dto.Type!.Value;
            vehicle.Brand = dto.Brand;
            vehicle.Model = dto.Model;
            vehicle.Year = dto.Year;
            vehicle.MaxWeightKg = dto.MaxWeightKg;
            vehicle.IsActive = dto.IsActive;

            await _unitOfWork.VehicleRepository.Update(vehicle);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<VehicleDto>(vehicle);
        }

        public async Task<bool> DeactivateVehicleAsync(int id)
        {
            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(id);

            if (vehicle == null)
                return false;

            if (vehicle.IsActive)
            {
                vehicle.IsActive = false;
                await _unitOfWork.VehicleRepository.Update(vehicle);
                await _unitOfWork.CommitAsync();
            }

            return true;
        }
    }
}
