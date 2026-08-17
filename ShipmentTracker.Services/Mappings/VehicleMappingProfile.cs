using AutoMapper;
using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Services.Mappings
{
    public class VehicleMappingProfile : Profile
    {
        public VehicleMappingProfile()
        {
            CreateMap<Vehicle, VehicleDto>();
        }
    }
}
