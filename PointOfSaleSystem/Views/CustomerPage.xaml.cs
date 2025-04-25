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
    /// A page that displays a list of customers and provides options to view, add, update, and delete customers.
    /// </summary>
    public sealed partial class CustomerPage : Page
    {
        public CustomerViewModel ViewModel { get; set; } = new CustomerViewModel();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerPage"/> class.
        /// </summary>
        /// <returns>A new instance of the CustomerPage.</returns>
        public CustomerPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        /// <summary>
        /// Handles the click event for the View Detail button.
        /// Opens a new window to display the details of the selected customer.
        /// </summary>
        /// <param name="sender">The source of the event, expected to be a Button with a Customer tag.</param>
        /// <param name="e">Event data.</param>
        /// <returns>Nothing.</returns>
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

        /// <summary>
        /// Handles the click event for the Add button.
        /// Opens a new window to add a new customer.
        /// </summary>
        /// <param name="sender">The source of the event, expected to be a Button.</param>
        /// <param name="e">Event data.</param>
        /// <returns>This method does not return a value.</returns>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new AddCustomerWindow(ViewModel);
            screen.Activate();
        }

        /// <summary>
        /// Handles the click event for the Update button.
        /// Opens a new window to update the details of the selected customer.
        /// </summary>
        /// <param name="sender">The source of the event, expected to be a Button with a Customer tag.</param>
        /// <param name="e">Event data.</param>
        /// <returns>This method does not return a value.</returns>
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

        /// <summary>
        /// Handles the click event for the Delete button.
        /// Displays a confirmation dialog and, if confirmed, deletes the selected customer
        /// from the ViewModel and the database.
        /// </summary>
        /// <param name="sender">The source of the event, expected to be a Button with a Customer tag.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
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
