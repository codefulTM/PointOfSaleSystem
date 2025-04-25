using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Models;

namespace PointOfSaleSystem.Services
{
    /// <summary>
    /// Represents a data access object (DAO) interface that provides 
    /// repositories for managing various entities in the system.
    /// </summary>
    public interface IDao
    {
        IRepository<Category> Categories { get; set; }
        IRepository<Product> Products { get; set; }
        IRepository<Customer> Customers { get; set; }
        IRepository<Order> Orders { get; set; }
        IRepository<OrderDetail> OrderDetails { get; set; }
        IRepository<PaymentMethod> PaymentMethods { get; set; }
    }
}
