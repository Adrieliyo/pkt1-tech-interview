using FluentValidation;
using ShipmentTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Services.Validators.Shipments
{
    public class StatusTransitionContext
    {
        public ShipmentStatus CurrentStatus { get; set; }
        public ShipmentStatus NewStatus { get; set; }
    }

    public class ShipmentTransitionValidator : AbstractValidator<StatusTransitionContext>
    {
        public ShipmentTransitionValidator()
        {
            RuleFor(x => x.NewStatus)
                .Must(BeAValidTransition)
                .WithMessage(x => $"Transición de estado inválida. No se puede pasar de '{x.CurrentStatus}' a '{x.NewStatus}'.");
        }

        private bool BeAValidTransition(StatusTransitionContext context, ShipmentStatus newStatus)
        {
            if (context.CurrentStatus == newStatus)
                return true;

            if (context.CurrentStatus == ShipmentStatus.Delivered ||
                context.CurrentStatus == ShipmentStatus.Cancelled)
            {
                return false;
            }

            if (context.CurrentStatus == ShipmentStatus.Collected)
            {
                return newStatus == ShipmentStatus.InTransit || newStatus == ShipmentStatus.Cancelled;
            }

            if (context.CurrentStatus == ShipmentStatus.InTransit)
            {
                return newStatus == ShipmentStatus.Delivered || newStatus == ShipmentStatus.Cancelled || newStatus == ShipmentStatus.OutForDelivery;
            }

            if (context.CurrentStatus == ShipmentStatus.OutForDelivery)
            {
                return newStatus == ShipmentStatus.Delivered || newStatus == ShipmentStatus.Cancelled;
            }

            return false;
        }
    }
}
