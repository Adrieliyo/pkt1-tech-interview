using ShipmentTracker.Core.Entities;
using ShipmentTracker.Core.Interfaces.Repositories;
using ShipmentTracker.Infrastructure.Data;

namespace ShipmentTracker.Infrastructure.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }
    }
}