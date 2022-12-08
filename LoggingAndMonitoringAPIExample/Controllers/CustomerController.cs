using LoggingAndMonitoringAPIExample.Models.Customer;
using LoggingAndMonitoringAPIExample.Params;
using LoggingAndMonitoringAPIExample.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoggingAndMonitoringAPIExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<CustomerResponse>>> GetAllCustomerAsync([FromQuery] CustomerResourceParameters customerResourceParameters)
        {
            return  await _customerService.GetAllCustomersAsync(customerResourceParameters);
        }
    }
}
