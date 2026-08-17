using AutoMapper;
using ShipmentTracker.Core.DTOs.Branches;
using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Services.Mappings
{
    public class BranchMappingProfile : Profile
    {
        public BranchMappingProfile()
        {
            CreateMap<Branch, BranchDto>();
            CreateMap<BranchSchedule, ScheduleEntryDto>();
        }
    }
}
