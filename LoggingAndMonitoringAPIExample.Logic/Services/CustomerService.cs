using LoggingAndMonitoringAPIExample.Logic.Context;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Parameters;
using Microsoft.EntityFrameworkCore;

namespace LoggingAndMonitoringAPIExample.Logic.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerDbContext _customerContext;

        public CustomerService(CustomerDbContext customerContext)
        {
            _customerContext = customerContext ?? throw new ArgumentNullException(nameof(customerContext));
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync(CustomerResourceParameters customerResourceParameters)
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

        public async Task<Customer?> GetCustomerAsync(int id)
        {
            var result = await _customerContext.Customers.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<bool> GetExistsAsync(int customerId)
        {
            return await _customerContext.Customers.FirstOrDefaultAsync(x => x.Id == customerId) != null;
        }

        public async Task<IEnumerable<Customer>> CreateCustomersAsync(IEnumerable<Customer> customers)
        {
            await _customerContext.Customers.AddRangeAsync(customers);
            return customers;
        }
    }
}
