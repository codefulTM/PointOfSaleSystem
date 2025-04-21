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
using PointOfSaleSystem.Views.ViewModels;
using PointOfSaleSystem.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrderPage : Page
    {
        public OrderViewModel ViewModel { get; set; } = new OrderViewModel();
        public OrderPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
            // Load data from the database
            ViewModel.LoadCategories();
            ViewModel.LoadProducts();
        }

        private void OnProductClicked(object sender, ItemClickEventArgs e)
        {
            var clickedProduct = e.ClickedItem as Product;
            if (clickedProduct != null)
            {
                ViewModel.AddProductToOrder(clickedProduct);
            }
        }

        private async void DeleteSelectedProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedProduct == null)
            {
                // Hiển thị thông báo nếu không có sản phẩm nào được chọn
                var dialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Vui lòng chọn một sản phẩm để xóa.",
                    CloseButtonText = "Đóng",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }

            // Hiển thị hộp thoại xác nhận
            var confirmDialog = new ContentDialog
            {
                Title = "Xác nhận xóa",
                Content = $"Bạn có chắc chắn muốn xóa sản phẩm '{ViewModel.SelectedProduct.Name}' khỏi đơn hàng không?",
                PrimaryButtonText = "Xóa",
                CloseButtonText = "Hủy",
                XamlRoot = this.Content.XamlRoot
            };

            var result = await confirmDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Xóa sản phẩm khỏi OrderProducts
                ViewModel.OrderProducts.Remove(ViewModel.SelectedProduct);

                // Cập nhật Total và Tax
                ViewModel.RaisePropertyChanged(nameof(ViewModel.Total));
                ViewModel.RaisePropertyChanged(nameof(ViewModel.Tax));
            }
        }

        private async void Checkout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Gọi phương thức CreateOrder trong ViewModel
                ViewModel.CreateOrder();

                // Hiển thị thông báo thành công
                var successDialog = new ContentDialog
                {
                    Title = "Thành công",
                    Content = "Chuyển đến giao diện thanh toán",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await successDialog.ShowAsync();
            }
            catch (InvalidOperationException ex)
            {
                // Hiển thị thông báo lỗi nếu không có sản phẩm trong đơn hàng
                var errorDialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi chung
                var errorDialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = $"Đã xảy ra lỗi: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }
    }
}
