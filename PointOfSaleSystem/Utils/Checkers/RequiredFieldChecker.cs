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
        public string? Check(Customer customer)
        {
            if(string.IsNullOrWhiteSpace(customer.Name))
            {
                return "Tên không thể là rỗng";
            }
            return null;
        }

        public string? Check(Category category)
        {
            return null;
        }

        public string? Check(Product product)
        {
            if(string.IsNullOrWhiteSpace(product.Name))
            {
                return "Tên sản phẩm không được để trống";
            }
            return null;
        }

        public string? Check(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
