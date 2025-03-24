using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Utils.Checkers;

namespace PointOfSaleSystem.Models
{
    public class Customer : INotifyPropertyChanged, ICheckable
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Gender { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? acceptForChecking(IChecker checker)
        {
            return checker.Check(this);
        }

        public bool Check()
        {
            return true;
        }
    }
}
