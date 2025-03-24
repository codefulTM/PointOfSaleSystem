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

            Customer newCustomer = new Customer()
            {
                Name = customerName.Text,
                PhoneNumber = customerPhoneNumber.Text,
                Address = customerAddress.Text,
                Birthday = customerBirthday.Date.DateTime,
                Gender = (genderRadioButtons.SelectedItem as RadioButton)?.Content.ToString()
            };

            Checker checker = new Checker();
            var checkStatus = checker.Check(newCustomer);
            if (checkStatus is null)
            {
                customer.Name = newCustomer.Name;
                customer.PhoneNumber = newCustomer.PhoneNumber;
                customer.Address = newCustomer.Address;
                customer.Birthday = newCustomer.Birthday;
                customer.Gender = newCustomer.Gender;

                var dao = Services.Services.GetKeyedSingleton<IDao>();
                dao.Customers.Update(customer);
                var dialog = new ContentDialog
                {
                    Title = "Thành công",
                    Content = "Khách hàng đã được cập nhật.",
                    CloseButtonText = "OK"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();

                this.Close();
            }
            else
            {
                var dialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = checkStatus,
                    CloseButtonText = "OK"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
            }
        }
    }
}
