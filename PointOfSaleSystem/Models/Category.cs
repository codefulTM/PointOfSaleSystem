using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSaleSystem.Models
{
    
    /// <summary>
    /// Represents a category in the Point of Sale system.
    /// </summary>
    public class Category
    {
        public int? Id { get; set; } = null;
        public string Name { get; set; } = string.Empty;
    }
}
