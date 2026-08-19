using ShipmentTracker.Core.Enums;
using System;

namespace ShipmentTracker.Core.Entities
{
    /// <summary>
    /// Representa una solicitud de envío creada por un cliente antes de que el paquete físico sea recolectado.
    /// Captura la intención del remitente: destinatario, destino, tipo de recolección y dimensiones/peso estimados.
    /// Pertenece exactamente a un Customer y referencia opcionalmente una Branch de origen (obligatoria solo con PickupType.DropOff).
    /// Una orden nunca se elimina: Cancelled es un estado terminal permanente. Converted es permanente
    /// para efectos de edición/cancelación, pero no bloquea generar Shipments adicionales sobre la
    /// misma orden (relación 1:N, ver IOrderService.ConvertToShipmentAsync).
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Identificador único de la orden.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Número de orden legible por humanos, generado automáticamente (ORD-YYYYMMDD-XXXX). Único e inmutable.
        /// </summary>
        public string OrderNumber { get; set; } = null!;

        /// <summary>
        /// Identificador del Customer propietario de la orden.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Identificador opcional de la Branch de origen. Requerido y activa solo cuando PickupType es DropOff; debe ser null con HomePickup.
        /// </summary>
        public int? OriginBranchId { get; set; }

        /// <summary>
        /// Estado actual de la orden (Pending, Confirmed, Converted, Cancelled).
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Tipo de servicio contratado (Standard, Express, Economy).
        /// </summary>
        public ServiceType ServiceType { get; set; }

        /// <summary>
        /// Tipo de recolección (HomePickup o DropOff).
        /// </summary>
        public PickupType PickupType { get; set; }

        /// <summary>
        /// Dirección de recolección. Requerida cuando PickupType es HomePickup; debe ser null con DropOff.
        /// </summary>
        public string? PickupAddress { get; set; }

        /// <summary>
        /// Fecha y hora programada de recolección. Requerida y en el futuro cuando PickupType es HomePickup; debe ser null con DropOff.
        /// </summary>
        public DateTime? PickupScheduledAt { get; set; }

        /// <summary>
        /// Nombre del destinatario.
        /// </summary>
        public string RecipientName { get; set; } = null!;

        /// <summary>
        /// Teléfono del destinatario.
        /// </summary>
        public string RecipientPhone { get; set; } = null!;

        /// <summary>
        /// Calle de la dirección de destino.
        /// </summary>
        public string RecipientAddress { get; set; } = null!;

        /// <summary>
        /// Ciudad de destino.
        /// </summary>
        public string RecipientCity { get; set; } = null!;

        /// <summary>
        /// Estado/entidad de destino (texto libre, sin lista fija de códigos).
        /// </summary>
        public string RecipientState { get; set; } = null!;

        /// <summary>
        /// Código postal del destino.
        /// </summary>
        public string RecipientZipCode { get; set; } = null!;

        /// <summary>
        /// Peso declarado en kilogramos, mayor a cero.
        /// </summary>
        public decimal DeclaredWeightKg { get; set; }

        /// <summary>
        /// Ancho declarado en centímetros, mayor a cero.
        /// </summary>
        public decimal DeclaredWidthCm { get; set; }

        /// <summary>
        /// Alto declarado en centímetros, mayor a cero.
        /// </summary>
        public decimal DeclaredHeightCm { get; set; }

        /// <summary>
        /// Largo declarado en centímetros, mayor a cero.
        /// </summary>
        public decimal DeclaredLengthCm { get; set; }

        /// <summary>
        /// Precio cotizado, mayor o igual a cero.
        /// </summary>
        public decimal QuotedPrice { get; set; }

        /// <summary>
        /// Notas opcionales de texto libre.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Fecha de creación del registro (UTC), asignada por el servicio.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de la última actualización (UTC). Null hasta la primera actualización vía PUT; también se asigna al confirmar, cancelar o convertir.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}