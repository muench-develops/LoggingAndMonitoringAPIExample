using LoggingAndMonitoringAPIExample.Logic.Models.Customer;
using Microsoft.EntityFrameworkCore;

namespace LoggingAndMonitoringAPIExample.Logic.Context
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext()
        {
        }

        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
