using LoggingAndMonitoringAPIExample.Logic.Models.Customer;
using LoggingAndMonitoringAPIExample.Logic.Params;
using Microsoft.AspNetCore.Mvc;
using LoggingAndMonitoringAPIExample.Logic.Services;

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
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        
        public async Task<ActionResult<List<CustomerResponse>>> GetAllCustomerAsync([FromQuery] CustomerResourceParameters customerResourceParameters)
        {
            var customers = await _customerService.GetAllCustomersAsync(customerResourceParameters);

            return customers.Any() ? Ok(customers) : BadRequest();
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CustomerResponse>> CreateCustomerAsync([FromBody] CustomerRequest customerRequest)
        {
            var customer = await _customerService.CreateCustomerAsync(customerRequest);
            return customer is not null ? Ok(customer) : BadRequest();
        }
    }
}
