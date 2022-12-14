using AutoMapper;
using LoggingAndMonitoringAPIExample.Handler;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Models;
using LoggingAndMonitoringAPIExample.Logic.Parameters;
using LoggingAndMonitoringAPIExample.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Web.Http.ModelBinding.Binders;

namespace LoggingAndMonitoringAPIExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerCollectionsController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerCollectionsController> _logger;
        private readonly IMemoryCache _cache;


        public CustomerCollectionsController(CustomerDependencyHandler dependencyHandler)
        {
            _customerService = dependencyHandler.GetCustomerService();
            _mapper = dependencyHandler.GetMapper();
            _logger = dependencyHandler.GetLoggerFactory().CreateLogger<CustomerCollectionsController>();
            _cache = dependencyHandler.GetCache();
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

        [HttpGet("error/customers", Name = "GetCustomerCollection")]
        [Produces("application/json", "application/xml", Type = typeof(IEnumerable<CustomerDto>))]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomerCollection()
        {
            _logger.LogInformation("Executing {Action}", nameof(GetCustomerCollection));

            throw new NotImplementedException();
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


        [HttpGet]
        [HttpHead]
        [Produces("application/json", "application/xml", Type = typeof(IEnumerable<CustomerDto>))]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers([FromQuery] CustomerResourceParameters customerResourceParameters)
        {
            _logger.LogInformation("Executing {Action} with parameters: {@Parameters}", nameof(GetCustomers), JsonSerializer.Serialize(customerResourceParameters));

            var customers = GetCustomersFromCache(customerResourceParameters);

            if (customers == null)
            {
                customers = await GetCustomersFromService(customerResourceParameters);
                AddCustomersToCache(customerResourceParameters, customers);
            }

            return Ok(_mapper.Map<IEnumerable<CustomerDto>>(customers));
        }

        private async Task<IEnumerable<Customer>> GetCustomersFromService(CustomerResourceParameters customerResourceParameters)
        {
            return await _customerService.GetAllCustomersAsync(customerResourceParameters);
        }

        private IEnumerable<Customer>? GetCustomersFromCache(CustomerResourceParameters customerResourceParameters)
        {
            var cacheKey = $"customer_{customerResourceParameters.FirstName}_{customerResourceParameters.LastName}_{customerResourceParameters.Email}_{customerResourceParameters.SearchQuery}";
            var cachedCustomers = _cache.Get<IEnumerable<Customer>>(cacheKey);

            if (cachedCustomers == null) return null;
            _logger.LogInformation("Returning cached customers");
            return cachedCustomers;

        }

        private void AddCustomersToCache(CustomerResourceParameters customerResourceParameters, IEnumerable<Customer> customers)
        {
            var cacheKey = $"customer_{customerResourceParameters.FirstName}_{customerResourceParameters.LastName}_{customerResourceParameters.Email}_{customerResourceParameters.SearchQuery}";
            _cache.Set(cacheKey, customers, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60)));
        }
    }
}
