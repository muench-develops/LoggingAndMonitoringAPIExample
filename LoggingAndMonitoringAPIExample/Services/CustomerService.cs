using LoggingAndMonitoringAPIExample.Models.Customer;
using LoggingAndMonitoringAPIExample.Params;
using System.Numerics;

namespace LoggingAndMonitoringAPIExample.Services
{
    public class CustomerService : ICustomerService
    {
        public async Task<List<CustomerResponse>> GetAllCustomersAsync(CustomerResourceParameters customerResourceParameters)
        {

            var result = GetAllCustomers();

            if (!string.IsNullOrEmpty(customerResourceParameters.FirstName))
            {
                result = result.Where(x => x.FirstName.ToLower().Contains(customerResourceParameters.FirstName.ToLower())).ToList();
            }
            
            if(!string.IsNullOrEmpty(customerResourceParameters.LastName))
            {
                result = result.Where(x => x.LastName.ToLower().Contains(customerResourceParameters.LastName.ToLower())).ToList();
            }
            
            if (!string.IsNullOrEmpty(customerResourceParameters.Email))
            {
                result = result.Where(x => x.Email.ToLower().Contains(customerResourceParameters.Email.ToLower())).ToList();
            }
            
            if (!string.IsNullOrEmpty(customerResourceParameters.SearchQuery))
            {
                result = result.Where(x => x.FirstName.ToLower().Contains(customerResourceParameters.SearchQuery.ToLower()) ||
                                           x.LastName.ToLower().Contains(customerResourceParameters.SearchQuery.ToLower()) ||
                                           x.Email.ToLower().Contains(customerResourceParameters.SearchQuery.ToLower())).ToList();
            }

            return await Task.FromResult(result.Skip(customerResourceParameters.PageSize * (customerResourceParameters.PageNumber - 1))
                .Take(customerResourceParameters.PageSize)
                .ToList());

        }

        private static List<CustomerResponse> GetAllCustomers()
        {
            return new List<CustomerResponse>
            { new CustomerResponse
                {
                    Id = 1,
                    Email = "John.Doe@gmail.com",
                    FirstName = "John",
                    LastName = "Doe",
                    Phone = "0111 1111 1111"
                },   new CustomerResponse
                {
                    Id = 2,
                    Email = "Jane.Doe@gmail.com",
                    FirstName = "Jane",
                    LastName = "Doe",
                    Phone = "0111 1111 1111"
                 }
            };
        }
    }
}
