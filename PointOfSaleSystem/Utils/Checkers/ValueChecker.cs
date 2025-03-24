using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Models;

namespace PointOfSaleSystem.Utils.Checkers
{
    public class ValueChecker : IChecker
    {
        public string? Check(Customer customer)
        {
            if(customer.Gender is not null && !customer.Gender.Equals("Nam") && !customer.Gender.Equals("Nữ"))
            {
                return "Giới tính phải là nam hoặc nữ";
            }
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
