using System;
using System.Collections.Generic;

namespace ShipmentTracker.Core.DTOs
{
    /// <summary>
    /// Representa una página de resultados junto con la metadata necesaria para paginar.
    /// </summary>
    /// <typeparam name="T">Tipo de los elementos contenidos en la página.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Los elementos de la página actual.
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Número de página actual (1-based).
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Tamaño de página efectivamente usado.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total de registros que cumplen el filtro aplicado, sin paginar.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Total de páginas disponibles dado <see cref="TotalCount"/> y <see cref="PageSize"/>.
        /// </summary>
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    }
}
