using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.DTOs.Customers;
using ShipmentTracker.Core.Enums;

namespace ShipmentTracker.Core.Interfaces.Services
{
    public interface ICustomerService
    {
        /// <summary>
        /// Crea un nuevo cliente Individual. El servicio marca el cliente como activo y asigna CreatedAt.
        /// </summary>
        Task<CustomerDetailDto> CreateIndividualAsync(CreateIndividualCustomerDto dto);

        /// <summary>
        /// Crea un nuevo cliente Business. El servicio marca el cliente como activo y asigna CreatedAt.
        /// </summary>
        Task<CustomerDetailDto> CreateBusinessAsync(CreateBusinessCustomerDto dto);

        /// <summary>
        /// Lista clientes de forma paginada, opcionalmente filtrados por estado activo/inactivo y por tipo.
        /// </summary>
        Task<PagedResult<CustomerDetailDto>> GetCustomersAsync(bool onlyActive = true, CustomerType? type = null, int page = 1, int pageSize = 5);

        /// <summary>
        /// Obtiene un cliente por su identificador, activo o inactivo.
        /// </summary>
        Task<CustomerDetailDto?> GetCustomerByIdAsync(int id);

        /// <summary>
        /// Reemplaza por completo los datos editables de un cliente existente. El tipo (Individual
        /// o Business) nunca cambia — no restringe la edición según el estado activo/inactivo actual.
        /// </summary>
        Task<CustomerDetailDto?> UpdateCustomerAsync(int id, UpdateCustomerDto dto);

        /// <summary>
        /// Desactiva un cliente (soft-delete). Nunca elimina el registro. Idempotente: repetir la
        /// operación sobre un cliente ya inactivo no produce error.
        /// </summary>
        Task<bool> DeactivateCustomerAsync(int id);
    }
}
