namespace ShipmentTracker.Core.Enums
{
    /// <summary>
    /// Representa los tipos de eventos que pueden registrarse en el ciclo de vida de un Shipment.
    /// Se modela de forma abierta para que módulos futuros añadan más miembros sin cambio de esquema
    /// (la columna se persiste como string).<br/><br/>
    /// <b>OrderConverted:</b> el Shipment fue generado a partir de una orden confirmada. Escrito
    /// únicamente por OrderService.ConvertToShipmentAsync — nunca aceptado por el endpoint genérico
    /// de registro de eventos.<br/><br/>
    /// <b>OutForDelivery:</b> el envío salió a reparto para su entrega final. Requiere un Employee
    /// activo con rol Driver.<br/><br/>
    /// <b>DeliveryAttempted:</b> se intentó entregar el envío y no se logró. Tiene su propio endpoint
    /// dedicado y siempre genera un DeliveryAttempt asociado — nunca aceptado por el endpoint
    /// genérico de registro de eventos.<br/><br/>
    /// <b>ReceivedAtBranch, DepartedFromBranch, InTransit:</b> añadidos por el módulo 008
    /// (Authentication &amp; Authorization) para el rol WarehouseStaff. Sin reglas de negocio propias
    /// más allá de la restricción de rol — se registran vía el endpoint genérico existente, igual que
    /// cualquier otro evento permitido (spec 008 FR-015, Assumptions).
    /// </summary>
    public enum ShipmentEventType
    {
        OrderConverted,
        OutForDelivery,
        DeliveryAttempted,
        ReceivedAtBranch,
        DepartedFromBranch,
        InTransit
    }
}