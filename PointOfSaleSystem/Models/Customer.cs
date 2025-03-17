using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSaleSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Phone { get; set; } = null;
        public string? Address { get; set; } = null;
        public DateOnly? DateOfBirth { get; set; } = null;
        public string? Gender { get; set; } = null;
    }
}

