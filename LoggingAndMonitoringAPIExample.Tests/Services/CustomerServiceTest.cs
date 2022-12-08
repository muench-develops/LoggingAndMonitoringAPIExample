using FluentAssertions;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Models;
using LoggingAndMonitoringAPIExample.Logic.Parameters;
using LoggingAndMonitoringAPIExample.Logic.Services;
using LoggingAndMonitoringAPIExample.Tests.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingAndMonitoringAPIExample.Tests.Services
{
    public class CustomerServiceTest : IClassFixture<CustomerSeedDataFixture>
    {
        private readonly ICustomerService _customerService;
        CustomerSeedDataFixture customerSeedDataFixture;

        public CustomerServiceTest(CustomerSeedDataFixture customerSeedDataFixture)
        {
            this.customerSeedDataFixture = customerSeedDataFixture;
            _customerService = new CustomerService(customerSeedDataFixture.CustomerDbContext);
        }

        [Fact]
        public async Task GetAllCustomersAsyncShouldAll()
        {
            var customers = await _customerService.GetAllCustomersAsync(new CustomerResourceParameters());
            customers.ToList().Count.Should().Be(4);
        }

        [Fact]
        public async Task GetAllCustomersAsyncShouldReturnOnlyFirstnameJane()
        {
            var customerResourceParameters = new CustomerResourceParameters
            {
                FirstName = "Jane"
            };

            var customers = await _customerService.GetAllCustomersAsync(customerResourceParameters);
            customers.ToList().Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAllCustomersAsyncShouldReturnOnlyLastNameMoe()
        {
            var customerResourceParameters = new CustomerResourceParameters
            {
                LastName = "Moe"
            };

            var customers = await _customerService.GetAllCustomersAsync(customerResourceParameters);
            customers.ToList().Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllCustomersAsyncShouldReturnOnlyLastNameMoeContacts()
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
        public async Task CreateCustomerAsyncShould()
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
        public async Task GetCustomerAsyncShould()
        {
            var customerId = 1;

            var result = await _customerService.GetCustomerAsync(customerId);

            result.Should().BeEquivalentTo(
                new Customer { Id = 1, Email = "Jane.Doe@example.com", FirstName = "Jane", LastName = "Doe", Phone = "0000 1111 1111" }
            );
        }

        [Fact]
        public async Task CustomerExistsAsyncShould()
        {
            var customerId = 1;
            var result = await _customerService.CustomerExistsAsync(customerId);
            result.Should().Be(true);
        }
    }
}
