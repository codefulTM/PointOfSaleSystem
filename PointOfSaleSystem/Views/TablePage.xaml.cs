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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TablePage : Page
    {
        public TableViewModel ViewModel { get; set; } = new TableViewModel();
        private Table? _selectedTable = new Table();

        public TablePage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        private void TableListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lấy bàn được chọn
            _selectedTable = TableListView.SelectedItem as Table;

            // Kích hoạt nút "Đặt bàn" nếu có bàn được chọn
            BookingButton.IsEnabled = _selectedTable != null;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new AddNewTableWindow(ViewModel);
            screen.Activate();
        }

        private void bookingButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTable != null)
            {
                var customerViewModel = new CustomerViewModel();
                var bookingWindow = new BookingWindow(customerViewModel, _selectedTable);

                // Hiển thị cửa sổ đặt bàn
                bookingWindow.Activate();
            }
        }

        private void deleteBookingButton_Click(object sender, RoutedEventArgs e)
        {
            // Logic hủy đặt bàn
        }

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
