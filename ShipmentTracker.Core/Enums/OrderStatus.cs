namespace ShipmentTracker.Core.Enums
{
    /// <summary>
    /// Representa el ciclo de vida de una orden de envío.<br/><br/>
    /// <b>Pending:</b> recién creada, editable.<br/><br/>
    /// <b>Confirmed:</b> revisada y aceptada por un operador, ya no editable.<br/><br/>
    /// <b>Converted:</b> se generó un Shipment a partir de esta orden. Estado terminal.<br/><br/>
    /// <b>Cancelled:</b> cancelada antes de la conversión. Estado terminal.
    /// </summary>
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Converted,
        Cancelled
    }
}