using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.DTOs.Employees;
using ShipmentTracker.Core.Entities;
using ShipmentTracker.Core.Enums;
using ShipmentTracker.Core.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateEmployeeDto> createValidator, IValidator<UpdateEmployeeDto> updateValidator, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _userManager = userManager;
        }

        // LINQ síncrono sobre UserManager.Users (IQueryable respaldado por EF Core) — mismo patrón
        // que UserService.CreateUserForEmployeeAsync, para no añadir una referencia a EF Core en
        // Services.
        private bool HasAccount(int employeeId) =>
            _userManager.Users.Any(u => u.EmployeeId == employeeId);

        private HashSet<int> EmployeeIdsWithAccounts(IEnumerable<int> employeeIds) =>
            _userManager.Users
                .Where(u => u.EmployeeId != null && employeeIds.Contains(u.EmployeeId.Value))
                .Select(u => u.EmployeeId!.Value)
                .ToHashSet();

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

            // A newly created employee can never already have an account.
            var createdDto = _mapper.Map<EmployeeDto>(employee);
            createdDto.HasAccount = false;
            return createdDto;
        }

        public async Task<PagedResult<EmployeeDto>> GetEmployeesAsync(bool onlyActive = true, int? branchId = null, EmployeeRole? role = null, int page = 1, int pageSize = 5)
        {
            Expression<Func<Employee, bool>> filter;
            if (branchId.HasValue && role.HasValue)
            {
                filter = x => x.IsActive == onlyActive && x.BranchId == branchId.Value && x.Role == role.Value;
            }
            else if (branchId.HasValue)
            {
                filter = x => x.IsActive == onlyActive && x.BranchId == branchId.Value;
            }
            else if (role.HasValue)
            {
                filter = x => x.IsActive == onlyActive && x.Role == role.Value;
            }
            else
            {
                filter = x => x.IsActive == onlyActive;
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

            var items = employees.Select(e => _mapper.Map<EmployeeDto>(e)).ToList();
            var accountIds = EmployeeIdsWithAccounts(items.Select(e => e.Id));
            foreach (var item in items)
            {
                item.HasAccount = accountIds.Contains(item.Id);
            }

            return new PagedResult<EmployeeDto>
            {
                Items = items,
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

            var dto = _mapper.Map<EmployeeDto>(employee);
            dto.HasAccount = HasAccount(employee.Id);
            return dto;
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

            var previousRole = employee.Role;

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

            if (previousRole != employee.Role)
            {
                await SyncAccountRoleAsync(employee.Id, employee.Role);
            }

            var updatedDto = _mapper.Map<EmployeeDto>(employee);
            updatedDto.HasAccount = HasAccount(employee.Id);
            return updatedDto;
        }

        // Mantiene sincronizado el rol de Identity (AspNetUserRoles) con Employee.Role tras un cambio
        // de rol en la edición. Sin esto, el rol de sesión (claims, [Authorize], sidebar) quedaba
        // desactualizado indefinidamente: EmployeeSessionValidator detecta el desfase en cada request
        // pero solo puede reconstruir el principal a partir de AspNetUserRoles, que nunca cambiaba. Es
        // un no-op si el empleado no tiene cuenta vinculada (mismo patrón de HasAccount/lookup por
        // EmployeeId).
        private async Task SyncAccountRoleAsync(int employeeId, EmployeeRole newRole)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.EmployeeId == employeeId);
            if (user == null)
            {
                return;
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var newRoleName = newRole.ToString();

            if (currentRoles.Contains(newRoleName) && currentRoles.Count == 1)
            {
                return;
            }

            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            await _userManager.AddToRoleAsync(user, newRoleName);
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
