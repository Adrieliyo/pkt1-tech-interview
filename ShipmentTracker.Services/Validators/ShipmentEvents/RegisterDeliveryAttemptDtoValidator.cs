using FluentValidation;
using ShipmentTracker.Core.DTOs.ShipmentEvents;
using System;

namespace ShipmentTracker.Services.Validators.ShipmentEvents
{
    /// <summary>
    /// Reglas estructurales para el registro de un intento de entrega. No usa Include()/herencia con
    /// RegisterEventDtoValidator (research.md Decisión 10) — las reglas dependientes de base de datos
    /// compartidas viven en ShipmentEventService.ValidateEmployeeAsync.
    /// </summary>
    public class RegisterDeliveryAttemptDtoValidator : AbstractValidator<RegisterDeliveryAttemptDto>
    {
        public RegisterDeliveryAttemptDtoValidator()
        {
            RuleFor(x => x.OccurredAt)
                .LessThanOrEqualTo(_ => DateTime.UtcNow).WithMessage("La fecha del evento no debe ser en el futuro.");

            RuleFor(x => x.FailureReason)
                .NotNull().WithMessage("El motivo del fallo es requerido.")
                .IsInEnum().WithMessage("El motivo del fallo no es válido.");

            RuleFor(x => x.NextAttemptAt)
                .GreaterThan(x => x.OccurredAt).WithMessage("La fecha del próximo intento debe ser posterior a la fecha del evento.")
                .When(x => x.NextAttemptAt.HasValue);
        }
    }
}
