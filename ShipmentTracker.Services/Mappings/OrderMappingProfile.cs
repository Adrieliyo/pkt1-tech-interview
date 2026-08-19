using AutoMapper;
using ShipmentTracker.Core.DTOs.Orders;
using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Services.Mappings
{
    /// <summary>
    /// Perfil de mapeo de salida para órdenes (entidad → DTO). No se usa para construir entidades.
    /// </summary>
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(d => d.OriginBranchName, opt => opt.Ignore());
        }
    }
}