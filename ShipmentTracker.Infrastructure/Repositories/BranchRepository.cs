using Microsoft.EntityFrameworkCore;
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
    public class BranchRepository : BaseRepository<Branch>, IBranchRepository
    {
        public BranchRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Branch?> GetByIdWithScheduleAsync(int id)
        {
            return await Context.Set<Branch>()
                .Include(x => x.Schedule)
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}
