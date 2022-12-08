using AutoMapper;
using Castle.Core.Resource;
using FluentAssertions;
using LoggingAndMonitoringAPIExample.Controllers;
using LoggingAndMonitoringAPIExample.Logic;
using LoggingAndMonitoringAPIExample.Logic.Context;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Models;
using LoggingAndMonitoringAPIExample.Logic.Parameters;
using LoggingAndMonitoringAPIExample.Logic.Services;
using LoggingAndMonitoringAPIExample.Tests.Mocks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingAndMonitoringAPIExample.Tests.Controller
{
    public class CustomerControllerShould
    {
        private readonly Mock<ICustomerService> _customerService;
        private readonly IMapper _mapper;
        private readonly CustomersController _customerController;

        public CustomerControllerShould()
        {
            _customerService = new Mock<ICustomerService>();

            _customerService.Setup(service => service.GetAllCustomersAsync(It.IsAny<CustomerResourceParameters>())).Returns(CustomerMocks.GetTestCustomersAsync());
            _customerService.Setup(service => service.CreateCustomerAsync(It.IsAny<Customer>())).Returns(CustomerMocks.GetTestCustomerAsync());
            _customerService.Setup(service => service.GetCustomerAsync(It.IsAny<int>())).Returns(CustomerMocks.GetTestCustomerAsync());
            _customerService.Setup(service => service.CustomerExistsAsync(It.IsAny<int>())).Returns(Task.FromResult(true));
                        
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new CustomerMappingProfile());
                });
                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }

            _customerController = new CustomersController(_customerService.Object, _mapper);
        }

        [Fact]
        public async Task GetAllCustomersAsyncShouldReturnAllCustomers()
        {
            var result = await _customerController.GetCustomers(new CustomerResourceParameters());

            //result should be 200
            
            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(await CustomerMocks.GetTestCustomersAsync());
        }

        [Fact]
        public async Task CreateCustomerAsyncShould()
        {
            var customerRequest = new CustomerForCreationDto
            {
                Email = "Jane.Doe@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                Phone = "0000 1111 1111"
            };

            var expected = new Customer
            {
                Id = 1,
                Email = "Jane.Doe@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                Phone = "0000 1111 1111"
            };

            var result = await _customerController.CreateCustomer(customerRequest);

            result.Result.Should().BeOfType<CreatedAtRouteResult>();
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            
            createdAtRouteResult.StatusCode.Should().Be(201);
            var cus = await CustomerMocks.GetTestCustomerAsync();
            createdAtRouteResult.Value.Should().BeEquivalentTo(expected);            
        }

        [Fact]
        public async Task GetCustomerAsync()
        {
            var result = await _customerController.GetCustomer(1);

            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(await CustomerMocks.GetTestCustomerAsync());
        }
        

 
    }
}
