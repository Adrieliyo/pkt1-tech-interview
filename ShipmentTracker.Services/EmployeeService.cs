using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
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
    public class EmployeeService : IEmployeeService
    {
        private const int MaxPageSize = 50;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateEmployeeDto> _createValidator;
        private readonly IValidator<UpdateEmployeeDto> _updateValidator;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateEmployeeDto> createValidator, IValidator<UpdateEmployeeDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        // Reglas dependientes de base de datos (sucursal activa + unicidad global de Email/EmployeeNumber,
        // incluso contra registros inactivos) — compartidas entre Create y Update. currentId = 0 en Create
        // (nunca coincide con un id real, ya que la identity empieza en 1), id real en Update.
        private async Task<List<ValidationFailure>> ValidateBusinessRulesAsync(int branchId, string email, string employeeNumber, int currentId)
        {
            var failures = new List<ValidationFailure>();

            var branch = await _unitOfWork.BranchRepository.SingleOrDefaultAsync(x => x.Id == branchId);
            if (branch == null || !branch.IsActive)
            {
                failures.Add(new ValidationFailure(nameof(Employee.BranchId), "La sucursal indicada no existe o no está activa."));
            }

            var emailInUse = await _unitOfWork.EmployeeRepository.SingleOrDefaultAsync(x => x.Email == email && x.Id != currentId);
            if (emailInUse != null)
            {
                failures.Add(new ValidationFailure(nameof(Employee.Email), "El correo electrónico ya está en uso por otro empleado."));
            }

            var numberInUse = await _unitOfWork.EmployeeRepository.SingleOrDefaultAsync(x => x.EmployeeNumber == employeeNumber && x.Id != currentId);
            if (numberInUse != null)
            {
                failures.Add(new ValidationFailure(nameof(Employee.EmployeeNumber), "El número de empleado ya está en uso por otro empleado."));
            }

            return failures;
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            dto.FirstName = dto.FirstName?.Trim() ?? string.Empty;
            dto.LastName = dto.LastName?.Trim() ?? string.Empty;
            dto.Email = dto.Email?.Trim() ?? string.Empty;
            dto.EmployeeNumber = dto.EmployeeNumber?.Trim() ?? string.Empty;

            var structuralResult = await _createValidator.ValidateAsync(dto);
            var failures = new List<ValidationFailure>(structuralResult.Errors);

            failures.AddRange(await ValidateBusinessRulesAsync(dto.BranchId, dto.Email, dto.EmployeeNumber, currentId: 0));

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            var employee = new Employee
            {
                BranchId = dto.BranchId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = dto.Role!.Value,
                EmployeeNumber = dto.EmployeeNumber,
                HireDate = dto.HireDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            await _unitOfWork.EmployeeRepository.AddAsync(employee);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<PagedResult<EmployeeDto>> GetEmployeesAsync(int? branchId = null, EmployeeRole? role = null, int page = 1, int pageSize = 5)
        {
            Expression<Func<Employee, bool>> filter;
            if (branchId.HasValue && role.HasValue)
            {
                filter = x => x.IsActive && x.BranchId == branchId.Value && x.Role == role.Value;
            }
            else if (branchId.HasValue)
            {
                filter = x => x.IsActive && x.BranchId == branchId.Value;
            }
            else if (role.HasValue)
            {
                filter = x => x.IsActive && x.Role == role.Value;
            }
            else
            {
                filter = x => x.IsActive;
            }

            var effectivePageSize = Math.Min(pageSize, MaxPageSize);
            long skip = (long)(page - 1) * effectivePageSize;

            IEnumerable<Employee> employees;
            if (skip > int.MaxValue)
            {
                employees = Enumerable.Empty<Employee>();
            }
            else
            {
                employees = await _unitOfWork.EmployeeRepository.GetAsync(
                    filter,
                    orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                    skip: (int)skip,
                    take: effectivePageSize);
            }

            var totalCount = await _unitOfWork.EmployeeRepository.CountAsync(filter);

            return new PagedResult<EmployeeDto>
            {
                Items = employees.Select(e => _mapper.Map<EmployeeDto>(e)),
                Page = page,
                PageSize = effectivePageSize,
                TotalCount = totalCount
            };
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(id);

            if (employee == null)
                return null;

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
        {
            var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(id);

            if (employee == null)
                return null;

            dto.FirstName = dto.FirstName?.Trim() ?? string.Empty;
            dto.LastName = dto.LastName?.Trim() ?? string.Empty;
            dto.Email = dto.Email?.Trim() ?? string.Empty;
            dto.EmployeeNumber = dto.EmployeeNumber?.Trim() ?? string.Empty;

            var structuralResult = await _updateValidator.ValidateAsync(dto);
            var failures = new List<ValidationFailure>(structuralResult.Errors);

            failures.AddRange(await ValidateBusinessRulesAsync(dto.BranchId, dto.Email, dto.EmployeeNumber, currentId: id));

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            employee.BranchId = dto.BranchId;
            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.Email = dto.Email;
            employee.Phone = dto.Phone;
            employee.Role = dto.Role!.Value;
            employee.EmployeeNumber = dto.EmployeeNumber;
            employee.HireDate = dto.HireDate;
            employee.IsActive = dto.IsActive;
            employee.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.EmployeeRepository.Update(employee);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<bool> DeactivateEmployeeAsync(int id)
        {
            var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(id);

            if (employee == null)
                return false;

            if (employee.IsActive)
            {
                // No se toca UpdatedAt: la desactivación es un cambio de estado, no una edición
                // de datos (research.md Decisión — ver M2 de /speckit-analyze).
                employee.IsActive = false;
                await _unitOfWork.EmployeeRepository.Update(employee);
                await _unitOfWork.CommitAsync();
            }

            return true;
        }
    }
}
