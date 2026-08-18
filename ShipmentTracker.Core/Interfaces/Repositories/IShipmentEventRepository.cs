using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repositorio de eventos de Shipment. Hereda toda la funcionalidad base; escrito internamente por la conversión de órdenes.
    /// </summary>
    public interface IShipmentEventRepository : IBaseRepository<ShipmentEvent>
    {
    }
}