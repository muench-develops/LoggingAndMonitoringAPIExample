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
    public class CustomerControllerShould
    {
        private readonly CustomerController _customerController;

        public CustomerControllerShould()
        {
            // Create a mock object that implements the CustomerControllerDependencyHandler interface
            var mockDependencyHandler = new Mock<CustomerControllerDependencyHandler>();

            // Set up the mock object to return the desired values for the dependencies
            SetUpMemoryCache(mockDependencyHandler);

            // Create a mock object that implements the ICustomerService interface
            var mockCustomerService = SetupCustomerService();

            // Set up the mock object to return the desired value when the GetAllCustomersAsync method is called

            mockDependencyHandler
                .Setup(x => x.GetCustomerService())
                .Returns(mockCustomerService.Object);

            var mapper = SetupMapper();

            mockDependencyHandler
                .Setup(x => x.GetMapper())
                .Returns(mapper);

            Mock<ILoggerFactory> mockLoggerFactory = SetupMockLoggerFactory();

            mockDependencyHandler
                .Setup(x => x.GetLoggerFactory())
                .Returns(mockLoggerFactory.Object);

            // Inject the mock object into the CustomerController constructor
            _customerController = new CustomerController(mockDependencyHandler.Object);
        }

        private static IMapper SetupMapper()
        {
            IMapper iMapper = null;
            if (iMapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new CustomerMappingProfile());
                });
                var mapper = mappingConfig.CreateMapper();
                iMapper = mapper;
            }
            return iMapper;
        }

        private static void SetUpMemoryCache(Mock<CustomerControllerDependencyHandler> mockDependencyHandler)
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();

            var memoryCache = serviceProvider.GetService<IMemoryCache>();

            mockDependencyHandler
                .Setup(x => x.GetCache())
                .Returns(memoryCache);

        }

        private static Mock<ILoggerFactory> SetupMockLoggerFactory()
        {
            // Create a mock object that implements the ILoggerFactory interface
            var mockLoggerFactory = new Mock<ILoggerFactory>();

            // Set up the mock object to return a mock logger when the CreateLogger method is called
            mockLoggerFactory
                .Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(Mock.Of<ILogger<CustomerController>>());
            return mockLoggerFactory;
        }

        private static Mock<ICustomerService> SetupCustomerService()
        {
            Mock<ICustomerService> customerService = new();


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

            return customerService;
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
