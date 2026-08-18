using ShipmentTracker.Core.Enums;
using System;
using System.Text.Json.Serialization;

namespace ShipmentTracker.Core.DTOs.Orders
{
    /// <summary>
    /// Datos requeridos para crear una nueva orden de envío. El número de orden y el estado son asignados por el sistema.
    /// </summary>
    public class CreateOrderDto
    {
        /// <summary>
        /// Identificador del Customer propietario de la orden. Debe existir y estar activo.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Identificador opcional de la Branch de origen. Requerido y activa cuando PickupType es DropOff; debe omitirse con HomePickup.
        /// </summary>
        public int? OriginBranchId { get; set; }

        /// <summary>
        /// Tipo de servicio (Standard, Express, Economy). Nullable para distinguir "omitido" de un valor explícito.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ServiceType? ServiceType { get; set; }

        /// <summary>
        /// Tipo de recolección (HomePickup o DropOff). Nullable para distinguir "omitido" de un valor explícito.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PickupType? PickupType { get; set; }

        /// <summary>
        /// Dirección de recolección. Requerida cuando PickupType es HomePickup; debe omitirse con DropOff.
        /// </summary>
        public string? PickupAddress { get; set; }

        /// <summary>
        /// Fecha y hora programada de recolección (ISO 8601). Requerida y en el futuro cuando PickupType es HomePickup; debe omitirse con DropOff.
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
    }
}