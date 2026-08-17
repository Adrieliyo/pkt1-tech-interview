using AutoMapper;
using ShipmentTracker.Core.DTOs.Shipments;
using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Web.Mappers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Shipment, CreateShipmentDto>();
            CreateMap<CreateShipmentDto, Shipment>();
        }
    }
}
