using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repositorio de órdenes. Hereda toda la funcionalidad base (GetAsync, CountAsync, SingleOrDefaultAsync, AddAsync, Update).
    /// </summary>
    public interface IOrderRepository : IBaseRepository<Order>
    {
    }
}