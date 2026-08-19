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
                .GreaterThan(0).WithMessage("El empleado es requerido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un dígito.")
                .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.");
        }
    }
}
