using ShipmentTracker.Core.Interfaces;
using ShipmentTracker.Core.Interfaces.Repositories;
using ShipmentTracker.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private ShipmentRepository _shipmentRepository;
        private BranchRepository _branchRepository;

        public UnitOfWork(AppDbContext context)
        {
            this._context = context;
        }

        public IShipmentRepository ShipmentRepository => _shipmentRepository ??= new ShipmentRepository(_context);

        public IBranchRepository BranchRepository => _branchRepository ??= new BranchRepository(_context);

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
        
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
