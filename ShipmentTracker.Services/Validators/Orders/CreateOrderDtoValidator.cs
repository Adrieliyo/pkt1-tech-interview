using FluentValidation;
using ShipmentTracker.Core.DTOs.Orders;
using ShipmentTracker.Core.Enums;
using System;

namespace ShipmentTracker.Services.Validators.Orders
{
    /// <summary>
    /// Reglas estructurales de creación de una orden: campos obligatorios, rangos numéricos,
    /// valores de enum válidos y las reglas condicionales HomePickup/DropOff. Sin acceso a repositorio
    /// (la existencia/estado activo del Customer/Branch se valida en OrderService).
    /// </summary>
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.ServiceType)
                .NotNull().WithMessage("ServiceType is required.")
                .IsInEnum().WithMessage("ServiceType is not a valid service type.");

            RuleFor(x => x.PickupType)
                .NotNull().WithMessage("PickupType is required.")
                .IsInEnum().WithMessage("PickupType is not a valid pickup type.");

            RuleFor(x => x.RecipientName).NotEmpty().WithMessage("RecipientName is required.");
            RuleFor(x => x.RecipientPhone).NotEmpty().WithMessage("RecipientPhone is required.");
            RuleFor(x => x.RecipientAddress).NotEmpty().WithMessage("RecipientAddress is required.");
            RuleFor(x => x.RecipientCity).NotEmpty().WithMessage("RecipientCity is required.");
            RuleFor(x => x.RecipientState).NotEmpty().WithMessage("RecipientState is required.");
            RuleFor(x => x.RecipientZipCode).NotEmpty().WithMessage("RecipientZipCode is required.");

            RuleFor(x => x.DeclaredWeightKg).GreaterThan(0).WithMessage("DeclaredWeightKg must be greater than zero.");
            RuleFor(x => x.DeclaredWidthCm).GreaterThan(0).WithMessage("DeclaredWidthCm must be greater than zero.");
            RuleFor(x => x.DeclaredHeightCm).GreaterThan(0).WithMessage("DeclaredHeightCm must be greater than zero.");
            RuleFor(x => x.DeclaredLengthCm).GreaterThan(0).WithMessage("DeclaredLengthCm must be greater than zero.");

            RuleFor(x => x.QuotedPrice).GreaterThanOrEqualTo(0).WithMessage("QuotedPrice must be greater than or equal to zero.");

            RuleFor(x => x.PickupAddress)
                .NotEmpty().WithMessage("PickupAddress is required for HomePickup orders.")
                .When(x => x.PickupType == PickupType.HomePickup);

            RuleFor(x => x.PickupScheduledAt)
                .NotNull().WithMessage("PickupScheduledAt is required for HomePickup orders.")
                .GreaterThan(DateTime.UtcNow).WithMessage("PickupScheduledAt must be in the future.")
                .When(x => x.PickupType == PickupType.HomePickup);

            RuleFor(x => x.OriginBranchId)
                .Null().WithMessage("OriginBranchId must not be provided for HomePickup orders.")
                .When(x => x.PickupType == PickupType.HomePickup);

            RuleFor(x => x.OriginBranchId)
                .NotNull().WithMessage("OriginBranchId is required for DropOff orders.")
                .When(x => x.PickupType == PickupType.DropOff);

            RuleFor(x => x.PickupAddress)
                .Null().WithMessage("PickupAddress must not be provided for DropOff orders.")
                .When(x => x.PickupType == PickupType.DropOff);

            RuleFor(x => x.PickupScheduledAt)
                .Null().WithMessage("PickupScheduledAt must not be provided for DropOff orders.")
                .When(x => x.PickupType == PickupType.DropOff);
        }
    }
}