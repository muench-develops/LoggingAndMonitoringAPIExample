using System.ComponentModel.DataAnnotations;

namespace LoggingAndMonitoringAPIExample.Models.Customer
{
    public abstract class CustomerBase
    {
        [Required]
        public string FirstName { get; set; }
       
        [Required]
        public string LastName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [Phone]
        public string Phone { get; set; }
    }
}
