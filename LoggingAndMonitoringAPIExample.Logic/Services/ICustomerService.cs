using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Models;
using LoggingAndMonitoringAPIExample.Logic.Params;

namespace LoggingAndMonitoringAPIExample.Logic.Services
{
    public interface ICustomerService
    {
        public Task<List<Customer>> GetAllCustomersAsync(CustomerResourceParameters customerResourceParameters);
        public Task<Customer> CreateCustomerAsync(Customer customer);
    }
}
