using FluentValidation;
using ShipmentTracker.Core.DTOs.Customers;

namespace ShipmentTracker.Services.Validators.Customers
{
    public class CreateBusinessCustomerDtoValidator : AbstractValidator<CreateBusinessCustomerDto>
    {
        // RFC persona moral: 3 letras + 6 dígitos (fecha) + 3 caracteres de homoclave = 12
        private const string RfcPattern = "^[A-Z]{3}\\d{6}[A-Z0-9]{3}$";

        public CreateBusinessCustomerDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es requerido.")
                .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.");

            RuleFor(x => x.Phone).NotEmpty().WithMessage("El teléfono es requerido.");
            RuleFor(x => x.Address).NotEmpty().WithMessage("La dirección es requerida.");
            RuleFor(x => x.City).NotEmpty().WithMessage("La ciudad es requerida.");
            RuleFor(x => x.State).NotEmpty().WithMessage("El estado es requerido.");
            RuleFor(x => x.ZipCode).NotEmpty().WithMessage("El código postal es requerido.");
            RuleFor(x => x.Country).NotEmpty().WithMessage("El país es requerido.");

            RuleFor(x => x.BusinessName).NotEmpty().WithMessage("La razón social es requerida.");
            RuleFor(x => x.LegalRepresentative).NotEmpty().WithMessage("El representante legal es requerido.");

            RuleFor(x => x.TaxId)
                .NotEmpty().WithMessage("El RFC es requerido.")
                .Matches(RfcPattern).WithMessage("El RFC no tiene el formato oficial de persona moral (12 caracteres alfanuméricos).");

            RuleFor(x => x.CreditLimit)
                .GreaterThanOrEqualTo(0).WithMessage("El límite de crédito no puede ser negativo.")
                .When(x => x.CreditLimit.HasValue);
        }
    }
}
