using LoggingAndMonitoringAPIExample.Logic.Parameters;
using Microsoft.AspNetCore.Mvc;
using LoggingAndMonitoringAPIExample.Logic.Services;
using LoggingAndMonitoringAPIExample.Logic.Models;
using AutoMapper;
using LoggingAndMonitoringAPIExample.Logic.Entities;

namespace LoggingAndMonitoringAPIExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerSController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CustomerSController(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [HttpHead]
        [Produces("application/json", "application/xml", Type = typeof(IEnumerable<CustomerDto>))]

        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers([FromQuery] CustomerResourceParameters customerResourceParameters)
        {
            var customers = await _customerService.GetAllCustomersAsync(customerResourceParameters);

            return Ok(_mapper.Map<IEnumerable<CustomerDto>>(customers));
        }

        [HttpGet("{customerId}", Name = "GetCustomer")]
        [Produces("application/json", "application/xml", Type = typeof(CustomerDto))]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int customerId)
        {
            var customer = await _customerService.GetCustomerAsync(customerId);

            if (!await _customerService.GetExistsAsync(customerId))
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CustomerDto>(customer));
        }

        [HttpPost]
        [Produces("application/json", "application/xml", Type = typeof(CustomerDto))]
        public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CustomerForCreationDto customerRequest)
        {
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
