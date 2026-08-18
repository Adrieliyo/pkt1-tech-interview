using FluentValidation;
using ShipmentTracker.Core.DTOs.ShipmentEvents;
using ShipmentTracker.Core.Enums;
using System;

namespace ShipmentTracker.Services.Validators.ShipmentEvents
{
    /// <summary>
    /// Reglas estructurales para el registro de un evento genérico: sin acceso a repositorio
    /// (la existencia/estado activo del Employee y la legalidad de la transición se validan en
    /// ShipmentEventService).
    /// </summary>
    public class RegisterEventDtoValidator : AbstractValidator<RegisterEventDto>
    {
        public RegisterEventDtoValidator()
        {
            RuleFor(x => x.EventType)
                .NotNull().WithMessage("EventType is required.")
                .IsInEnum().WithMessage("EventType is not a valid event type.")
                .Must(t => t != ShipmentEventType.DeliveryAttempted && t != ShipmentEventType.OrderConverted)
                .WithMessage("EventType must not be DeliveryAttempted or OrderConverted — use their dedicated creation paths.");

            RuleFor(x => x.OccurredAt)
                .LessThanOrEqualTo(_ => DateTime.UtcNow).WithMessage("OccurredAt must not be in the future.");
        }
    }
}
