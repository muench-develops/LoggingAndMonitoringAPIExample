using LoggingAndMonitoringAPIExample.Logic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoggingAndMonitoringAPIExample.WebApp.Pages
{
    public class CustomerListingModel : PageModel
    {
        private readonly HttpClient _apiClient;
        public IEnumerable<CustomerDto> Customers { get; set; }
        public CustomerListingModel(HttpClient apiClient)
        {
            _apiClient = apiClient;
            _apiClient.BaseAddress = new Uri("https://localhost:7081/");
        }

        public async Task OnGetAsync()
        {
            var response = await _apiClient.GetAsync("api/CustomerCollections");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Api call failed");
            }

            Customers = await response.Content.ReadFromJsonAsync<List<CustomerDto>>() ?? new List<CustomerDto>();
        }
    }
}
