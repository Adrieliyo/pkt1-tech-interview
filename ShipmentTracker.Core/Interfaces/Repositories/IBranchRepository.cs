using ShipmentTracker.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Interfaces.Repositories
{
    public interface IBranchRepository : IBaseRepository<Branch>
    {
        // Recupera una sucursal con su horario ya cargado (Include) — GetByIdAsync (FindAsync) no soporta Include
        Task<Branch?> GetByIdWithScheduleAsync(int id);
    }
}
