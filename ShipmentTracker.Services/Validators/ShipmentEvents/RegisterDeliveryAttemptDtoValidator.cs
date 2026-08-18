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
                .LessThanOrEqualTo(_ => DateTime.UtcNow).WithMessage("OccurredAt must not be in the future.");

            RuleFor(x => x.FailureReason)
                .NotNull().WithMessage("FailureReason is required.")
                .IsInEnum().WithMessage("FailureReason is not a valid failure reason.");

            RuleFor(x => x.NextAttemptAt)
                .GreaterThan(x => x.OccurredAt).WithMessage("NextAttemptAt must be later than OccurredAt.")
                .When(x => x.NextAttemptAt.HasValue);
        }
    }
}
