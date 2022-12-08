﻿using System.ComponentModel.DataAnnotations;

namespace LoggingAndMonitoringAPIExample.Logic.Models.Customer
{
    public class CustomerResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}