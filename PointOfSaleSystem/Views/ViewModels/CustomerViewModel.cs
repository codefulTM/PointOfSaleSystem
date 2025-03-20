using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Models;
using PointOfSaleSystem.Utils;

namespace PointOfSaleSystem.Views.ViewModels
{
    public class CustomerViewModel
    {
        public FullObservableCollection<Customer> Customers { get; set; }

        public CustomerViewModel()
        {
            var customerRepository = CustomerRepository.GetInstance();
            Customers = new FullObservableCollection<Customer>(customerRepository.GetAll());
        }
    }
}