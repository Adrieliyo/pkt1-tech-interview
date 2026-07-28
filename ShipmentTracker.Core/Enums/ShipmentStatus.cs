using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Enums
{
    /// <summary>
    /// Representa los posibles estados por los que puede transitar un envío.<br/><br/>
    /// <b>0 - Collected:</b> El envío ha sido recolectado y se encuentra en preparación para su traslado.<br/><br/>
    /// <b>1 - InTransit:</b> El envío se encuentra en camino hacia su destino.<br/><br/>
    /// <b>2 - Delivered:</b> El envío ha sido entregado de manera efectiva al destinatario.<br/><br/>
    /// <b>3 - Cancelled:</b> El envío ha sido cancelado y ya no se encuentra en proceso.
    /// </summary>
    public enum ShipmentStatus
    {
        Collected,
        InTransit,
        Delivered,
        Cancelled
    }
}
