using AutoMapper;
using LoggingAndMonitoringAPIExample.Logic.Context;
using LoggingAndMonitoringAPIExample.Logic.Models.Customer;
using LoggingAndMonitoringAPIExample.Logic.Params;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace LoggingAndMonitoringAPIExample.Logic.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerDbContext _customerContext;
        private readonly MapperConfiguration _config;
        private readonly IMapper _mapper;

        public CustomerService(CustomerDbContext customerContext)
        {
            _customerContext = customerContext;
            _config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Customer, CustomerResponse>();
                cfg.CreateMap<CustomerResponse, Customer>();
                cfg.CreateMap<CustomerRequest, Customer>();
                cfg.CreateMap<Customer, CustomerRequest>();
                cfg.CreateMap<CustomerRequest, CustomerResponse>();
                cfg.CreateMap<CustomerResponse, CustomerRequest>();
            });
            _mapper = _config.CreateMapper();
        }

        public async Task<List<CustomerResponse>> GetAllCustomersAsync()
        {
            var customers = await _customerContext.Customers.ToListAsync();
            var customersResponse = _mapper.Map<List<CustomerResponse>>(customers);
            return customersResponse;
        }

        public async Task<List<CustomerResponse>> GetAllCustomersAsync(CustomerResourceParameters customerResourceParameters)
        {

            var collection = _customerContext.Customers as IQueryable<Customer>;

            collection = Filter(customerResourceParameters, collection);

            var result = await collection.Skip(customerResourceParameters.PageSize * (customerResourceParameters.PageNumber - 1))
                .Take(customerResourceParameters.PageSize)
                .ToListAsync();

            return _mapper.Map<List<CustomerResponse>>(result);
        }

        private static IQueryable<Customer> Filter(CustomerResourceParameters customerResourceParameters, IQueryable<Customer> collection)
        {
            if (!string.IsNullOrEmpty(customerResourceParameters.FirstName))
            {
                collection = collection.Where(x => x.FirstName.ToLower().Contains(customerResourceParameters.FirstName.ToLower()));
            }

            if (!string.IsNullOrEmpty(customerResourceParameters.LastName))
            {
                collection = collection.Where(x => x.LastName.ToLower().Contains(customerResourceParameters.LastName.ToLower()));
            }

            if (!string.IsNullOrEmpty(customerResourceParameters.Email))
            {
                collection = collection.Where(x => x.Email.ToLower().Contains(customerResourceParameters.Email.ToLower()));
            }

            if (!string.IsNullOrEmpty(customerResourceParameters.SearchQuery))
            {
                collection = collection.Where(x => x.FirstName.ToLower().Contains(customerResourceParameters.SearchQuery.ToLower()) ||
                                           x.LastName.ToLower().Contains(customerResourceParameters.SearchQuery.ToLower()) ||
                                           x.Email.ToLower().Contains(customerResourceParameters.SearchQuery.ToLower()));
            }

            return collection;
        }

        public async Task<CustomerResponse> CreateCustomerAsync(CustomerRequest customerRequest)
        {
            var customer = _mapper.Map<Customer>(customerRequest);
            var result = await _customerContext.Customers.AddAsync(customer);
            await _customerContext.SaveChangesAsync();
            return _mapper.Map<CustomerResponse>(result.Entity);
        }

    }
}
