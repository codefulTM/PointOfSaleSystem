using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PointOfSaleSystem.Models;
using PointOfSaleSystem.Services;
using PointOfSaleSystem.Views.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// A page for managing tables.
    /// </summary>
    public sealed partial class TablePage : Page
    {
        public TableViewModel ViewModel { get; set; } = new TableViewModel();
        private Table? _selectedTable { get; set; } 

        /// <summary>
        /// Initializes a new instance of the <c>TablePage</c> class.
        /// </summary>
        public TablePage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        /// <summary>
        /// Handles the selection changed event of the table list view.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void TableListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lấy bàn được chọn
            _selectedTable = TableListView.SelectedItem as Table;

            // Kích hoạt nút "Đặt bàn" nếu có bàn được chọn
            BookingButton.IsEnabled = _selectedTable != null;
            DeleteBookingButton.IsEnabled = _selectedTable != null;
        }

        /// <summary>
        /// Handles the click event of the add button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new AddNewTableWindow(ViewModel);
            screen.Activate();
        }

        /// <summary>
        /// Handles the click event of the booking button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void bookingButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTable != null)
            {
                // Kiểm tra nếu bàn đã được đặt
                if (_selectedTable.State == "booked")
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Lỗi",
                        Content = $"Bàn '{_selectedTable.Name}' đã được đặt trước. Vui lòng chọn bàn khác.",
                        CloseButtonText = "Đóng",
                        XamlRoot = this.Content.XamlRoot
                    };

                    _ = dialog.ShowAsync(); // Hiển thị thông báo lỗi
                    return;
                }

                // Nếu bàn chưa được đặt, tiếp tục logic đặt bàn
                var customerViewModel = new CustomerViewModel();
                var bookingWindow = new BookingWindow(customerViewModel, _selectedTable);

                // Hiển thị cửa sổ đặt bàn
                bookingWindow.Activate();
            }
        }

        /// <summary>
        /// Handles the click event of the delete booking button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void deleteBookingButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTable == null)
            {
                var dialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Vui lòng chọn bàn để hủy đặt bàn.",
                    CloseButtonText = "Đóng",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }

            // Giữ bản sao
            var tableToUpdate = _selectedTable;

            if (tableToUpdate.State == "empty")
            {
                var dialog = new ContentDialog
                {
                    Title = "Thông báo",
                    Content = $"Bàn '{tableToUpdate.Name}' hiện không được đặt. Không thể hủy đặt bàn.",
                    CloseButtonText = "Đóng",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }

            var confirmDialog = new ContentDialog
            {
                Title = "Xác nhận",
                Content = $"Bạn có chắc chắn muốn hủy đặt bàn '{tableToUpdate.Name}' không?",
                PrimaryButtonText = "Hủy đặt bàn",
                CloseButtonText = "Đóng",
                XamlRoot = this.Content.XamlRoot
            };

            var result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                tableToUpdate.State = "empty";
                tableToUpdate.CustomerId = null;
                tableToUpdate.BookTime = null;

                var dao = Services.Services.GetKeyedSingleton<IDao>();
                dao.Tables.Update(tableToUpdate);

                var successDialog = new ContentDialog
                {
                    Title = "Thành công",
                    Content = $"Đã hủy đặt bàn '{tableToUpdate.Name}' thành công.",
                    CloseButtonText = "Đóng",
                    XamlRoot = this.Content.XamlRoot
                };
                await successDialog.ShowAsync();
            }
        }

        /// <summary>
        /// Handles the click event of the delete button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var table = button?.Tag as Table;

            if (table != null)
            {
                var dialog = new ContentDialog
                {
                    Title = "Xác nhận",
                    Content = $"Bạn có chắc chắn muốn xóa bàn '{table.Name}' không?",
                    PrimaryButtonText = "Xóa",
                    CloseButtonText = "Hủy",
                    XamlRoot = this.Content.XamlRoot
                };

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    IDao dao = Services.Services.GetKeyedSingleton<IDao>();
                    dao.Tables.Delete(table.Id);
                    ViewModel.Tables.Remove(table);
                }
            }
        }
    }
}
