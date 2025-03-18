using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSaleSystem.Models
{
    public interface IDao
    {
        IRepository<Category> Categories { get; set; }
        IRepository<Product> Products { get; set; }
        IRepository<Customer> Customers { get; set; }
    }
}
