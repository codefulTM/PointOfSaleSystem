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
    /// A page that allows users to create and manage customer orders.
    /// It displays products, allows adding them to an order, deleting items, and proceeding to checkout.
    /// </summary>
    public sealed partial class OrderPage : Page
    {
        public OrderViewModel ViewModel { get; set; } = new OrderViewModel();

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderPage"/> class.
        /// Sets the DataContext and loads initial data (categories and products) into the ViewModel.
        /// </summary>
        /// <returns>A new instance of the OrderPage.</returns>
        public OrderPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
            // Load data from the database
            ViewModel.LoadCategories();
            ViewModel.LoadProducts();
        }

        /// <summary>
        /// Handles the click event when a product item is clicked in the product list.
        /// Adds the clicked product to the current order in the ViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data containing information about the clicked item.</param>
        /// <returns>This method does not return a value.</returns>        
        private void OnProductClicked(object sender, ItemClickEventArgs e)
        {
            var clickedProduct = e.ClickedItem as Product;
            if (clickedProduct != null)
            {
                ViewModel.AddProductToOrder(clickedProduct);
            }
        }

        /// <summary>
        /// Handles the click event for the Delete Selected Product button.
        /// Removes the currently selected product from the order after displaying a confirmation dialog.
        /// Updates the total and tax values in the ViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
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

                // Cập nhật Total
                ViewModel.RaisePropertyChanged(nameof(ViewModel.Total));
            }
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            // Tạo ViewModel cho danh sách khách hàng  
            var customerViewModel = new CustomerViewModel();

            // Mở cửa sổ chọn khách hàng  
            var selectCustomerWindow = new SelectCustomerWindow(customerViewModel);
            selectCustomerWindow.Activate();

            // Lắng nghe sự kiện khi cửa sổ bị đóng  
            selectCustomerWindow.Closed += (_, _) =>
            {
                if (selectCustomerWindow.SelectedCustomer != null)
                {
                    // Gán khách hàng được chọn vào đơn hàng  
                    ViewModel.SetCustomer(selectCustomerWindow.SelectedCustomer);
                }
            };
        }

        /// <summary>
        /// Handles the click event for the Checkout button.
        /// Creates an order from the current items, opens the payment window,
        /// and handles potential errors like an empty order.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async void Checkout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra nếu chưa thêm khách hàng
                if (ViewModel.SelectedCustomer == null)
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Lỗi",
                        Content = "Vui lòng thêm khách hàng trước khi thanh toán.",
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                    return;
                }

                // Gọi phương thức CreateOrder trong ViewModel
                var order = ViewModel.CreateOrder();

                // Mở cửa sổ thanh toán
                var paymentWindow = new PaymentWindow(order);
                paymentWindow.Activate();
                //// Hiển thị thông báo thành công 
                //var successDialog = new ContentDialog
                //{
                //    Title = "Thành công",
                //    Content = "Chuyển đến giao diện thanh toán",
                //    CloseButtonText = "OK",
                //    XamlRoot = this.Content.XamlRoot
                //};
                //await successDialog.ShowAsync();
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
