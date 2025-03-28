using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Models;

namespace PointOfSaleSystem.Utils.Checkers
{
    public class RequiredFieldChecker : IChecker
    {
        public string? Check(Customer customer)
        {
            return null;
        }

        public string? Check(Category category)
        {
            throw new NotImplementedException();
        }

        public string? Check(Product product)
        {
            if(string.IsNullOrWhiteSpace(product.Name))
            {
                return "Tên sản phẩm không được để trống";
            }
            return null;
        }
    }
}
