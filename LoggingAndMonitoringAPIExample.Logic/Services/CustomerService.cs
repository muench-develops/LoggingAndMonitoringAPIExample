using AutoMapper;
using LoggingAndMonitoringAPIExample.Logic.Context;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Models;
using LoggingAndMonitoringAPIExample.Logic.Params;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace LoggingAndMonitoringAPIExample.Logic.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerDbContext _customerContext;
        private readonly MapperConfiguration _config;


        public CustomerService(CustomerDbContext customerContext)
        {
            _customerContext = customerContext;
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var customers = await _customerContext.Customers.ToListAsync();
            return customers;
        }

        public async Task<List<Customer>> GetAllCustomersAsync(CustomerResourceParameters customerResourceParameters)
        {

            var collection = _customerContext.Customers as IQueryable<Customer>;

            collection = Filter(customerResourceParameters, collection);

            var result = await collection.Skip(customerResourceParameters.PageSize * (customerResourceParameters.PageNumber - 1))
                .Take(customerResourceParameters.PageSize)
                .ToListAsync();

            return result;
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
        
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            var result = await _customerContext.Customers.AddAsync(customer);
            await _customerContext.SaveChangesAsync();
            return result.Entity;
        }

    }
}
