using FluentAssertions;
using LoggingAndMonitoringAPIExample.Controllers;
using LoggingAndMonitoringAPIExample.Logic.Context;
using LoggingAndMonitoringAPIExample.Logic.Models.Customer;
using LoggingAndMonitoringAPIExample.Logic.Params;
using LoggingAndMonitoringAPIExample.Logic.Services;
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
        private readonly CustomerController _customerController;

        public CustomerControllerShould()
        {
            _customerService = new Mock<ICustomerService>();

            _customerService.Setup(service => service.GetAllCustomersAsync(It.IsAny<CustomerResourceParameters>())).Returns((GetTestCustomerAsync()));

            _customerController = new CustomerController(_customerService.Object);
        }

        [Fact]
        public async Task GetAllCustomersAsyncShouldReturnAllCustomers()
        {
            var result = await _customerController.GetAllCustomerAsync(new CustomerResourceParameters());

            //result should be 200
            
            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(await GetTestCustomerAsync());
        }

        [Fact]
        public async Task CreateCustomerAsyncShould()
        {
            var customerRequest = new CustomerRequest
            {
                Email = "Jane.Doe@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                Phone = "0000 1111 1111"
            };

            var customerResponse = new CustomerResponse
            {
                Email = "Jane.Doe@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                Phone = "0000 1111 1111"
            };
            
            var result = await _customerController.CreateCustomerAsync(customerRequest);

            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(customerResponse);
        }


        private async Task<List<CustomerResponse>> GetTestCustomerAsync()
        {
            var customers = new List<CustomerResponse>
            {
                new CustomerResponse { Id = 1, Email = "Jane.Doe@example.com", FirstName = "Jane", LastName = "Doe", Phone = "0000 1111 1111" },
                new CustomerResponse { Id = 2, Email = "Joe.Doe@example.com", FirstName = "Joe", LastName = "Doe", Phone = "0000 1111 1112" },
                new CustomerResponse { Id = 3, Email = "Max.Doe@example.com", FirstName = "Max", LastName = "Moe", Phone = "0000 1111 1113" },
                new CustomerResponse { Id = 4, Email = "Lisa.Doe@example.com", FirstName = "Lisa", LastName = "Moe", Phone = "0000 1111 1114" }
            };
            return await Task.FromResult(customers);
        }
    }
}
