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
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProductDetailsWindow : Window
    {
        Product product;
        public ProductDetailsWindow(Product product)
        {
            this.product = product;
            this.InitializeComponent();
            this.Activated += ProductDetailsWindowActivated;
        }

        private void ProductDetailsWindowActivated(object sender, WindowActivatedEventArgs args)
        {
            productInfo.DataContext = product;
            if(product.Category == null)
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

        private void OpenUpdateWindow(object sender, RoutedEventArgs e)
        {
            var updateProductWindow = new UpdateProductWindow(product);
            updateProductWindow.Activate();
            this.Close();
        }

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
                ProductRepository productRepo = ProductRepository.GetInstance();
                productRepo.Delete(product.Id);

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
