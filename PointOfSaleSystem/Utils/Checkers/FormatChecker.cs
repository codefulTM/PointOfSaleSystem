using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PointOfSaleSystem.Models;

namespace PointOfSaleSystem.Utils.Checkers
{
    /// <summary>
    /// The FormatChecker class is responsible for validating the format of various entities
    /// such as Customer, Product, and others. It implements the IChecker interface and provides
    /// methods to check for specific format-related issues, such as invalid characters in names,
    /// phone numbers, or file paths.
    /// </summary>
    public class FormatChecker : IChecker
    {
        public string Pattern { get; set; }

        /// <summary>
        /// Checks a customer for any format-related issues.
        /// </summary>
        /// <param name="customer">The customer to check.</param>
        /// <returns>
        /// A string containing the error message if the check fails, or null if the check succeeds.
        /// </returns>
        
        public string? Check(Customer customer)
        {
            // If there is a number in the customer's name -> generate error message
            Pattern = @"\d+";
            bool checkResult = Regex.IsMatch(customer.Name, Pattern);
            if(checkResult)
            {
                return "Tên không thể chứa số";
            }

            // If there is a letter in the customer's phone number -> generate error message
            Pattern = @"[a-zA-Z]+";
            checkResult = Regex.IsMatch(customer.PhoneNumber, Pattern);
            if (checkResult)
            {
                return "Số điện thoại không thể chứa chữ cái";
            }

            // No error
            return null;
        }

        /// <summary>
        /// Checks a category for any format-related issues.
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
        /// Checks a product for any format-related issues.
        /// </summary>
        /// <param name="product">The product to check.</param>
        /// <returns>
        /// A string containing the error message if the check fails, or null if the check succeeds.
        /// </returns>
        public string? Check(Product product)
        {
            if (!string.IsNullOrWhiteSpace(product.Image) && !Path.IsPathRooted(product.Image))
            {
                return "Đường dẫn ảnh không hợp lệ";
            }
            return null;
        }

        /// <summary>
        /// Checks an order for any format-related issues.
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
