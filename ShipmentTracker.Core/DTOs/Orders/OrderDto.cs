using ShipmentTracker.Core.Enums;
using System;
using System.Text.Json.Serialization;

namespace ShipmentTracker.Core.DTOs.Orders
{
    /// <summary>
    /// Modelo de datos para representar una orden de envío en la capa de transferencia de datos (DTO).
    /// </summary>
    public class OrderDto
    {
        /// <summary>
        /// Identificador único de la orden.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Número de orden legible por humanos (ORD-YYYYMMDD-XXXX), único e inmutable.
        /// </summary>
        public string OrderNumber { get; set; } = null!;

        /// <summary>
        /// Identificador del Customer propietario de la orden.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Identificador opcional de la Branch de origen.
        /// </summary>
        public int? OriginBranchId { get; set; }

        /// <summary>
        /// Nombre de la Branch de origen (null cuando OriginBranchId es null). Resuelto por
        /// OrderService a partir de OriginBranchId — no proviene de una navigation property en
        /// Order, así que AutoMapper lo ignora explícitamente (ver OrderMappingProfile).
        /// </summary>
        public string? OriginBranchName { get; set; }

        /// <summary>
        /// Estado actual de la orden (Pending, Confirmed, Converted, Cancelled).
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Tipo de servicio (Standard, Express, Economy).
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ServiceType ServiceType { get; set; }

        /// <summary>
        /// Tipo de recolección (HomePickup o DropOff).
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PickupType PickupType { get; set; }

        /// <summary>
        /// Dirección de recolección (presente solo con HomePickup).
        /// </summary>
        public string? PickupAddress { get; set; }

        /// <summary>
        /// Fecha y hora programada de recolección (ISO 8601; presente solo con HomePickup).
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
        /// Estado/entidad de destino (texto libre).
        /// </summary>
        public string RecipientState { get; set; } = null!;

        /// <summary>
        /// Código postal del destino.
        /// </summary>
        public string RecipientZipCode { get; set; } = null!;

        /// <summary>
        /// Peso declarado en kilogramos.
        /// </summary>
        public decimal DeclaredWeightKg { get; set; }

        /// <summary>
        /// Ancho declarado en centímetros.
        /// </summary>
        public decimal DeclaredWidthCm { get; set; }

        /// <summary>
        /// Alto declarado en centímetros.
        /// </summary>
        public decimal DeclaredHeightCm { get; set; }

        /// <summary>
        /// Largo declarado en centímetros.
        /// </summary>
        public decimal DeclaredLengthCm { get; set; }

        /// <summary>
        /// Precio cotizado.
        /// </summary>
        public decimal QuotedPrice { get; set; }

        /// <summary>
        /// Notas opcionales de texto libre.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Fecha de creación del registro (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de la última actualización (UTC). Null hasta la primera actualización vía PUT.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Cantidad total de Shipments generados a partir de esta orden (incluye cancelados). Calculado
        /// bajo demanda a partir de los Shipments asociados; no es un valor persistido. Siempre 0 para
        /// órdenes que nunca se convirtieron.
        /// </summary>
        public int ShipmentsCount { get; set; }

        /// <summary>
        /// Indica si la orden está completamente cumplida: existe al menos un Shipment no cancelado y
        /// todos los Shipments no cancelados están en estado Delivered. Calculado bajo demanda; no es un
        /// valor persistido (el campo <see cref="Status"/> no cambia de significado).
        /// </summary>
        public bool IsFulfilled { get; set; }
    }
}