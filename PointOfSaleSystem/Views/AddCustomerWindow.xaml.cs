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
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddCustomerWindow : Window
    {
        private CustomerViewModel _customerViewModel;
        public AddCustomerWindow(CustomerViewModel ViewModel)
        {
            this.InitializeComponent();
            _customerViewModel = ViewModel;
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog;

            var newCustomer = new Customer
            {
                Name = NameTextBox.Text,
                PhoneNumber = PhoneNumberTextBox.Text,
                Address = AddressTextBox.Text,
                Birthday = BirthdayDatePicker.Date.DateTime,
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
