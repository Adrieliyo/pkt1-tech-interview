namespace ShipmentTracker.Core.Enums
{
    /// <summary>
    /// Representa los tipos de eventos que pueden registrarse en el ciclo de vida de un Shipment.
    /// Se modela de forma abierta para que módulos futuros añadan más miembros sin cambio de esquema
    /// (la columna se persiste como string).<br/><br/>
    /// <b>OrderConverted:</b> el Shipment fue generado a partir de una orden confirmada.
    /// </summary>
    public enum ShipmentEventType
    {
        OrderConverted
    }
}