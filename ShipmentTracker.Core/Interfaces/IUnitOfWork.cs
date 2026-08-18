using ShipmentTracker.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IShipmentRepository ShipmentRepository { get; }

        IBranchRepository BranchRepository { get; }

        IEmployeeRepository EmployeeRepository { get; }

        IVehicleRepository VehicleRepository { get; }

        ICustomerRepository CustomerRepository { get; }

        Task<int> CommitAsync();
    }
}
