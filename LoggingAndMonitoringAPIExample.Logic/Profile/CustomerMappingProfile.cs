using AutoMapper;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
