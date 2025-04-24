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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// A window that displays the detailed information of a specific product.
    /// It also provides options to update or delete the product.
    /// </summary>
    public sealed partial class ProductDetailsWindow : Window
    {
        Product product;
        ProductViewModel ProductViewModel = new ProductViewModel();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDetailsWindow"/> class.
        /// Sets the product and ViewModel to be used and subscribes to the Activated event.
        /// </summary>
        /// <param name="product">The product object whose details are to be displayed.</param>
        /// <param name="productViewModel">The ViewModel used to manage product data, specifically for removing the product after deletion.</param>
        /// <returns>A new instance of the ProductDetailsWindow.</returns>
        public ProductDetailsWindow(Product product, ProductViewModel productViewModel)
        {
            this.product = product;
            this.ProductViewModel = productViewModel;
            this.InitializeComponent();
            this.Activated += ProductDetailsWindowActivated;
        }

        /// <summary>
        /// Handles the Activated event of the ProductDetailsWindow.
        /// Sets the DataContext for the product information and displays "Chưa có dữ liệu"
        /// for any product properties that are null.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Event data.</param>
        /// <returns>This method does not return a value.</returns>
        private void ProductDetailsWindowActivated(object sender, WindowActivatedEventArgs args)
        {
            productInfo.DataContext = product;
            if (product.Category == null)
            {
                productCategory.Text = "Chưa có dữ liệu";
            }
            if (product.Brand == null)
            {
                productBrand.Text = "Chưa có dữ liệu";
            }
            if(product.Quantity == null)
            {
                productQuantity.Text = "Chưa có dữ liệu";
            }
            if (product.CostPrice == null)
            {
                productCostPrice.Text = "Chưa có dữ liệu";
            }
            if (product.SellingPrice == null)
            {
                productSellingPrice.Text = "Chưa có dữ liệu";
            }
        }

        /// <summary>
        /// Handles the click event for the Open Update Window button.
        /// Creates and activates a new UpdateProductWindow for the current product, then closes the current window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>This method does not return a value.</returns>
        private void OpenUpdateWindow(object sender, RoutedEventArgs e)
        {
            var updateProductWindow = new UpdateProductWindow(product);
            updateProductWindow.Activate();
            this.Close();
        }

        /// <summary>
        /// Handles the click event for the Delete Product button.
        /// Displays a confirmation dialog and, if confirmed, deletes the product from the database
        /// and removes it from the ViewModel's product list. Displays a success dialog afterwards and closes the window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async void DeleteProduct(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteDialog = new ContentDialog
            {
                Title = "Xác nhận xóa",
                Content = "Bạn có chắc chắn muốn xóa sản phẩm này không?",
                PrimaryButtonText = "Xóa",
                CloseButtonText = "Hủy",
            };

            deleteDialog.XamlRoot = this.Content.XamlRoot;
            ContentDialogResult result = await deleteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                IDao dao = Services.Services.GetKeyedSingleton<IDao>();
                var productRepo = dao.Products;
                productRepo.Delete(product.Id);
                ProductViewModel.Products.Remove(product);

                ContentDialog successDialog = new ContentDialog
                {
                    Title = "Thành công",
                    Content = "Sản phẩm đã được xóa thành công.",
                    CloseButtonText = "OK"
                };

                successDialog.XamlRoot = this.Content.XamlRoot;
                await successDialog.ShowAsync();
                this.Close();
            }
        }
    }
}
