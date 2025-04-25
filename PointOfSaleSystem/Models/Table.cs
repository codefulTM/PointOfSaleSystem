using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Utils.Checkers;

namespace PointOfSaleSystem.Models
{
    public class Table : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string? Name { get; set; }
        public string? State { get; set; }
        public DateTime? BookTime { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
