using FluentValidation;
using ShipmentTracker.Core.DTOs.Customers;

namespace ShipmentTracker.Services.Validators.Customers
{
    public class CreateIndividualCustomerDtoValidator : AbstractValidator<CreateIndividualCustomerDto>
    {
        // 4 letras + 6 dígitos (fecha) + sexo + 2 letras (entidad) + 3 consonantes + homoclave + dígito verificador = 18
        private const string CurpPattern = "^[A-Z]{4}\\d{6}[HM][A-Z]{5}[A-Z0-9]\\d$";

        public CreateIndividualCustomerDtoValidator()
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

            RuleFor(x => x.FirstName).NotEmpty().WithMessage("El nombre es requerido.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("El apellido es requerido.");

            RuleFor(x => x.GovernmentId)
                .NotEmpty().WithMessage("El identificador gubernamental (CURP) es requerido.")
                .Matches(CurpPattern).WithMessage("El identificador gubernamental no tiene el formato oficial de CURP (18 caracteres alfanuméricos).");
        }
    }
}
