using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.DTOs
{
    /// <summary>
    /// Datos requeridos para la creación de un nuevo envío.
    /// </summary>
    public class CreateShipmentDto
    {
        /// <summary>
        /// Nombre completo de la persona o empresa que recibirá el envío.
        /// </summary>
        public string Recipient { get; set; } = null!;
    }
}
