using LoggingAndMonitoringAPIExample.Logic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoggingAndMonitoringAPIExample.WebApp.Pages
{
    public class CustomerListingModel : PageModel
    {
        private readonly HttpClient _apiClient;
        private readonly ILogger<CustomerListingModel> _logger;
        public IEnumerable<CustomerDto> Customers { get; set; }
        public CustomerListingModel(HttpClient apiClient, ILoggerFactory loggerFactory)
        {
            _apiClient = apiClient;
            _apiClient.BaseAddress = new Uri("https://localhost:7081/");
            _logger = loggerFactory.CreateLogger<CustomerListingModel>();
        }

        public async Task OnGetAsync()
        {
            var response = await _apiClient.GetAsync("api/CustomerCollections/error/customers");

            if (!response.IsSuccessStatusCode)
            {
                var details = await response.Content.ReadFromJsonAsync<ProblemDetails>() ?? new ProblemDetails();

                _logger.LogWarning("API failure: {adress} Response {response}, Trace {trace}",
                    $"{_apiClient.BaseAddress}api/CustomerCollections", response.StatusCode, details.Extensions["traceId"]?.ToString());
                throw new Exception("Api call failed");
            }

            Customers = await response.Content.ReadFromJsonAsync<List<CustomerDto>>() ?? new List<CustomerDto>();
        }
    }
}
