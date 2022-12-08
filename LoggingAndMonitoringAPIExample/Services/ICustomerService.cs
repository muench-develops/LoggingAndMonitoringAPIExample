using LoggingAndMonitoringAPIExample.Models.Customer;
using LoggingAndMonitoringAPIExample.Params;

namespace LoggingAndMonitoringAPIExample.Services
{
    public interface ICustomerService
    {
        public Task<List<CustomerResponse>> GetAllCustomersAsync(CustomerResourceParameters customerResourceParameters);
    }
}
