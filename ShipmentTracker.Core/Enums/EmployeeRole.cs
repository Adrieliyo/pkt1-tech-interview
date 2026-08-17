using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Enums
{
    /// <summary>
    /// Representa el rol de un empleado dentro de la red logística.<br/><br/>
    /// <b>0 - Operator:</b> Encargado de recolecciones.<br/><br/>
    /// <b>1 - Driver:</b> Encargado de la entrega de última milla.<br/><br/>
    /// <b>2 - WarehouseStaff:</b> Personal de almacén.<br/><br/>
    /// <b>3 - BranchManager:</b> Encargado de sucursal.
    /// </summary>
    public enum EmployeeRole
    {
        Operator,
        Driver,
        WarehouseStaff,
        BranchManager
    }
}
