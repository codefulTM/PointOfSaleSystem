using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Models;
using PointOfSaleSystem.Services;
using PointOfSaleSystem.Utils;

namespace PointOfSaleSystem.Views.ViewModels
{
    public class CustomerViewModel
    {
        public FullObservableCollection<Customer> Customers { get; set; }

        public CustomerViewModel()
        {
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            var customerRepository = dao.Customers;
            Customers = new FullObservableCollection<Customer>(customerRepository.GetAll());
        }
    }
}