using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PointOfSaleSystem.Models;
using System.Collections.ObjectModel;
using PointOfSaleSystem.Views.ViewModels;
using PointOfSaleSystem.Services;
using Windows.UI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProductPage : Page
    {
        public IDao dao { get; set; }
        public ProductViewModel ProductViewModel { get; set; } = new ProductViewModel();
        public ProductPage()
        {
            dao = Services.Services.GetKeyedSingleton<IDao>();
            this.InitializeComponent();
            this.DataContext = ProductViewModel;
        }

        public void AddProduct(object sender, RoutedEventArgs e)
        {
            // Add product to cart
            AddProductWindow addProductWindow = new AddProductWindow(ProductViewModel);
            addProductWindow.Activate();
        }

        public void Search(object sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            // Search for product
        }

        public void OnItemClicked(object sender, ItemClickEventArgs e)
        {
            if(e.ClickedItem is Product product)
            {
                ProductDetailsWindow productDetailsWindow = new ProductDetailsWindow(product, ProductViewModel);
                productDetailsWindow.Activate();
            }
        }
    }
}
