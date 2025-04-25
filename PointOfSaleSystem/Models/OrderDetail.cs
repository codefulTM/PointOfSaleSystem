using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSaleSystem.Models
{
    /// <summary>
    /// Represents the details of an order, including the associated order ID, product ID, and quantity.
    /// </summary>
    public class OrderDetail
    {
        public int? OrderId { get; set; } = null;
        public int? ProductId { get; set; } = null;
        public int? Quantity { get; set; }
    }
}
