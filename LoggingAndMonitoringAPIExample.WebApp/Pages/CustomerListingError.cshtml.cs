using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoggingAndMonitoringAPIExample.WebApp.Pages
{
    public class CustomerListingErrorModel : PageModel
    {
        private readonly HttpClient _apiClient;
        public CustomerListingErrorModel(HttpClient apiClient)
        {
            _apiClient = apiClient;
            _apiClient.BaseAddress = new Uri("https://localhost:7081/");
        }

        public void OnGet()
        {
            throw new NotImplementedException("This is a test exception");
        }
    }
}
