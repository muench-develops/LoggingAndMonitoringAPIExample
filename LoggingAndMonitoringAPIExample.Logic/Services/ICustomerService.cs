using LoggingAndMonitoringAPIExample.Logic.Models.Customer;
using LoggingAndMonitoringAPIExample.Logic.Params;

namespace LoggingAndMonitoringAPIExample.Logic.Services
{
    public interface ICustomerService
    {
        public Task<List<CustomerResponse>> GetAllCustomersAsync(CustomerResourceParameters customerResourceParameters);
        public Task<CustomerResponse> CreateCustomerAsync(CustomerRequest customerRequest);
    }
}
