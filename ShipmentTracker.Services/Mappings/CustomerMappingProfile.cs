using AutoMapper;
using ShipmentTracker.Core.DTOs.Customers;
using ShipmentTracker.Core.Entities;

namespace ShipmentTracker.Services.Mappings
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<Customer, CustomerDetailDto>()
                .Include<IndividualCustomer, CustomerDetailDto>()
                .Include<BusinessCustomer, CustomerDetailDto>();

            CreateMap<IndividualCustomer, CustomerDetailDto>()
                .AfterMap((src, dest) => dest.Individual = new IndividualDetailDto
                {
                    FirstName = src.FirstName,
                    LastName = src.LastName,
                    BirthDate = src.BirthDate,
                    GovernmentId = src.GovernmentId
                });

            CreateMap<BusinessCustomer, CustomerDetailDto>()
                .AfterMap((src, dest) => dest.Business = new BusinessDetailDto
                {
                    BusinessName = src.BusinessName,
                    TaxId = src.TaxId,
                    LegalRepresentative = src.LegalRepresentative,
                    Industry = src.Industry,
                    CreditLimit = src.CreditLimit
                });
        }
    }
}
