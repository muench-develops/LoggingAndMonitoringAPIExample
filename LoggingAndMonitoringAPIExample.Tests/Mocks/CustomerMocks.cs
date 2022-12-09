using LoggingAndMonitoringAPIExample.Logic.Entities;

namespace LoggingAndMonitoringAPIExample.Tests.Mocks
{
    internal static class CustomerMocks
    {
        public static async Task<Customer> GetTestCustomerAsync()
        {
            var customer = new Customer { Id = 1, Email = "Jane.Doe@example.com", FirstName = "Jane", LastName = "Doe", Phone = "0000 1111 1111" };

            return await Task.FromResult(customer);
        }


        public static async Task<IEnumerable<Customer>> GetTestCustomersAsync()
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
