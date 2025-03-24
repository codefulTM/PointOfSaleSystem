using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PointOfSaleSystem.Models;

namespace PointOfSaleSystem.Utils.Checkers
{
    public class FormatChecker : IChecker
    {
        public string Pattern { get; set; }

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

        public string? Check(Category category)
        {
            throw new NotImplementedException();
        }

        public string? Check(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
