using AutoMapper;
using ShipmentTracker.Core.DTOs.Shipments;
using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Services.Mappings
{
    public class ShipmentMappingProfile : Profile
    {
        public ShipmentMappingProfile()
        {
            CreateMap<Shipment, ShipmentDto>();
        }
    }
}
