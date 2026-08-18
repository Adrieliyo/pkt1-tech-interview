using FluentValidation;
using ShipmentTracker.Core.DTOs.Auth;

namespace ShipmentTracker.Services.Validators.Auth
{
    /// <summary>
    /// Reglas estructurales para el aprovisionamiento de una cuenta: sin acceso a repositorio (la
    /// existencia/estado activo del Employee y la unicidad de cuenta se validan en UserService).
    /// La política de contraseña replica la configurada en Identity (Program.cs): 8+ caracteres,
    /// al menos un dígito, al menos una mayúscula.
    /// </summary>
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.");
        }
    }
}
