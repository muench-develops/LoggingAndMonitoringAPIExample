using AutoMapper;
using Castle.Core.Resource;
using FluentAssertions;
using LoggingAndMonitoringAPIExample.Controllers;
using LoggingAndMonitoringAPIExample.Logic;
using LoggingAndMonitoringAPIExample.Logic.Context;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Models;
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
        private readonly IMapper _mapper;
        private readonly CustomerController _customerController;

        public CustomerControllerShould()
        {
            _customerService = new Mock<ICustomerService>();

            _customerService.Setup(service => service.GetAllCustomersAsync(It.IsAny<CustomerResourceParameters>())).Returns(GetTestCustomersAsync());
            _customerService.Setup(service => service.CreateCustomerAsync(It.IsAny<Customer>())).Returns(GetTestCustomerAsync());

            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new SourceMappingProfile());
                });
                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }

            _customerController = new CustomerController(_customerService.Object, _mapper);
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
            okResult.Value.Should().BeEquivalentTo(await GetTestCustomersAsync());
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

           
            var result = await _customerController.CreateCustomerAsync(customerRequest);

            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(await GetTestCustomerAsync());
        }

        private async Task<Customer> GetTestCustomerAsync()
        {
            var customer = new Customer { Id = 1, Email = "Jane.Doe@example.com", FirstName = "Jane", LastName = "Doe", Phone = "0000 1111 1111" };

            return await Task.FromResult(customer);
        }


        private async Task<List<Customer>> GetTestCustomersAsync()
        {
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Email = "Jane.Doe@example.com", FirstName = "Jane", LastName = "Doe", Phone = "0000 1111 1111" },
                new Customer { Id = 2, Email = "Joe.Doe@example.com", FirstName = "Joe", LastName = "Doe", Phone = "0000 1111 1112" },
                new Customer { Id = 3, Email = "Max.Doe@example.com", FirstName = "Max", LastName = "Moe", Phone = "0000 1111 1113" },
                new Customer { Id = 4, Email = "Lisa.Doe@example.com", FirstName = "Lisa", LastName = "Moe", Phone = "0000 1111 1114" }
            };
            return await Task.FromResult(customers);
        }
    }
}
