using System.ComponentModel.DataAnnotations;

namespace LoggingAndMonitoringAPIExample.Logic.Parameters
{
    public class CustomerResourceParameters
    {
        const int maxPageSize = 20;
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public string? SearchQuery { get; set; }

        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

    }
}
