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
    public class ProductViewModel
    {
        public FullObservableCollection<Product> Products { get; set; }

        public ProductViewModel()
        {
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            var productRepository = dao.Products;
            Products = new FullObservableCollection<Product>(productRepository.GetAll());
        }
    }
}
