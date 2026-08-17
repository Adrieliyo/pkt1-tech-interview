using AutoMapper;
using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Services.Mappings
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {
            CreateMap<Employee, EmployeeDto>();
        }
    }
}
