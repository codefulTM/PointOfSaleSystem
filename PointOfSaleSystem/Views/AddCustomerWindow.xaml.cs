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
using PointOfSaleSystem.Services;
using PointOfSaleSystem.Utils.Checkers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// A window that allows the user to add a new customer to the system.
    /// </summary>
    public sealed partial class AddCustomerWindow : Window
    {
        private CustomerViewModel _customerViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddCustomerWindow"/> class.
        /// </summary>
        /// <param name="customerViewModel">The CustomerViewModel used to manage customer data.</param>        
        public AddCustomerWindow(CustomerViewModel customerViewModel)
        {
            this.InitializeComponent();
            _customerViewModel = customerViewModel;
        }

        /// <summary>
        /// Handles the click event for the Add Customer button.
        /// Retrieves customer information from the input fields, performs validation,
        /// adds the new customer to the database and the ViewModel, displays a success or error dialog,
        /// and closes the window upon successful addition.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog;

            var newCustomer = new Customer
            {
                Name = NameTextBox.Text,
                PhoneNumber = PhoneNumberTextBox.Text,
                Address = AddressTextBox.Text,
                Birthday = BirthdayDatePicker.SelectedDate is not null ? BirthdayDatePicker.SelectedDate.Value.DateTime : (DateTime?)null,
                Gender = (bool)MaleRadioButton.IsChecked ? "Nam" : (bool)FemaleRadioButton.IsChecked ? "Nữ" : ""
            };

            // Constraint checking
            // Check format
            string? checkRes = newCustomer.AcceptForChecking(new FormatChecker());
            if(checkRes is not null)
            {
                dialog = new ContentDialog
                {
                    Title = "Nhập thông tin sai định dạng",
                    Content = checkRes,
                    CloseButtonText = "OK"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
                return;
            }

            // Check required fields
            checkRes = newCustomer.AcceptForChecking(new RequiredFieldChecker());
            if(checkRes is not null)
            {
                dialog = new ContentDialog
                {
                    Title = "Nhập thiếu thông tin cần thiết",
                    Content = checkRes,
                    CloseButtonText = "OK"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
                return;
            }

            // Check values
            checkRes = newCustomer.AcceptForChecking(new ValueChecker());
            if(checkRes is not null)
            {
                dialog = new ContentDialog
                {
                    Title = "Nhập thông tin không hợp lệ",
                    Content = checkRes,
                    CloseButtonText = "OK"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
                return;
            }

            var dao = Services.Services.GetKeyedSingleton<IDao>();
            dao.Customers.Create(newCustomer);
            _customerViewModel.Customers.Add(newCustomer);


            dialog = new ContentDialog
            {
                Title = "Thành công",
                Content = "Khách hàng đã được thêm vào cơ sở dữ liệu.",
                CloseButtonText = "OK"
            };

            dialog.XamlRoot = this.Content.XamlRoot;
            await dialog.ShowAsync();
            this.Close();
        }
    }
}
