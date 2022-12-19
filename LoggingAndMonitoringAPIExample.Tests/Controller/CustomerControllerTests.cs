using AutoMapper;
using FluentAssertions;
using LoggingAndMonitoringAPIExample.Controllers;
using LoggingAndMonitoringAPIExample.Handler;
using LoggingAndMonitoringAPIExample.Logic;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Models;
using LoggingAndMonitoringAPIExample.Logic.Parameters;
using LoggingAndMonitoringAPIExample.Logic.Services;
using LoggingAndMonitoringAPIExample.Tests.Mocks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace LoggingAndMonitoringAPIExample.Tests.Controller
{
    public class CustomerControllerTests
    {
        private readonly CustomerController _customerController;

        public CustomerControllerTests()
        {
            // Create a mock object that implements the CustomerControllerDependencyHandler interface
            var mockDependencyHandler = new Mock<CustomerDependencyHandler>();

            // Create a mock object that implements the ICustomerService interface
            var mockCustomerService = ServiceMocks.SetupCustomerService();

            // Set up the mock object to return the desired value when the GetAllCustomersAsync method is called

            mockDependencyHandler
                .Setup(x => x.GetCustomerService())
                .Returns(mockCustomerService.Object);

            var mapper = ServiceMocks.SetupMapper();

            mockDependencyHandler
                .Setup(x => x.GetMapper())
                .Returns(mapper);

            Mock<ILoggerFactory> mockLoggerFactory = ServiceMocks.SetupMockLoggerFactory();

            mockDependencyHandler
                .Setup(x => x.GetLoggerFactory())
                .Returns(mockLoggerFactory.Object);

            // Inject the mock object into the CustomerController constructor
            _customerController = new CustomerController(mockDependencyHandler.Object);
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
