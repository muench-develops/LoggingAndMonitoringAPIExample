using AutoMapper;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Models;

namespace LoggingAndMonitoringAPIExample.Logic
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<CustomerDto, Customer>();
            CreateMap<CustomerForCreationDto, Customer>();
                
            CreateMap<Customer, CustomerForCreationDto>();
            CreateMap<CustomerForCreationDto, CustomerDto>();
            CreateMap<CustomerDto, CustomerForCreationDto>();
            CreateMap<IEnumerable<CustomerForCreationDto>, IEnumerable<CustomerDto>>();
            CreateMap<IEnumerable<CustomerDto>, IEnumerable<CustomerForCreationDto>>();
        }
    }
}
