using AutoMapper;
using LoggingAndMonitoringAPIExample.Controllers;
using LoggingAndMonitoringAPIExample.Handler;
using LoggingAndMonitoringAPIExample.Logic;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Parameters;
using LoggingAndMonitoringAPIExample.Logic.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingAndMonitoringAPIExample.Tests.Mocks
{
    public static class ServiceMocks
    {

        private static readonly int[] customerIds = new int[] { 1, 2, 4 };

        public static IMapper SetupMapper()
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
        public static Mock<ILoggerFactory> SetupMockLoggerFactory()
        {
            // Create a mock object that implements the ILoggerFactory interface
            var mockLoggerFactory = new Mock<ILoggerFactory>();

            // Set up the mock object to return a mock logger when the CreateLogger method is called
            mockLoggerFactory
                .Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(Mock.Of<ILogger>);
            return mockLoggerFactory;
        }

        public static Mock<ICustomerService> SetupCustomerService()
        {
            Mock<ICustomerService> customerService = new();

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

            customerService.Setup(service => service.CreateCustomersAsync(It.IsAny<IEnumerable<Customer>>())).Returns(CustomerMocks.GetTestCustomersAsync());
            customerService
                .Setup(service => service.GetCustomersAsync(It.IsAny<IEnumerable<int>>()))
                .Returns(async () =>
                {
                    var customers = await CustomerMocks.GetTestCustomersAsync();
                    return customers.Where(customer => customerIds.Contains(customer.Id));
                });

            customerService
                .Setup(service => service.GetAllCustomersAsync(It.IsAny<CustomerResourceParameters>()))
                .Returns(CustomerMocks.GetTestCustomersAsync());

            return customerService;
        }


        public static Mock<CustomerDependencyHandler> SetUpMemoryCache(Mock<CustomerDependencyHandler> mockDependencyHandler)
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();

            var memoryCache = serviceProvider.GetService<IMemoryCache>();

            mockDependencyHandler
                .Setup(x => x.GetCache())
                .Returns(memoryCache);

            return mockDependencyHandler;
        }
    }
}
