using FluentValidation;
using ShipmentTracker.Core.DTOs.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Services.Validators.Vehicles
{
    public class UpdateVehicleDtoValidator : AbstractValidator<UpdateVehicleDto>
    {
        public UpdateVehicleDtoValidator()
        {
            RuleFor(x => x.Plate).NotEmpty().WithMessage("La placa es requerida.");
            RuleFor(x => x.Brand).NotEmpty().WithMessage("La marca es requerida.");
            RuleFor(x => x.Model).NotEmpty().WithMessage("El modelo es requerido.");

            RuleFor(x => x.Type)
                .NotNull().WithMessage("El tipo de vehículo es requerido.")
                .IsInEnum().WithMessage("El tipo de vehículo no es válido.");

            RuleFor(x => x.Year)
                .LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("El año de fabricación no puede ser futuro.");

            RuleFor(x => x.MaxWeightKg)
                .GreaterThan(0).WithMessage("La capacidad máxima de carga debe ser positiva.");
        }
    }
}
