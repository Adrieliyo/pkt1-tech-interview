using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.DTOs.Orders;
using ShipmentTracker.Core.DTOs.Shipments;
using ShipmentTracker.Core.Enums;
using System.Threading.Tasks;

namespace ShipmentTracker.Core.Interfaces.Services
{
    /// <summary>
    /// Servicio de dominio para el ciclo de vida de las órdenes de envío.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Crea una nueva orden (estado Pending) con número de orden generado automáticamente.
        /// Lanza FluentValidation.ValidationException si fallan las reglas estructurales o de negocio.
        /// </summary>
        Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);

        /// <summary>
        /// Lista órdenes paginadas, opcionalmente filtradas por customerId, status y/o una coincidencia
        /// parcial (contains, case-insensitive) sobre OrderNumber, ordenadas por CreatedAt descendente.
        /// </summary>
        Task<PagedResult<OrderDto>> GetOrdersAsync(int? customerId = null, OrderStatus? status = null, string? orderNumberContains = null, int page = 1, int pageSize = 5);

        /// <summary>
        /// Obtiene una orden por su identificador. Devuelve null si no existe.
        /// </summary>
        Task<OrderDto?> GetOrderByIdAsync(int id);

        /// <summary>
        /// Obtiene una orden por su número de orden. Devuelve null si no existe.
        /// </summary>
        Task<OrderDto?> GetOrderByNumberAsync(string orderNumber);

        /// <summary>
        /// Actualiza una orden pendiente. Devuelve null si no existe; lanza ValidationException por reglas
        /// estructurales/de negocio e InvalidOperationException si la orden no está Pending.
        /// </summary>
        Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto dto);

        /// <summary>
        /// Confirma una orden pendiente (Pending → Confirmed). Devuelve null si no existe;
        /// lanza InvalidOperationException si la orden no está Pending.
        /// </summary>
        Task<OrderDto?> ConfirmOrderAsync(int id);

        /// <summary>
        /// Cancela una orden pendiente (Pending → Cancelled). Devuelve false si no existe;
        /// lanza InvalidOperationException si la orden no está Pending.
        /// </summary>
        Task<bool> CancelOrderAsync(int id);

        /// <summary>
        /// Convierte una orden Confirmed o ya Converted en un nuevo Shipment (Confirmed → Converted, o
        /// Converted → Converted), creando el Shipment, su primer ShipmentEvent y actualizando el
        /// estado de la orden en una sola operación atómica. Puede invocarse repetidamente sobre la
        /// misma orden: cada llamada genera un Shipment independiente con su propio número de guía.
        /// Devuelve null si no existe; lanza InvalidOperationException si la orden está Pending o Cancelled.
        /// </summary>
        Task<ConvertOrderResultDto?> ConvertToShipmentAsync(int id);

        /// <summary>
        /// Lista, de forma paginada, los Shipments generados a partir de una orden (relación 1:N),
        /// ordenados por CreatedAt descendente. Devuelve null si la orden no existe.
        /// </summary>
        Task<PagedResult<ShipmentDto>?> GetShipmentsByOrderAsync(int orderId, int page = 1, int pageSize = 5);
    }
}