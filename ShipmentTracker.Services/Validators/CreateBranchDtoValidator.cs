using FluentValidation;
using ShipmentTracker.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Services.Validators
{
    public class CreateBranchDtoValidator : AbstractValidator<CreateBranchDto>
    {
        public CreateBranchDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre es requerido.");
            RuleFor(x => x.Address).NotEmpty().WithMessage("La dirección es requerida.");
            RuleFor(x => x.City).NotEmpty().WithMessage("La ciudad es requerida.");
            RuleFor(x => x.State).NotEmpty().WithMessage("El estado es requerido.");
            RuleFor(x => x.ZipCode).NotEmpty().WithMessage("El código postal es requerido.");

            RuleFor(x => x.Type)
                .NotNull().WithMessage("El tipo de sucursal es requerido.")
                .IsInEnum().WithMessage("El tipo de sucursal no es válido.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .When(x => x.Latitude.HasValue)
                .WithMessage("La latitud debe estar entre -90 y 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .When(x => x.Longitude.HasValue)
                .WithMessage("La longitud debe estar entre -180 y 180.");

            RuleFor(x => x.Schedule)
                .Must(schedule => schedule != null && schedule.Count == 7)
                .WithMessage("El horario debe tener exactamente 7 entradas, una por día de la semana.");

            RuleFor(x => x.Schedule)
                .Must(schedule => schedule == null || schedule.Select(s => s.DayOfWeek).Distinct().Count() == schedule.Count)
                .WithMessage("El horario no puede tener días duplicados.");

            RuleForEach(x => x.Schedule).SetValidator(new ScheduleEntryInputDtoValidator());
        }
    }
}
