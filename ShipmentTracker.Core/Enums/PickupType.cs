namespace ShipmentTracker.Core.Enums
{
    /// <summary>
    /// Representa cómo se recolectará el paquete de la orden de envío.<br/><br/>
    /// <b>HomePickup:</b> el cliente entrega el paquete a un mensajero en una dirección de recolección programada.<br/><br/>
    /// <b>DropOff:</b> el cliente lleva el paquete a una sucursal de origen.
    /// </summary>
    public enum PickupType
    {
        HomePickup,
        DropOff
    }
}