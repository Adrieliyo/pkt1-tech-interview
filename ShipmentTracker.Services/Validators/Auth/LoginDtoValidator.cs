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
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is not a valid email address.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
