using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Enums
{
    /// <summary>
    /// Representa los posibles estados por los que puede transitar un envío. Persistido como string
    /// (HasConversion&lt;string&gt;()), por lo que el orden/valor numérico de los miembros no afecta
    /// el almacenamiento.<br/><br/>
    /// <b>Collected:</b> El envío ha sido recolectado y se encuentra en preparación para su traslado.<br/><br/>
    /// <b>InTransit:</b> El envío se encuentra en camino hacia su destino.<br/><br/>
    /// <b>OutForDelivery:</b> El envío salió a reparto para su entrega final.<br/><br/>
    /// <b>Delivered:</b> El envío ha sido entregado de manera efectiva al destinatario.<br/><br/>
    /// <b>Cancelled:</b> El envío ha sido cancelado y ya no se encuentra en proceso.
    /// </summary>
    public enum ShipmentStatus
    {
        Collected,
        InTransit,
        OutForDelivery,
        Delivered,
        Cancelled
    }
}
