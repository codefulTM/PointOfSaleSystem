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
using PointOfSaleSystem.Views.ViewModels;
using PointOfSaleSystem.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BookingWindow : Window
    {
        public CustomerViewModel CustomerViewModel { get; set; }
        public Table SelectedTable { get; set; }
        public Customer? SelectedCustomer { get; set; }
        public DateTimeOffset BookingDate { get; set; }
        public string BookingTime { get; set; } = string.Empty;

        public BookingWindow(CustomerViewModel cusViewModel, Table table)
        {
            this.InitializeComponent();
            CustomerViewModel = cusViewModel;
            SelectedTable = table;
            BookingDate = DateTimeOffset.Now;
        }

        private async void Select_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCustomer == null)
            {
                var dialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Vui lòng chọn khách hàng trước khi xác nhận.",
                    CloseButtonText = "Đóng",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }

            if (BookingDate.ToString("yyyy-MM-dd") == "")
            {
                var dialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Vui lòng chọn ngày đặt bàn.",
                    CloseButtonText = "Đóng",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }

            if (!TimeSpan.TryParse(BookingTime, out var time))
            {
                var dialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Thời gian không hợp lệ. Vui lòng nhập theo định dạng HH:mm.",
                    CloseButtonText = "Đóng",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }


            // Cập nhật vào cơ sở dữ liệu  
            var bookingTime = BookingDate.Date + time; // Combine date and time  
            var selectedTable = SelectedTable;

            if (selectedTable == null)
            {
                var dialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Không tìm thấy bàn tương ứng",
                    CloseButtonText = "Đóng",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }

            selectedTable.CustomerId = SelectedCustomer.Id;
            selectedTable.BookTime = bookingTime;
            selectedTable.State = "booked";

            var dao = Services.Services.GetKeyedSingleton<IDao>();
            dao.Tables.Update(selectedTable);

            // Optionally notify the user of success  
            var successDialog = new ContentDialog
            {
                Title = "Thành công",
                Content = "Đặt bàn thành công.",
                CloseButtonText = "Đóng",
                XamlRoot = this.Content.XamlRoot
            };
            await successDialog.ShowAsync();

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedCustomer = null;
            this.Close();
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var addCustomerWindow = new AddCustomerWindow(CustomerViewModel);
            addCustomerWindow.Activate();
        }
    }
}
