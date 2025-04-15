using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Utils.Checkers;

namespace PointOfSaleSystem.Models
{
    public class Order : INotifyPropertyChanged, ICheckable
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? TotalPrice { get; set; }
        public int? Discount { get; set; }
        public bool? IsPaid { get; set; }
        public DateTime? OrderTime { get; set; }

        public List<int> ProductIds { get; set; } = new List<int>();

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
