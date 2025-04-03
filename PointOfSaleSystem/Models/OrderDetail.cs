using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSaleSystem.Models
{
    public class OrderDetail
    {
        public int? OrderId { get; set; } = null;
        public int? ProductId { get; set; } = null;
        public int? Quantity { get; set; }
    }
}
