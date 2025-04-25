using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Models;

namespace PointOfSaleSystem.Utils.Checkers
{
    /// <summary>
    /// The <c>RequiredFieldChecker</c> class is responsible for validating 
    /// that required fields in various entities are not null or empty.
    /// Implements the <c>IChecker</c> interface to provide specific 
    /// validation logic for different entity types.
    /// </summary>
    public class RequiredFieldChecker : IChecker
    {
        /// <summary>
        /// Checks a customer for any required field-related issues.
        /// </summary>
        /// <param name="customer">The customer to check.</param>
        /// <returns>
        /// A string containing the error message if the check fails, or null if the check succeeds.
        /// </returns>
        public string? Check(Customer customer)
        {
            if(string.IsNullOrWhiteSpace(customer.Name))
            {
                return "Tên không thể là rỗng";
            }
            return null;
        }

        /// <summary>
        /// Checks a category for any required field-related issues.
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
        /// Checks a product for any required field-related issues.
        /// </summary>
        /// <param name="product">The product to check.</param>
        /// <returns>
        /// A string containing the error message if the check fails, or null if the check succeeds.
        /// </returns>
        public string? Check(Product product)
        {
            if(string.IsNullOrWhiteSpace(product.Name))
            {
                return "Tên sản phẩm không được để trống";
            }
            return null;
        }

        /// <summary>
        /// Checks an order for any required field-related issues.
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
