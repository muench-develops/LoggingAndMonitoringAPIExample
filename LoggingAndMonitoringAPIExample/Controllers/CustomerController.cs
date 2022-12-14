using System.Text.Json;
using LoggingAndMonitoringAPIExample.Logic.Parameters;
using Microsoft.AspNetCore.Mvc;
using LoggingAndMonitoringAPIExample.Logic.Services;
using LoggingAndMonitoringAPIExample.Logic.Models;
using AutoMapper;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using Microsoft.Extensions.Caching.Memory;
using LoggingAndMonitoringAPIExample.Handler;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace LoggingAndMonitoringAPIExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerDependencyHandler _dependencyHandler;
        
        private readonly ILogger<CustomerController> _logger;
        private readonly IMapper _mapper;
        private readonly ICustomerService _customerService;

        public CustomerController(CustomerDependencyHandler dependencyHandler)
        {
            _dependencyHandler = dependencyHandler;
            
            _logger = dependencyHandler.GetLoggerFactory().CreateLogger<CustomerController>();
            _mapper = dependencyHandler.GetMapper();
            _customerService = dependencyHandler.GetCustomerService();
        }
        
        [HttpGet("{customerId}", Name = "GetCustomer")]
        [Produces("application/json", "application/xml", Type = typeof(CustomerDto))]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int customerId)
        {
            _logger.LogInformation("Executing {Action} with Id {customerId}", nameof(GetCustomer), customerId);
            
            var customer = await _customerService.GetCustomerAsync(customerId);

            if (customer != null)
            {
                var result = _mapper.Map<CustomerDto>(customer);

                return Ok(result);
            }

            _logger.LogWarning("No customer found with Id {customerId}", customerId);

            return NotFound();
        }

        [HttpPost]
        [Produces("application/json", "application/xml", Type = typeof(CustomerDto))]
        public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CustomerForCreationDto customerRequest)
        {
            _logger.LogInformation("Creating new customer with request {@CustomerRequest}", JsonSerializer.Serialize(customerRequest));

            var customerEntity = _mapper.Map<Customer>(customerRequest);
            var customer = await _customerService.CreateCustomerAsync(customerEntity);

            if (await _customerService.GetExistsAsync(customer.Id))
            {
                var customerToReturn = _mapper.Map<CustomerDto>(customer);
                return CreatedAtRoute("GetCustomer", new
                {
                    customerId = customerToReturn.Id
                }, customerToReturn);
            };

            return BadRequest();
        }
    }
}
