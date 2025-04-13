using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Models;

namespace PointOfSaleSystem.Services
{
    public interface IDao
    {
        IRepository<Category> Categories { get; set; }
        IRepository<Product> Products { get; set; }
        IRepository<Customer> Customers { get; set; }
        IRepository<Order> Orders { get; set; }
        IRepository<OrderDetail> OrderDetails { get; set; }
    }
}
