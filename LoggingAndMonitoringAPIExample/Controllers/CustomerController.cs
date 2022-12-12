using System.Text.Json;
using LoggingAndMonitoringAPIExample.Logic.Parameters;
using Microsoft.AspNetCore.Mvc;
using LoggingAndMonitoringAPIExample.Logic.Services;
using LoggingAndMonitoringAPIExample.Logic.Models;
using AutoMapper;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using Microsoft.Extensions.Caching.Memory;
using LoggingAndMonitoringAPIExample.Handler;

namespace LoggingAndMonitoringAPIExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerControllerDependencyHandler _dependencyHandler;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(CustomerControllerDependencyHandler dependencyHandler)
        {
            _dependencyHandler = dependencyHandler;
            _logger = dependencyHandler.GetLoggerFactory().CreateLogger<CustomerController>();
        }


        [HttpGet]
        [HttpHead]
        [Produces("application/json", "application/xml", Type = typeof(IEnumerable<CustomerDto>))]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers([FromQuery] CustomerResourceParameters customerResourceParameters)
        {
            _logger.LogInformation("Executing {Action} with parameters: {@Parameters}", nameof(GetCustomers), customerResourceParameters);
    
            var customers = GetCustomersFromCache(customerResourceParameters);

            if (customers == null)
            {
                customers = await GetCustomersFromService(customerResourceParameters);
                AddCustomersToCache(customerResourceParameters, customers);
            }

            return Ok(_dependencyHandler.GetMapper().Map<IEnumerable<CustomerDto>>(customers));
        }

        private IEnumerable<Customer>? GetCustomersFromCache(CustomerResourceParameters customerResourceParameters)
        {
            var cacheKey = $"customer_{customerResourceParameters.FirstName}_{customerResourceParameters.LastName}_{customerResourceParameters.Email}_{customerResourceParameters.SearchQuery}";
            var cachedCustomers = _dependencyHandler.GetCache().Get<IEnumerable<Customer>>(cacheKey);

            if (cachedCustomers == null) return null;
            _logger.LogInformation("Returning cached customers");
            return cachedCustomers;

        }

        private async Task<IEnumerable<Customer>> GetCustomersFromService(CustomerResourceParameters customerResourceParameters)
        {
            return await _dependencyHandler.GetCustomerService().GetAllCustomersAsync(customerResourceParameters);
        }

        private void AddCustomersToCache(CustomerResourceParameters customerResourceParameters, IEnumerable<Customer> customers)
        {
            var cacheKey = $"customer_{customerResourceParameters.FirstName}_{customerResourceParameters.LastName}_{customerResourceParameters.Email}_{customerResourceParameters.SearchQuery}";
            _dependencyHandler.GetCache().Set(cacheKey, customers, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60)));
        }


        [HttpGet("{customerId}", Name = "GetCustomer")]
        [Produces("application/json", "application/xml", Type = typeof(CustomerDto))]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int customerId)
        {
            _logger.LogInformation("Executing {Action} with parameters {Parameters}", nameof(GetCustomer), System.Text.Json.JsonSerializer.Serialize(customerId));
            
            var customer = await _dependencyHandler.GetCustomerService().GetCustomerAsync(customerId);

            if (customer != null)
            {
                return Ok(_dependencyHandler.GetMapper().Map<CustomerDto>(customer));
            }

            return NotFound();
        }

        [HttpPost]
        [Produces("application/json", "application/xml", Type = typeof(CustomerDto))]
        public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CustomerForCreationDto customerRequest)
        {
            _logger.LogInformation("Creating new customer with request {@CustomerRequest}", customerRequest);

            var customerEntity = _dependencyHandler.GetMapper().Map<Customer>(customerRequest);
            var customer = await _dependencyHandler.GetCustomerService().CreateCustomerAsync(customerEntity);

            if (await _dependencyHandler.GetCustomerService().GetExistsAsync(customer.Id))
            {
                var customerToReturn = _dependencyHandler.GetMapper().Map<CustomerDto>(customer);
                return CreatedAtRoute("GetCustomer", new
                {
                    customerId = customerToReturn.Id
                }, customerToReturn);
            };

            return BadRequest();
        }
    }
}
