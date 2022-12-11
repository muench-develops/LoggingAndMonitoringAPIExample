using AutoMapper;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Models;
using LoggingAndMonitoringAPIExample.Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoggingAndMonitoringAPIExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerCollectionsController : ControllerBase               
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerCollectionsController> _logger;


        public CustomerCollectionsController(ICustomerService customerService, IMapper mapper,ILoggerFactory loggerFactory)
        {
            _customerService = customerService;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<CustomerCollectionsController>();
        }


        [HttpPost]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> CreateCustomerCollection([FromBody] IEnumerable<CustomerForCreationDto> customerCollection)
        {
            _logger.LogInformation("Executing {Action} with parameters: {@Parameters}", nameof(CreateCustomerCollection), customerCollection);
            
            var customerEntities = _mapper.Map<IEnumerable<Customer>>(customerCollection);
            var result = await _customerService.CreateCustomersAsync(customerEntities);

            var customerCollectionToReturn = _mapper.Map<IEnumerable<CustomerDto>>(result);
            var idsAsString = string.Join(",", customerCollectionToReturn.Select(x => x.Id));

            return CreatedAtRoute("GetCustomerCollection", new { ids = idsAsString }, customerCollectionToReturn);
        }
    }
}
