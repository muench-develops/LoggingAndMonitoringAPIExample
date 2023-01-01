using Castle.Core.Logging;
using FluentAssertions;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Models;
using LoggingAndMonitoringAPIExample.Logic.Parameters;
using LoggingAndMonitoringAPIExample.Logic.Services;
using LoggingAndMonitoringAPIExample.Tests.Context;
using Microsoft.Extensions.Logging;
using Moq;

namespace LoggingAndMonitoringAPIExample.Tests.Services
{
    public class CustomerServiceTests : IClassFixture<CustomerSeedDataFixture>
    {
        private readonly ICustomerService _customerService;

        public CustomerServiceTests()
        {
            var customerSeedDataFixture = new CustomerSeedDataFixture();
            Mock<Microsoft.Extensions.Logging.ILoggerFactory> loggerFactory = new();
            loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger<ICustomerService>>().Object);
            
            _customerService = new CustomerService(customerSeedDataFixture.DbContext, loggerFactory.Object);
        }
        
        [Fact]
        public async Task GetAllCustomersAsync_WithNoFilter_ReturnsAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync(new CustomerResourceParameters());
            customers.ToList().Count.Should().Be(4);
        }

        [Fact]
        public async Task GetAllCustomersAsync_WithFirstNameJaneFilter_ReturnsOnlyFirstNameJane()
        {
            var customerResourceParameters = new CustomerResourceParameters
            {
                FirstName = "Jane"
            };

            var customers = await _customerService.GetAllCustomersAsync(customerResourceParameters);
            customers.ToList().Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAllCustomersAsync_WithLastNameMoeFilter_ReturnsOnlyLastNameMoe()
        {
            var customerResourceParameters = new CustomerResourceParameters
            {
                LastName = "Moe"
            };

            var customers = await _customerService.GetAllCustomersAsync(customerResourceParameters);
            var result = customers.ToArray();

            result[0].Should().BeEquivalentTo(new CustomerDto
            {
                Id = 3,
                Email = "Max.Doe@example.com",
                FirstName = "Max",
                LastName = "Moe",
                Phone = "0000 1111 1113"
            });
            result[1].Should().BeEquivalentTo(new CustomerDto
            {
                Id = 4,
                Email = "Lisa.Doe@example.com",
                FirstName = "Lisa",
                LastName = "Moe",
                Phone = "0000 1111 1114"
            });
        }

        [Fact]
        public async Task CreateCustomerAsync_WithValidInput_ReturnsCreatedCustomer()
        {
            var customer = new Customer
            {
                Email = "test@test.de",
                FirstName = "test",
                LastName = "test",
                Phone = "0000 1111 2222"
            };

            var result = await _customerService.CreateCustomerAsync(customer);

            result.Should().BeEquivalentTo(new CustomerDto
            {
                Id = 5,
                Email = "test@test.de",
                FirstName = "test",
                LastName = "test",
                Phone = "0000 1111 2222"
            });
        }

        [Fact]
        public async Task GetCustomerAsync_WithCustomerIdOne_ReturnsCustomerOne()
        {
            var customerId = 1;

            var result = await _customerService.GetCustomerAsync(customerId);

            result.Should().BeEquivalentTo(
                new Customer { Id = 1, Email = "Jane.Doe@example.com", FirstName = "Jane", LastName = "Doe", Phone = "0000 1111 1111" }
            );
        }

        [Fact]
        public async Task GetExistsAsync_WithCustomerIdOne_ReturnsTrue()
        {
            //Arrange
            var customerId = 1;
            //Act
            var result = await _customerService.GetExistsAsync(customerId);
            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CreateCustomersAsync_WithFourCustomers_ReturnsAllFourCreatedCustomers()
        {
            IEnumerable<Customer> customers = new List<Customer>
            {
                new Customer { Email = "Jane.Doe@example.com", FirstName = "Jane", LastName = "Doe", Phone = "0000 1111 1111" },
                new Customer {  Email = "Joe.Doe@example.com", FirstName = "Joe", LastName = "Doe", Phone = "0000 1111 1112" },
                new Customer { Email = "Max.Doe@example.com", FirstName = "Max", LastName = "Moe", Phone = "0000 1111 1113" },
                new Customer {  Email = "Lisa.Doe@example.com", FirstName = "Lisa", LastName = "Moe", Phone = "0000 1111 1114" }
            };

            var result = await _customerService.CreateCustomersAsync(customers);

            result.Should().HaveCount(4);
        }

        [Fact]
        public async Task GetCustomersAsync_WithSpecificThreeIds_ReturnsSpecificCustomers()
        {
            var ids = new List<int> { 1, 2, 3 };

            var result = await _customerService.GetCustomersAsync(ids);

            result.Should().HaveCount(3);
        }
    }
}
