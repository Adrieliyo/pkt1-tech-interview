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
                .NotNull().WithMessage("El tipo de evento es requerido.")
                .IsInEnum().WithMessage("El tipo de evento no es válido.")
                .Must(t => t != ShipmentEventType.DeliveryAttempted && t != ShipmentEventType.OrderConverted)
                .WithMessage("El tipo de evento no puede ser DeliveryAttempted ni OrderConverted — use sus rutas de creación dedicadas.");

            RuleFor(x => x.OccurredAt)
                .LessThanOrEqualTo(_ => DateTime.UtcNow).WithMessage("La fecha del evento no debe ser en el futuro.");
        }
    }
}
