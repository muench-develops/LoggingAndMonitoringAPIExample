namespace LoggingAndMonitoringAPIExample.Params
{
    public class CustomerResourceParameters
    {
        const int maxPageSize = 20;
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? SearchQuery { get; set; }

        public int PageNumber { get; set; } = 1;
        private int _pageSize  = 10;
        
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
        
    }
}
