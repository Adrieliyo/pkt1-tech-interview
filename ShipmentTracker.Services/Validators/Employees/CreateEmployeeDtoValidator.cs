using FluentValidation;
using ShipmentTracker.Core.DTOs.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Services.Validators.Employees
{
    public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("El nombre es requerido.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("El apellido es requerido.");
            RuleFor(x => x.EmployeeNumber).NotEmpty().WithMessage("El número de empleado es requerido.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es requerido.")
                .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.");

            RuleFor(x => x.Role)
                .NotNull().WithMessage("El rol es requerido.")
                .IsInEnum().WithMessage("El rol no es válido.");

            RuleFor(x => x.HireDate)
                .NotEqual(default(DateOnly)).WithMessage("La fecha de contratación es requerida.");
        }
    }
}
