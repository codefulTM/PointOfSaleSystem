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
            return null;
        }

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
    }
}
