using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;

namespace PointOfSaleSystem.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Type { get; set; } 
        public string? AccountNumber { get; set; } = null;
        public string? BankName { get; set; } = null;
        public string? AccountHolder { get; set; } = null;
        public string? PhoneNumber { get; set; } = null;
        public bool IsDefault { get; set; } = false;
    }
}
