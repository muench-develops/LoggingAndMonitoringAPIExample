using AutoMapper;
using FluentAssertions;
using LoggingAndMonitoringAPIExample.Controllers;
using LoggingAndMonitoringAPIExample.Logic;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using LoggingAndMonitoringAPIExample.Logic.Models;
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
    public class CustomerCollectionsControllerShould
    {
        private readonly Mock<ICustomerService> _customerService;
        private readonly IMapper _mapper;
        private readonly CustomerCollectionsController _customerCollectionController;
        public CustomerCollectionsControllerShould()
        {
            _customerService = new Mock<ICustomerService>();
            _customerService.Setup(service => service.CreateCustomersAsync(It.IsAny<IEnumerable<Customer>>())).Returns(CustomerMocks.GetTestCustomersAsync());
            
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new CustomerMappingProfile());
                });
                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }
            
            _customerCollectionController = new CustomerCollectionsController(_customerService.Object, _mapper);
        }

        [Fact]
        public async Task CreateCustomerCollectionShould()
        {
            var entities =_mapper.Map<List<CustomerForCreationDto>>(await CustomerMocks.GetTestCustomersAsync());
            var result = await _customerCollectionController.CreateCustomerCollection(entities);
            result.Result.Should().BeOfType<CreatedAtRouteResult>();
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;

            createdAtRouteResult.StatusCode.Should().Be(201);
            createdAtRouteResult.Value.Should().BeEquivalentTo(await CustomerMocks.GetTestCustomersAsync());
        }
    }
}
