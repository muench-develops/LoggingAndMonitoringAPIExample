using LoggingAndMonitoringAPIExample.Logic.Context;
using LoggingAndMonitoringAPIExample.Logic.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoggingAndMonitoringAPIExample.Tests.Context
{
    public class CustomerSeedDataFixture : IDisposable
    {
        public CustomerDbContext DbContext { get; private set; }

        public CustomerSeedDataFixture()
        {
            var options = new DbContextOptionsBuilder<CustomerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            DbContext = new CustomerDbContext(options);
            
            DbContext.Customers.Add(new Customer { Id = 1, Email = "Jane.Doe@example.com", FirstName = "Jane", LastName = "Doe", Phone = "0000 1111 1111" });
            DbContext.Customers.Add(new Customer { Id = 2, Email = "Joe.Doe@example.com", FirstName = "Joe", LastName = "Doe", Phone = "0000 1111 1112" });
            DbContext.Customers.Add(new Customer { Id = 3, Email = "Max.Doe@example.com", FirstName = "Max", LastName = "Moe", Phone = "0000 1111 1113" });
            DbContext.Customers.Add(new Customer { Id = 4, Email = "Lisa.Doe@example.com", FirstName = "Lisa", LastName = "Moe", Phone = "0000 1111 1114" });
            DbContext.SaveChanges();
        }


        public void Dispose() => DbContext.Dispose();
    }
}
