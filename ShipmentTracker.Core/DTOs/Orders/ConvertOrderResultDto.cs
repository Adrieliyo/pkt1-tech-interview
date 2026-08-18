namespace ShipmentTracker.Core.DTOs.Orders
{
    /// <summary>
    /// Resultado de convertir una orden confirmada en un Shipment.
    /// </summary>
    public class ConvertOrderResultDto
    {
        /// <summary>
        /// Identificador del Shipment recién creado.
        /// </summary>
        public int ShipmentId { get; set; }

        /// <summary>
        /// Número de guía único del Shipment (TRK-YYYYMMDD-XXXX).
        /// </summary>
        public string TrackingNumber { get; set; } = null!;
    }
}