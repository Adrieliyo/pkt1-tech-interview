using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using ShipmentTracker.Core.DTOs.Auth;
using ShipmentTracker.Core.Identity;
using ShipmentTracker.Core.Interfaces;
using ShipmentTracker.Core.Interfaces.Services;

namespace ShipmentTracker.Services
{
    /// <summary>
    /// Servicio de dominio para el aprovisionamiento de cuentas de staff (module 008, US4).
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IValidator<CreateUserDto> _createValidator;

        public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IValidator<CreateUserDto> createValidator)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _createValidator = createValidator;
        }

        public async Task<UserSessionDto> CreateUserForEmployeeAsync(CreateUserDto dto)
        {
            var structuralResult = await _createValidator.ValidateAsync(dto);
            if (!structuralResult.IsValid)
            {
                throw new ValidationException(structuralResult.Errors);
            }

            var employee = await _unitOfWork.EmployeeRepository.SingleOrDefaultAsync(x => x.Id == dto.EmployeeId);
            if (employee == null || !employee.IsActive)
            {
                throw new InvalidOperationException("The specified employee does not exist or is not active.");
            }

            // LINQ síncrono (no EF Core async extensions) para no añadir una referencia a EF Core
            // en Services — UserManager.Users es IQueryable, System.Linq basta sin ese paquete.
            var hasExistingUser = _userManager.Users.Any(x => x.EmployeeId == dto.EmployeeId);
            if (hasExistingUser)
            {
                throw new InvalidOperationException("This employee already has a linked account.");
            }

            var user = new ApplicationUser
            {
                UserName = employee.Email,
                Email = employee.Email,
                EmailConfirmed = true,
                EmployeeId = employee.Id
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                var failures = createResult.Errors
                    .Select(e => new ValidationFailure(nameof(CreateUserDto.Password), e.Description))
                    .ToList();
                throw new ValidationException(failures);
            }

            await _userManager.AddToRoleAsync(user, employee.Role.ToString());

            return new UserSessionDto
            {
                UserId = user.Id,
                Email = user.Email,
                EmployeeId = user.EmployeeId,
                FullName = $"{employee.FirstName} {employee.LastName}",
                Role = employee.Role.ToString(),
                BranchId = employee.BranchId
            };
        }
    }
}
