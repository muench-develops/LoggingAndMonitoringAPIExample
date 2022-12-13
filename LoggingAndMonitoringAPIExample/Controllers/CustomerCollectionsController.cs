using AutoMapper;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Models;
using LoggingAndMonitoringAPIExample.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LoggingAndMonitoringAPIExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerCollectionsController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerCollectionsController> _logger;


        public CustomerCollectionsController(ICustomerService customerService, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _customerService = customerService;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<CustomerCollectionsController>();
        }


        [HttpPost]
        [Produces("application/json", "application/xml", Type = typeof(IEnumerable<CustomerDto>))]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> CreateCustomerCollection([FromBody] IEnumerable<CustomerForCreationDto> customerCollection)
        {
            _logger.LogInformation("Executing {Action} with parameters: {Parameters}", nameof(CreateCustomerCollection), JsonSerializer.Serialize(customerCollection));

            var customerEntities = _mapper.Map<IEnumerable<Customer>>(customerCollection);
            var result = await _customerService.CreateCustomersAsync(customerEntities);

            var customerCollectionToReturn = _mapper.Map<IEnumerable<CustomerDto>>(result);
            var idsAsString = string.Join(",", customerCollectionToReturn.Select(x => x.Id));


            return CreatedAtRoute("GetCustomers", new { customerIds = idsAsString }, customerCollectionToReturn);
        }

        [HttpGet("({customerIds})", Name = "GetCustomers")]
        [Produces("application/json", "application/xml", Type = typeof(IEnumerable<CustomerDto>))]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers([ModelBinder][FromRoute] int[] customerIds)
        {
            _logger.LogInformation("Executing {Action} with parameters {Parameters}", nameof(GetCustomers), JsonSerializer.Serialize(customerIds));

            var customers = await _customerService.GetCustomersAsync(customerIds);

            if (customers != null)
            {
                var result = _mapper.Map<IEnumerable<CustomerDto>>(customers);
                return Ok(result);
            }

            return NotFound();
        }
    }
}
