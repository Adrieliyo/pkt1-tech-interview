using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Enums
{
    /// <summary>
    /// Representa el tipo de una sucursal dentro de la red logística.<br/><br/>
    /// <b>0 - Headquarters:</b> Sede central de la empresa.<br/><br/>
    /// <b>1 - Hub:</b> Centro de distribución.<br/><br/>
    /// <b>2 - SalesPoint:</b> Punto de venta.<br/><br/>
    /// <b>3 - PickupPoint:</b> Punto de recogida.
    /// </summary>
    public enum BranchType
    {
        Headquarters,
        Hub,
        SalesPoint,
        PickupPoint
    }
}
