using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Models;

namespace PointOfSaleSystem.Utils.Checkers
{
    /// <summary>
    /// The <c>ValueChecker</c> class provides methods to validate various entities such as 
    /// <c>Customer</c>, <c>Category</c>, <c>Product</c>, and <c>Order</c>. 
    /// It implements the <c>IChecker</c> interface and ensures that the entities meet 
    /// specific business rules and constraints.
    /// </summary>
    public class ValueChecker : IChecker
    {
        /// <summary>
        /// Checks a customer for any value-related issues.
        /// </summary>
        /// <param name="customer">The customer to check.</param>
        /// <returns>
        /// A string containing the error message if the check fails, or null if the check succeeds.
        /// </returns>
        public string? Check(Customer customer)
        {
            if(customer.Gender is not null && !customer.Gender.Equals("Nam") && !customer.Gender.Equals("Nữ"))
            {   
                return "Giới tính phải là nam hoặc nữ";
            }
            return null;
        }

        /// <summary>
        /// Checks a category for any value-related issues.
        /// </summary>
        /// <param name="category">The category to check.</param>
        /// <returns>
        /// A string containing the error message if the check fails, or null if the check succeeds.
        /// </returns>
        public string? Check(Category category)
        {
            return null;
        }

        /// <summary>
        /// Checks a product for any value-related issues.
        /// </summary>
        /// <param name="product">The product to check.</param>
        /// <returns>
        /// A string containing the error message if the check fails, or null if the check succeeds.
        /// </returns>
        public string? Check(Product product)
        {
            if(product.Quantity is not null && product.Quantity < 0)
            {
                return "Số lượng sản phẩm không thể nhỏ hơn 0";
            }
            if (product.CostPrice is not null && product.CostPrice < 0)
            {
                return "Giá nhập sản phẩm không thể nhỏ hơn 0";
            }
            if (product.SellingPrice is not null && product.SellingPrice < 0)
            {
                return "Giá bán sản phẩm không thể nhỏ hơn 0";
            }
            return null;
        }

        /// <summary>
        /// Checks an order for any value-related issues.
        /// </summary>
        /// <param name="order">The order to check.</param>
        /// <returns>
        /// A string containing the error message if the check fails, or null if the check succeeds.
        /// </returns>
        public string? Check(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
