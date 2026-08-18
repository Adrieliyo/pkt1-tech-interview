using FluentValidation;
using ShipmentTracker.Core.DTOs.Customers;

namespace ShipmentTracker.Services.Validators.Customers
{
    // Reglas de forma únicamente — qué campos son requeridos/prohibidos según el tipo real del
    // cliente se decide en CustomerService, que sí conoce ese dato (research.md Decisión 8).
    public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
    {
        // 4 letras + 6 dígitos (fecha) + sexo + 2 letras (entidad) + 3 consonantes + homoclave + dígito verificador = 18
        private const string CurpPattern = "^[A-Z]{4}\\d{6}[HM][A-Z]{5}[A-Z0-9]\\d$";

        // RFC persona moral: 3 letras + 6 dígitos (fecha) + 3 caracteres de homoclave = 12
        private const string RfcPattern = "^[A-Z]{3}\\d{6}[A-Z0-9]{3}$";

        public UpdateCustomerDtoValidator()
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

            RuleFor(x => x.GovernmentId)
                .Matches(CurpPattern).WithMessage("El identificador gubernamental no tiene el formato oficial de CURP (18 caracteres alfanuméricos).")
                .When(x => !string.IsNullOrEmpty(x.GovernmentId));

            RuleFor(x => x.TaxId)
                .Matches(RfcPattern).WithMessage("El RFC no tiene el formato oficial de persona moral (12 caracteres alfanuméricos).")
                .When(x => !string.IsNullOrEmpty(x.TaxId));

            RuleFor(x => x.CreditLimit)
                .GreaterThanOrEqualTo(0).WithMessage("El límite de crédito no puede ser negativo.")
                .When(x => x.CreditLimit.HasValue);
        }
    }
}
