using AutoMapper;
using ShipmentTracker.Core.DTOs.ShipmentEvents;
using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Services.Mappings
{
    /// <summary>
    /// Perfil de mapeo de salida para eventos de Shipment y la vista pública de tracking. La lista
    /// Events de ShipmentTrackingDto se puebla a mano en el servicio (requiere el lookup por evento
    /// de su DeliveryAttempt asociado).
    /// </summary>
    public class ShipmentEventMappingProfile : Profile
    {
        public ShipmentEventMappingProfile()
        {
            CreateMap<ShipmentEvent, ShipmentEventDto>();
            CreateMap<ShipmentEvent, TrackingEventDto>();
            CreateMap<Shipment, ShipmentTrackingDto>()
                .ForMember(d => d.Events, opt => opt.Ignore());
        }
    }
}
