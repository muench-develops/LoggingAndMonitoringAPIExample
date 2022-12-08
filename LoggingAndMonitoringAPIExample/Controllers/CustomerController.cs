using LoggingAndMonitoringAPIExample.Logic.Params;
using Microsoft.AspNetCore.Mvc;
using LoggingAndMonitoringAPIExample.Logic.Services;
using LoggingAndMonitoringAPIExample.Logic.Models;
using AutoMapper;
using LoggingAndMonitoringAPIExample.Logic.Entities;

namespace LoggingAndMonitoringAPIExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CustomerController(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }

        [HttpGet("customers")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        
        public async Task<ActionResult<List<CustomerDto>>> GetAllCustomerAsync([FromQuery] CustomerResourceParameters customerResourceParameters)
        {
            var customers = await _customerService.GetAllCustomersAsync(customerResourceParameters);

            return customers.Any() ? Ok(_mapper.Map<List<CustomerDto>>(customers)) : NoContent();
        }

        [HttpPost("customers")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CustomerDto>> CreateCustomerAsync([FromBody] CustomerForCreationDto customerRequest)
        {
            var customer = await _customerService.CreateCustomerAsync(_mapper.Map<Customer>(customerRequest));
            return customer is not null ? Ok(_mapper.Map<CustomerDto>(customer)) : BadRequest();
        }
    }
}
