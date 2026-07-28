using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Enums
{
    /// <summary>
    /// Representa los posibles estados por los que puede transitar un envío.
    /// 0. Collected: El envío ha sido recolectado y se encuentra en preparación para su traslado.
    /// 1. InTransit: El envío se encuentra en camino hacia su destino.
    /// 2. Delivered: El envío ha sido entregado de manera efectiva al destinatario.
    /// 3. Cancelled: El envío ha sido cancelado y ya no se encuentra en proceso.
    /// </summary>
    public enum ShipmentStatus
    {
        Collected,
        InTransit,
        Delivered,
        Cancelled
    }
}
