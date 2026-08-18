namespace ShipmentTracker.Core.Enums
{
    /// <summary>
    /// Representa el motivo por el que un intento de entrega no se pudo completar.<br/><br/>
    /// <b>NoOneHome:</b> No había nadie para recibir el envío.<br/><br/>
    /// <b>WrongAddress:</b> La dirección proporcionada es incorrecta o no se pudo localizar.<br/><br/>
    /// <b>Refused:</b> El destinatario rechazó el envío.<br/><br/>
    /// <b>AccessDenied:</b> No se pudo acceder al domicilio o edificio (portón cerrado, etc.).<br/><br/>
    /// <b>Other:</b> Otro motivo no cubierto por las opciones anteriores.
    /// </summary>
    public enum DeliveryFailureReason
    {
        NoOneHome,
        WrongAddress,
        Refused,
        AccessDenied,
        Other
    }
}
