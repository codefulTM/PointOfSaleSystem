﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSaleSystem.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Barcode { get; set; } = null;
        public string Name { get; set; }
        public string? Category { get; set; } = null;
        public string? Supplier { get; set; } = null;
        public string? Brand { get; set; } = null;
        public int? Quantity { get; set; } = null;
        public int? CostPrice { get; set; } = null;
        public int? SellingPrice { get; set; } = null;
        public string? Image { get; set; } = null;
    }
}
