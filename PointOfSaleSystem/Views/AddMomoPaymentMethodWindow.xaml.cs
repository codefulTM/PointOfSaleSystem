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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddMomoPaymentMethodWindow : Window
    {
        public PaymentMethodViewModel _paymentMethodViewModel { get; set; }
        public AddMomoPaymentMethodWindow(PaymentMethodViewModel paymentMethodViewModel)
        {
            _paymentMethodViewModel = paymentMethodViewModel;
            this.InitializeComponent();
        }

        private async void AddMomoPaymentMethodButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a new bank payment method
            var newPaymentMethod = new PaymentMethod
            {
                PhoneNumber = momoNumberTextBox.Text,
                AccountHolder = nameTextBox.Text,
                Type = "momo"
            };

            // Add the new payment method to the list
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            dao.PaymentMethods.Create(newPaymentMethod);
            _paymentMethodViewModel.MomoPaymentMethods.Add(newPaymentMethod);


            var dialog = new ContentDialog
            {
                Title = "Thành công",
                Content = "Phương thức thanh toán đã được thêm",
                CloseButtonText = "OK"
            };

            dialog.XamlRoot = this.Content.XamlRoot;
            await dialog.ShowAsync();
            this.Close();
        }
    }
}
