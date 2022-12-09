using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Parameters;

namespace LoggingAndMonitoringAPIExample.Logic.Services
{
    public interface ICustomerService
    {
        public Task<IEnumerable<Customer>> GetAllCustomersAsync(CustomerResourceParameters customerResourceParameters);
        public Task<Customer> CreateCustomerAsync(Customer customer);
        public Task<IEnumerable<Customer>> CreateCustomersAsync(IEnumerable<Customer> customers);
        public Task<Customer?> GetCustomerAsync(int customerId);
        public Task<bool> GetExistsAsync(int customerId);
    }
}
