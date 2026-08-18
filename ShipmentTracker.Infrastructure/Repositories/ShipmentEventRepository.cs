using ShipmentTracker.Core.Entities;
using ShipmentTracker.Core.Interfaces.Repositories;
using ShipmentTracker.Infrastructure.Data;

namespace ShipmentTracker.Infrastructure.Repositories
{
    public class ShipmentEventRepository : BaseRepository<ShipmentEvent>, IShipmentEventRepository
    {
        public ShipmentEventRepository(AppDbContext context) : base(context)
        {
        }
    }
}