using ShipmentTracker.Core.Entities;
using ShipmentTracker.Core.Interfaces.Repositories;
using ShipmentTracker.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Infrastructure.Repositories
{
    public class VehicleRepository : BaseRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(AppDbContext context) : base(context)
        {
        }
    }
}
