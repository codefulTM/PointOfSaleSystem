﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;

namespace PointOfSaleSystem.Models
{
    /// <summary>
    /// Represents a payment method used in the Point of Sale system.
    /// </summary>
    public class PaymentMethod : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Type { get; set; } 
        public string? AccountNumber { get; set; } = null;
        public string? BankName { get; set; } = null;
        public string? AccountHolder { get; set; } = null;
        public string? PhoneNumber { get; set; } = null;
        public bool IsDefault { get; set; } = false;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
