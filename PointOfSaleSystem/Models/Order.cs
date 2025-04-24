using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Utils.Checkers;

namespace PointOfSaleSystem.Models
{
    /// <summary>
    /// Represents an order in the Point of Sale system.
    /// This class contains details about the order, such as its ID, customer ID, total price, discount, 
    /// payment status, and the time the order was placed. It also implements interfaces for property 
    /// change notification and custom checking logic.
    /// </summary>
    public class Order : INotifyPropertyChanged, ICheckable
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? TotalPrice { get; set; }
        public int? Discount { get; set; }
        public bool? IsPaid { get; set; }
        public DateTime? OrderTime { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? AcceptForChecking(IChecker checker)
        {
            return checker.Check(this);
        }

        public bool Check()
        {
            return true;
        }
    }
}
