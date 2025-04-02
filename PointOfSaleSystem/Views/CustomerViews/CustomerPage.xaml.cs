using System;
using System.Collections.Generic;
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
    public sealed partial class CustomerPage : Page
    {
        public CustomerViewModel ViewModel { get; set; } = new CustomerViewModel();
        public CustomerPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        private void viewDetailButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var customer = button?.Tag as Customer;
            if (customer != null)
            {
                var screen = new CustomerDetailsWindow(customer);
                screen.Activate();
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new AddCustomerWindow(ViewModel);
            screen.Activate();
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var customer = button?.Tag as Customer;
            if(customer is not null)
            {
                var screen = new UpdateCustomerWindow(ViewModel, customer);
                screen.Activate();
            }
        }

        private async void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            bool isRemovable = false;

            var dialog = new ContentDialog
            {
                Title = "Xác nhận",
                Content = "Bạn có chắc chắn muốn xóa khách hàng này?",
                PrimaryButtonText = "Xóa",
                CloseButtonText = "Hủy"
            };

            dialog.XamlRoot = this.Content.XamlRoot;
            dialog.PrimaryButtonClick += (_sender, _e) => isRemovable = true;
            await dialog.ShowAsync();

            if(isRemovable)
            {
                var button = sender as Button;
                var customer = button?.Tag as Customer;
                if(customer is not null)
                {
                    ViewModel.Customers.Remove(customer);
                    IDao Dao = Services.Services.GetKeyedSingleton<IDao>();
                    Dao.Customers.Delete((int)customer.Id);
                }
            }
        }
    }
}
