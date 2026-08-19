using FluentValidation;
using ShipmentTracker.Core.DTOs.Auth;

namespace ShipmentTracker.Services.Validators.Auth
{
    /// <summary>Reglas estructurales para el intento de login: sin acceso a repositorio.</summary>
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es requerido.")
                .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida.");
        }
    }
}
