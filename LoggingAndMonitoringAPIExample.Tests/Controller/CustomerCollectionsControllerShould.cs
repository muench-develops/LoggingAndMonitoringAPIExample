using AutoMapper;
using FluentAssertions;
using LoggingAndMonitoringAPIExample.Controllers;
using LoggingAndMonitoringAPIExample.Logic;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Models;
using LoggingAndMonitoringAPIExample.Logic.Services;
using LoggingAndMonitoringAPIExample.Tests.Mocks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LoggingAndMonitoringAPIExample.Tests.Controller
{
    public class CustomerCollectionsControllerShould
    {
        private readonly IMapper _mapper;
        private readonly CustomerCollectionsController _customerCollectionController;
        private static readonly int[] customerIds = new int[] { 1, 2, 4 };

        public CustomerCollectionsControllerShould()
        {
            Mock<ICustomerService> customerService = new();
            Mock<ILoggerFactory> loggerFactory = new();
            // Setup loggerFactory
            loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger<CustomerCollectionsController>>().Object);
            customerService.Setup(service => service.CreateCustomersAsync(It.IsAny<IEnumerable<Customer>>())).Returns(CustomerMocks.GetTestCustomersAsync());
            customerService
                .Setup(service => service.GetCustomersAsync(It.IsAny<IEnumerable<int>>()))
                .Returns(async () =>
                {
                    var customers = await CustomerMocks.GetTestCustomersAsync();
                    return customers.Where(customer => customerIds.Contains(customer.Id));
                });
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new CustomerMappingProfile());
                });
                var mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }

            _customerCollectionController = new CustomerCollectionsController(customerService.Object, _mapper, loggerFactory.Object);
        }

        [Fact]
        public async Task CreateCustomerCollectionShould()
        {
            var entities = _mapper.Map<List<CustomerForCreationDto>>(await CustomerMocks.GetTestCustomersAsync());
            var result = await _customerCollectionController.CreateCustomerCollection(entities);
            result.Result.Should().BeOfType<CreatedAtRouteResult>();
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;

            createdAtRouteResult?.StatusCode.Should().Be(201);
            createdAtRouteResult?.Value.Should().BeEquivalentTo(await CustomerMocks.GetTestCustomersAsync());
        }

        [Fact]
        public async Task GetCustomersShould()
        {
            var result = await _customerCollectionController.GetCustomers(customerIds);
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult?.StatusCode.Should().Be(200);

            var expected = CustomerMocks.GetTestCustomersAsync().Result.Where(x => x.Id == 1 || x.Id == 2 || x.Id == 4).ToList();
            okResult?.Value.Should().BeEquivalentTo(expected);
        }
    }
}

