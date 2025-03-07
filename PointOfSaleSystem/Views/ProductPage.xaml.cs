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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProductPage : Page
    {
        public ProductRepository ProductRepo { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public ProductPage()
        {
            ProductRepo = ProductRepository.GetInstance();
            Products = new ObservableCollection<Product>(ProductRepo.GetAll());
            this.InitializeComponent();
        }

        public void AddProduct(object sender, RoutedEventArgs e)
        {
            // Add product to cart
            AddProductWindow addProductWindow = new AddProductWindow();
            addProductWindow.AddProductEvent += Refresh;
            addProductWindow.Activate();
        }

        private void Refresh(object? sender, EventArgs e)
        {
            Products = new ObservableCollection<Product>(ProductRepo.GetAll());
            productPage.ItemsSource = Products;
        }

        public void Search(object sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            // Search for product
        }

        public void OnItemClicked(object sender, ItemClickEventArgs e)
        {
            if(e.ClickedItem is Product product)
            {
                ProductDetailsWindow productDetailsWindow = new ProductDetailsWindow(product);
                productDetailsWindow.Activate();
            }
        }
    }
}
