using AutoMapper;
using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.Entities;
using ShipmentTracker.Web.Models;

namespace ShipmentTracker.Web.Mappers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Shipment, ShipmentModel>();
            CreateMap<Shipment, CreateShipmentDto>();

            CreateMap<ShipmentModel, Shipment>();
            CreateMap<CreateShipmentDto, Shipment>();
        }
    }
}
