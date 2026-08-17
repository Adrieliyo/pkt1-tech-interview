using FluentValidation;
using ShipmentTracker.Core.DTOs.Branches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Services.Validators.Branches
{
    public class ScheduleEntryInputDtoValidator : AbstractValidator<ScheduleEntryInputDto>
    {
        public ScheduleEntryInputDtoValidator()
        {
            RuleFor(x => x.DayOfWeek)
                .NotNull().WithMessage("El día de la semana es requerido.")
                .IsInEnum().WithMessage("El día de la semana no es válido.");

            When(x => !x.IsClosed, () =>
            {
                RuleFor(x => x.OpensAt)
                    .NotNull().WithMessage("La hora de apertura es requerida cuando el día no está marcado como cerrado.");
                RuleFor(x => x.ClosesAt)
                    .NotNull().WithMessage("La hora de cierre es requerida cuando el día no está marcado como cerrado.");
                RuleFor(x => x)
                    .Must(x => !x.OpensAt.HasValue || !x.ClosesAt.HasValue || x.OpensAt.Value < x.ClosesAt.Value)
                    .WithMessage("La hora de apertura debe ser estrictamente anterior a la hora de cierre.");
            });

            When(x => x.IsClosed, () =>
            {
                RuleFor(x => x.OpensAt)
                    .Null().WithMessage("Un día marcado como cerrado no puede tener hora de apertura.");
                RuleFor(x => x.ClosesAt)
                    .Null().WithMessage("Un día marcado como cerrado no puede tener hora de cierre.");
            });
        }
    }
}
