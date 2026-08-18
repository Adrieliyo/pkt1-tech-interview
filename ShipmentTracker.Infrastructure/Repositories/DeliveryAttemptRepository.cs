using ShipmentTracker.Core.Entities;
using ShipmentTracker.Core.Interfaces.Repositories;
using ShipmentTracker.Infrastructure.Data;

namespace ShipmentTracker.Infrastructure.Repositories
{
    public class DeliveryAttemptRepository : BaseRepository<DeliveryAttempt>, IDeliveryAttemptRepository
    {
        public DeliveryAttemptRepository(AppDbContext context) : base(context)
        {
        }
    }
}
