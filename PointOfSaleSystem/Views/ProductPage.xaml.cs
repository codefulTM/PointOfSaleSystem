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
    /// A page that displays a list of products and provides options to add, search, and view details of products.
    /// </summary>
    public sealed partial class ProductPage : Page
    {
        public IDao dao { get; set; }
        public ProductViewModel ProductViewModel { get; set; } = new ProductViewModel();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductPage"/> class.
        /// Retrieves the DAO singleton, initializes the component, and sets the DataContext.
        /// </summary>
        /// <returns>A new instance of the ProductPage.</returns>
        public ProductPage()
        {
            dao = Services.Services.GetKeyedSingleton<IDao>();
            this.InitializeComponent();
            this.DataContext = ProductViewModel;
        }

        /// <summary>
        /// Handles the click event for adding a product.
        /// Opens the AddProductWindow to allow the user to add a new product.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>This method does not return a value.</returns>
        public void AddProduct(object sender, RoutedEventArgs e)
        {
            // Add product to cart
            AddProductWindow addProductWindow = new AddProductWindow(ProductViewModel);
            addProductWindow.Activate();
        }

        /// <summary>
        /// Handles the query submission event for the search box.
        /// This method is intended for implementing product search functionality.
        /// </summary>
        /// <param name="sender">The source of the event, the AutoSuggestBox.</param>
        /// <param name="e">Event data containing the query text.</param>
        /// <returns>This method does not return a value.</returns>
        public void Search(object sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            // Search for product
        }

        /// <summary>
        /// Handles the ItemClick event for the product list.
        /// Opens the ProductDetailsWindow to display the details of the clicked product.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data containing information about the clicked item.</param>
        /// <returns>This method does not return a value.</returns>
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
