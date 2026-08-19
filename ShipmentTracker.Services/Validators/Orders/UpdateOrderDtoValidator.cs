using FluentValidation;
using ShipmentTracker.Core.DTOs.Orders;
using ShipmentTracker.Core.Enums;
using System;

namespace ShipmentTracker.Services.Validators.Orders
{
    /// <summary>
    /// Reglas estructurales de actualización de una orden: idénticas a las de creación
    /// (la misma regla condicional HomePickup/DropOff aplica), pero sin CustomerId.
    /// </summary>
    public class UpdateOrderDtoValidator : AbstractValidator<UpdateOrderDto>
    {
        public UpdateOrderDtoValidator()
        {
            RuleFor(x => x.ServiceType)
                .NotNull().WithMessage("El tipo de servicio es requerido.")
                .IsInEnum().WithMessage("El tipo de servicio no es válido.");

            RuleFor(x => x.PickupType)
                .NotNull().WithMessage("El tipo de recogida es requerido.")
                .IsInEnum().WithMessage("El tipo de recogida no es válido.");

            RuleFor(x => x.RecipientName).NotEmpty().WithMessage("El nombre del destinatario es requerido.");
            RuleFor(x => x.RecipientPhone).NotEmpty().WithMessage("El teléfono del destinatario es requerido.");
            RuleFor(x => x.RecipientAddress).NotEmpty().WithMessage("La dirección del destinatario es requerida.");
            RuleFor(x => x.RecipientCity).NotEmpty().WithMessage("La ciudad del destinatario es requerida.");
            RuleFor(x => x.RecipientState).NotEmpty().WithMessage("El estado del destinatario es requerido.");
            RuleFor(x => x.RecipientZipCode).NotEmpty().WithMessage("El código postal del destinatario es requerido.");

            RuleFor(x => x.DeclaredWeightKg).GreaterThan(0).WithMessage("El peso declarado (kg) debe ser mayor que cero.");
            RuleFor(x => x.DeclaredWidthCm).GreaterThan(0).WithMessage("El ancho declarado (cm) debe ser mayor que cero.");
            RuleFor(x => x.DeclaredHeightCm).GreaterThan(0).WithMessage("La altura declarada (cm) debe ser mayor que cero.");
            RuleFor(x => x.DeclaredLengthCm).GreaterThan(0).WithMessage("El largo declarado (cm) debe ser mayor que cero.");

            RuleFor(x => x.QuotedPrice).GreaterThanOrEqualTo(0).WithMessage("El precio cotizado debe ser mayor o igual a cero.");

            RuleFor(x => x.PickupAddress)
                .NotEmpty().WithMessage("La dirección de recogida es requerida para órdenes de recogida a domicilio.")
                .When(x => x.PickupType == PickupType.HomePickup);

            RuleFor(x => x.PickupScheduledAt)
                .NotNull().WithMessage("La fecha programada de recogida es requerida para órdenes de recogida a domicilio.")
                .GreaterThan(DateTime.UtcNow).WithMessage("La fecha programada de recogida debe ser en el futuro.")
                .When(x => x.PickupType == PickupType.HomePickup);

            RuleFor(x => x.OriginBranchId)
                .Null().WithMessage("La sucursal de origen no debe proporcionarse para órdenes de recogida a domicilio.")
                .When(x => x.PickupType == PickupType.HomePickup);

            RuleFor(x => x.OriginBranchId)
                .NotNull().WithMessage("La sucursal de origen es requerida para órdenes de entrega en sucursal.")
                .When(x => x.PickupType == PickupType.DropOff);

            RuleFor(x => x.PickupAddress)
                .Null().WithMessage("La dirección de recogida no debe proporcionarse para órdenes de entrega en sucursal.")
                .When(x => x.PickupType == PickupType.DropOff);

            RuleFor(x => x.PickupScheduledAt)
                .Null().WithMessage("La fecha programada de recogida no debe proporcionarse para órdenes de entrega en sucursal.")
                .When(x => x.PickupType == PickupType.DropOff);
        }
    }
}