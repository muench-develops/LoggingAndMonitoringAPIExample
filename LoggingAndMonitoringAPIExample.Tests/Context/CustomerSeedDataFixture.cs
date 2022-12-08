using LoggingAndMonitoringAPIExample.Logic.Context;
using LoggingAndMonitoringAPIExample.Logic.Models.Customer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingAndMonitoringAPIExample.Tests.Context
{
    public class CustomerSeedDataFixture : IDisposable
    {
        public CustomerDbContext CustomerDbContext { get; private set; }

        public CustomerSeedDataFixture()
        {
            var options = new DbContextOptionsBuilder<CustomerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            CustomerDbContext = new CustomerDbContext(options);
            
            CustomerDbContext.Customers.Add(new Customer { Id = 1, Email = "Jane.Doe@example.com", FirstName = "Jane", LastName = "Doe", Phone = "0000 1111 1111" });
            CustomerDbContext.Customers.Add(new Customer { Id = 2, Email = "Joe.Doe@example.com", FirstName = "Joe", LastName = "Doe", Phone = "0000 1111 1112" });
            CustomerDbContext.Customers.Add(new Customer { Id = 3, Email = "Max.Doe@example.com", FirstName = "Max", LastName = "Moe", Phone = "0000 1111 1113" });
            CustomerDbContext.Customers.Add(new Customer { Id = 4, Email = "Lisa.Doe@example.com", FirstName = "Lisa", LastName = "Moe", Phone = "0000 1111 1114" });
            CustomerDbContext.SaveChanges();
        }


        public void Dispose() => CustomerDbContext.Dispose();
    }
}
