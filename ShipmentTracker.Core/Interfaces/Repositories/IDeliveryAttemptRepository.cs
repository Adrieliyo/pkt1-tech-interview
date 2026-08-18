using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repositorio de intentos de entrega. Hereda toda la funcionalidad base; AttemptNumber se
    /// calcula vía CountAsync con un filtro sobre la navegación ShipmentEvent.ShipmentId.
    /// </summary>
    public interface IDeliveryAttemptRepository : IBaseRepository<DeliveryAttempt>
    {
    }
}
