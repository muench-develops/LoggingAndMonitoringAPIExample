﻿using AutoMapper;
using FluentAssertions;
using LoggingAndMonitoringAPIExample.Controllers;
using LoggingAndMonitoringAPIExample.Logic;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Models;
using LoggingAndMonitoringAPIExample.Logic.Parameters;
using LoggingAndMonitoringAPIExample.Logic.Services;
using LoggingAndMonitoringAPIExample.Tests.Mocks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace LoggingAndMonitoringAPIExample.Tests.Controller
{
    public class CustomerControllerShould
    {
        private readonly IMapper _mapper;
        private readonly CustomerController _customerController;
        //Mock loggerFactory
        public CustomerControllerShould()
        {
            Mock<ICustomerService> mockCustomerService = new();
            Mock<ILoggerFactory> mockLoggerFactory = new();
            Mock<IMemoryCache> mockCache = new();
      
            //Setup Get MemoryCache
mockCache.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);
            mockCache.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MemoryCacheEntryOptions>()));
            
            // Setup loggerFactory
            mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger<CustomerController>>().Object);
            ServiceSetup(mockCustomerService);

            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new CustomerMappingProfile());
                });
                var mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }

            _customerController = new CustomerController(mockCustomerService.Object, _mapper, mockLoggerFactory.Object, mockCache.Object);
        }

        private static void ServiceSetup(Mock<ICustomerService> customerService)
        {
            customerService
                .Setup(service => service.GetAllCustomersAsync(It.IsAny<CustomerResourceParameters>()))
                .Returns(CustomerMocks.GetTestCustomersAsync());

            customerService
                .Setup(service => service.CreateCustomerAsync(It.IsAny<Customer>()))
                .Returns(CustomerMocks.GetTestCustomerAsync());

            customerService
                .Setup(service => service.GetCustomerAsync(It.IsAny<int>()))
                .Returns(CustomerMocks.GetTestCustomerAsync());

            customerService
                .Setup(service => service.GetCustomerAsync(It.Is<int>(id => id == 0)))
                .Returns(Task.FromResult<Customer>(null));

            customerService
                .Setup(service => service.GetExistsAsync(It.Is<int>(id => id == 0)))
                .Returns(Task.FromResult(false));

            customerService
                .Setup(service => service.GetExistsAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(true));
        }


        [Fact]
        public async Task GetAllCustomersAsyncShouldReturnAllCustomers()
        {
            var result = await _customerController.GetCustomers(new CustomerResourceParameters());

            //result should be 200
            
            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().BeEquivalentTo(await CustomerMocks.GetTestCustomersAsync());
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
            
            createdAtRouteResult?.StatusCode.Should().Be(201);
            createdAtRouteResult?.Value.Should().BeEquivalentTo(expected);            
        }

        [Fact]
        public async Task GetCustomerAsync()
        {
            var result = await _customerController.GetCustomer(1);

            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().BeEquivalentTo(await CustomerMocks.GetTestCustomerAsync());
        }

        [Fact]

        public async Task GetCustomerShouldFail()
        {
            var result = await _customerController.GetCustomer(0);

            result.Result.Should().BeOfType<NotFoundResult>();
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult?.StatusCode.Should().Be(404);
        }
    }
}
