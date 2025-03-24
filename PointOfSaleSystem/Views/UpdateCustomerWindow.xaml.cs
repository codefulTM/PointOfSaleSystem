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
using PointOfSaleSystem.Services;
using PointOfSaleSystem.Views.ViewModels;
using System.Diagnostics;
using PointOfSaleSystem.Utils.Checkers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UpdateCustomerWindow : Window
    {
        private CustomerViewModel _customerViewModel;
        public UpdateCustomerWindow(CustomerViewModel customerViewModel, Customer customer)
        {
            this.InitializeComponent();
            _customerViewModel = customerViewModel;
            customerInformation.DataContext = customer;
            var radioButtons = genderRadioButtons.Items;
            foreach(var item in radioButtons)
            {
                if(item is RadioButton radioButton)
                {
                    if(radioButton.Content.ToString().Equals(customer.Gender))
                    {
                        radioButton.IsChecked = true;
                        break;
                    }
                }
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var customer = button?.Tag as Customer;
            ContentDialog dialog;

            Customer newCustomer = new Customer()
            {
                Name = customerName.Text,
                PhoneNumber = customerPhoneNumber.Text,
                Address = customerAddress.Text,
                Birthday = customerBirthday.Date.DateTime,
                Gender = (genderRadioButtons.SelectedItem as RadioButton)?.Content.ToString()
            };

            // Constraint checking
            // Check format
            string? checkRes = newCustomer.AcceptForChecking(new FormatChecker());
            if (checkRes is not null)
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
            if (checkRes is not null)
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
            if (checkRes is not null)
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

            customer.Name = newCustomer.Name;
            customer.PhoneNumber = newCustomer.PhoneNumber;
            customer.Address = newCustomer.Address;
            customer.Birthday = newCustomer.Birthday;
            customer.Gender = newCustomer.Gender;

            var dao = Services.Services.GetKeyedSingleton<IDao>();
            dao.Customers.Update(customer);
            dialog = new ContentDialog
            {
                Title = "Thành công",
                Content = "Khách hàng đã được cập nhật.",
                CloseButtonText = "OK"
            };
            dialog.XamlRoot = this.Content.XamlRoot;
            await dialog.ShowAsync();

            this.Close();
        }
    }
}
