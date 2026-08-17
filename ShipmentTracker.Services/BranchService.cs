using AutoMapper;
using FluentValidation;
using ShipmentTracker.Core.DTOs;
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
    public class BranchService : IBranchService
    {
        private const int MaxPageSize = 50;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateBranchDto> _createValidator;
        private readonly IValidator<UpdateBranchDto> _updateValidator;

        public BranchService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateBranchDto> createValidator, IValidator<UpdateBranchDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<BranchDto> CreateBranchAsync(CreateBranchDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var branch = new Branch
            {
                Name = dto.Name,
                Type = dto.Type!.Value,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Phone = dto.Phone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Schedule = dto.Schedule.Select(s => new BranchSchedule
                {
                    DayOfWeek = s.DayOfWeek!.Value,
                    OpensAt = s.OpensAt,
                    ClosesAt = s.ClosesAt,
                    IsClosed = s.IsClosed
                }).ToList()
            };

            await _unitOfWork.BranchRepository.AddAsync(branch);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<BranchDto>(branch);
        }

        public async Task<PagedResult<BranchDto>> GetBranchesAsync(bool onlyActive = true, BranchType? type = null, int page = 1, int pageSize = 5)
        {
            Expression<Func<Branch, bool>> filter = type.HasValue
                ? x => x.IsActive == onlyActive && x.Type == type.Value
                : x => x.IsActive == onlyActive;

            var effectivePageSize = Math.Min(pageSize, MaxPageSize);
            long skip = (long)(page - 1) * effectivePageSize;

            IEnumerable<Branch> branches;
            if (skip > int.MaxValue)
            {
                branches = Enumerable.Empty<Branch>();
            }
            else
            {
                branches = await _unitOfWork.BranchRepository.GetAsync(
                    filter,
                    orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                    skip: (int)skip,
                    take: effectivePageSize);
            }

            var totalCount = await _unitOfWork.BranchRepository.CountAsync(filter);

            return new PagedResult<BranchDto>
            {
                Items = branches.Select(b => _mapper.Map<BranchDto>(b)),
                Page = page,
                PageSize = effectivePageSize,
                TotalCount = totalCount
            };
        }

        public async Task<BranchDto?> GetBranchByIdAsync(int id)
        {
            var branch = await _unitOfWork.BranchRepository.GetByIdWithScheduleAsync(id);

            if (branch == null)
                return null;

            return _mapper.Map<BranchDto>(branch);
        }

        public async Task<BranchDto?> UpdateBranchAsync(int id, UpdateBranchDto dto)
        {
            var branch = await _unitOfWork.BranchRepository.GetByIdWithScheduleAsync(id);

            if (branch == null)
                return null;

            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            branch.Name = dto.Name;
            branch.Type = dto.Type!.Value;
            branch.Address = dto.Address;
            branch.City = dto.City;
            branch.State = dto.State;
            branch.ZipCode = dto.ZipCode;
            branch.Latitude = dto.Latitude;
            branch.Longitude = dto.Longitude;
            branch.Phone = dto.Phone;
            branch.IsActive = dto.IsActive;

            branch.Schedule.Clear();
            foreach (var entry in dto.Schedule)
            {
                branch.Schedule.Add(new BranchSchedule
                {
                    DayOfWeek = entry.DayOfWeek!.Value,
                    OpensAt = entry.OpensAt,
                    ClosesAt = entry.ClosesAt,
                    IsClosed = entry.IsClosed
                });
            }

            await _unitOfWork.BranchRepository.Update(branch);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<BranchDto>(branch);
        }

        public async Task<bool> DeactivateBranchAsync(int id)
        {
            var branch = await _unitOfWork.BranchRepository.SingleOrDefaultAsync(x => x.Id == id);

            if (branch == null)
                return false;

            if (branch.IsActive)
            {
                branch.IsActive = false;
                await _unitOfWork.BranchRepository.Update(branch);
                await _unitOfWork.CommitAsync();
            }

            return true;
        }
    }
}
