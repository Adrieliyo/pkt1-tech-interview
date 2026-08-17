using AutoMapper;
using ShipmentTracker.Core.DTOs.Vehicles;
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
