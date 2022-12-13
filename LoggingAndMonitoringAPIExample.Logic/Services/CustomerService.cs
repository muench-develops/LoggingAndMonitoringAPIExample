using System.Text.Json;
using LoggingAndMonitoringAPIExample.Logic.Context;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Parameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LoggingAndMonitoringAPIExample.Logic.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerDbContext _customerContext;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(CustomerDbContext customerContext, ILoggerFactory loggerFactory)
        {
            _customerContext = customerContext;
            _logger = loggerFactory.CreateLogger<CustomerService>();
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync(
            CustomerResourceParameters customerResourceParameters)
        {
            _logger.LogInformation("Executing {Action} {Parameters}", nameof(GetAllCustomersAsync),
                JsonSerializer.Serialize(customerResourceParameters));

            var collection = _customerContext.Customers as IQueryable<Customer>;

            collection = Filter(customerResourceParameters, collection);

            var result = await collection
                .Skip(customerResourceParameters.PageSize * (customerResourceParameters.PageNumber - 1))
                .Take(customerResourceParameters.PageSize)
                .ToListAsync();

            return result;
        }

        private static IQueryable<Customer> Filter(CustomerResourceParameters customerResourceParameters,
            IQueryable<Customer> collection)
        {
            if (IsEmpty(customerResourceParameters.FirstName))
            {
                collection = collection.Where(x => Matches(x.FirstName, customerResourceParameters.FirstName));
            }

            if (IsEmpty(customerResourceParameters.LastName))
            {
                collection = collection.Where(x => Matches(x.LastName, customerResourceParameters.LastName));
            }

            if (IsEmpty(customerResourceParameters.Email))
            {
                collection = collection.Where(x => Matches(x.Email, customerResourceParameters.Email));
            }

            if (IsEmpty(customerResourceParameters.SearchQuery))
            {
                collection = collection.Where(x => Matches(x.FirstName, customerResourceParameters.SearchQuery) ||
                                                   Matches(x.LastName, customerResourceParameters.SearchQuery) ||
                                                   Matches(x.Email, customerResourceParameters.SearchQuery));
            }

            return collection;
        }

        private static bool IsEmpty(string? value)
        {
            return !string.IsNullOrEmpty(value);
        }

        private static bool Matches(string value, string searchTerm)
        {
            return value.ToLower().Contains(searchTerm.ToLower());
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            _logger.LogInformation("Executing {Action} {Parameters}", nameof(CreateCustomerAsync),
                JsonSerializer.Serialize(customer));

            var result = await _customerContext.Customers.AddAsync(customer);
            await _customerContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Customer?> GetCustomerAsync(int id)
        {
            _logger.LogInformation("Executing {Action} {Parameters}", nameof(GetCustomerAsync),
                JsonSerializer.Serialize(id));

            var result = await _customerContext.Customers.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<bool> GetExistsAsync(int customerId)
        {
            _logger.LogInformation("Executing {Action} {Parameters}", nameof(GetExistsAsync),
                JsonSerializer.Serialize(customerId));

            return await _customerContext.Customers.FirstOrDefaultAsync(x => x.Id == customerId) != null;
        }

        public async Task<IEnumerable<Customer>> CreateCustomersAsync(IEnumerable<Customer> customers)
        {
            _logger.LogInformation("Executing {Action} {Parameters}", nameof(CreateCustomersAsync),
                JsonSerializer.Serialize(customers));

            await _customerContext.Customers.AddRangeAsync(customers);
            await _customerContext.SaveChangesAsync();
            return customers;
        }

        
        public async Task<IEnumerable<Customer>> GetCustomersAsync(IEnumerable<int> customerIds)
        {
            _logger.LogInformation("Executing {Action} {Parameters}", nameof(GetCustomersAsync),
                 JsonSerializer.Serialize(customerIds));

            var result = await _customerContext.Customers.Where(x => customerIds.Contains(x.Id)).ToListAsync();

            return result;
        }
    }
}