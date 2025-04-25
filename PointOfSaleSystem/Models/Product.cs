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
    /// Represents a product in the Point of Sale system.
    /// This class contains properties for product details such as ID, barcode, name, category, supplier, brand, 
    /// quantity, cost price, selling price, and image. It also implements the INotifyPropertyChanged interface 
    /// to support property change notifications and the ICheckable interface for validation.
    /// </summary>
    public class Product : INotifyPropertyChanged, ICheckable
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
        public bool LowStockWarning => Quantity.HasValue && Quantity < 10;

        public event PropertyChangedEventHandler? PropertyChanged;
        
        /// <summary>
        /// Accepts a checker to perform a validation check on this <see cref="Product"/> instance.
        /// </summary>
        /// <param name="checker">The checker object to perform the validation.</param>
        /// <returns>
        /// A string containing the error message if the validation check fails, or
        /// <c>null</c> if the validation check succeeds.
        /// </returns>
        public string? AcceptForChecking(IChecker checker)
        {
            return checker.Check(this);
        }
    }
}
