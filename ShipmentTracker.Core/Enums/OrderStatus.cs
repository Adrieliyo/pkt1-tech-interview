namespace ShipmentTracker.Core.Enums
{
    /// <summary>
    /// Representa el ciclo de vida de una orden de envío.<br/><br/>
    /// <b>Pending:</b> recién creada, editable.<br/><br/>
    /// <b>Confirmed:</b> revisada y aceptada por un operador, ya no editable.<br/><br/>
    /// <b>Converted:</b> se generó al menos un Shipment a partir de esta orden. Ya no se puede editar
    /// ni cancelar, pero sí seguir generando Shipments adicionales (conversión repetible, ver
    /// IOrderService.ConvertToShipmentAsync) — el cumplimiento agregado de todos sus envíos se calcula
    /// bajo demanda (OrderDto.ShipmentsCount/IsFulfilled), no es un valor propio de este estado.<br/><br/>
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