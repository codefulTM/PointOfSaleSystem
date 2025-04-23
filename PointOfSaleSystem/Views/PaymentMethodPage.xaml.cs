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
using System.Threading.Tasks;
using PointOfSaleSystem.Models;
using PointOfSaleSystem.Services;
using Windows.Media.Capture.Core;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PaymentMethodPage : Page
    {
        public PaymentMethodViewModel ViewModel { get; set; } = new PaymentMethodViewModel();
        public PaymentMethodPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        private async void Bank_SetDefaultClick(object sender, RoutedEventArgs e)
        {
            // Get the selected payment method
            var paymentMethod = bankPaymentMethods.SelectedItem as PaymentMethod;
            if (paymentMethod != null)
            {
                // Ask user if they want to set this payment method as default
                var dialog = new ContentDialog
                {
                    Title = "Xác nhận",
                    Content = "Bạn có chắc chắn muốn đặt phương thức thanh toán này làm mặc định?",
                    PrimaryButtonText = "Đặt mặc định",
                    CloseButtonText = "Hủy"
                };

                dialog.XamlRoot = this.Content.XamlRoot;
                dialog.PrimaryButtonClick += (s, args) =>
                {
                    var dao = Services.Services.GetKeyedSingleton<IDao>();

                    // Set the selected payment method as default
                    foreach (var method in ViewModel.BankPaymentMethods)
                    {
                        method.IsDefault = false;
                        dao.PaymentMethods.Update(method);
                    }
                    paymentMethod.IsDefault = true;
                    dao.PaymentMethods.Update(paymentMethod);
                };

                // Show the dialog
                await dialog.ShowAsync();
            }
        }

        private void Bank_AddPaymentMethodClick(object sender, RoutedEventArgs e)
        {
            var addBankPaymentMethodWindow = new AddBankPaymentMethodWindow(ViewModel);
            addBankPaymentMethodWindow.Activate();
        }

        private async void Bank_RemovePaymentMethodClick(object sender, RoutedEventArgs e)
        {
            // Get the selected payment method
            var paymentMethod = bankPaymentMethods.SelectedItem as PaymentMethod;
            if (paymentMethod != null)
            {
                // Remove the payment method from the list
                var dialog = new ContentDialog
                {
                    Title = "Xác nhận",
                    Content = "Bạn có chắc chắn muốn xóa phương thức thanh toán này?",
                    PrimaryButtonText = "Xóa",
                    CloseButtonText = "Hủy"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                dialog.PrimaryButtonClick += (s, args) =>
                {
                    var dao = Services.Services.GetKeyedSingleton<IDao>();
                    dao.PaymentMethods.Delete(paymentMethod.Id);
                    ViewModel.BankPaymentMethods.Remove(paymentMethod);
                };
                await dialog.ShowAsync();
            }
        }

        private async void Momo_SetDefaultClick(object sender, RoutedEventArgs e)
        {
            // Get the selected payment method
            var paymentMethod = momoPaymentMethods.SelectedItem as PaymentMethod;
            if (paymentMethod != null)
            {
                // Ask user if they want to set this payment method as default
                var dialog = new ContentDialog
                {
                    Title = "Xác nhận",
                    Content = "Bạn có chắc chắn muốn đặt phương thức thanh toán này làm mặc định?",
                    PrimaryButtonText = "Đặt mặc định",
                    CloseButtonText = "Hủy"
                };

                dialog.XamlRoot = this.Content.XamlRoot;
                dialog.PrimaryButtonClick += (s, args) =>
                {
                    var dao = Services.Services.GetKeyedSingleton<IDao>();

                    // Set the selected payment method as default
                    foreach (var method in ViewModel.MomoPaymentMethods)
                    {
                        method.IsDefault = false;
                        dao.PaymentMethods.Update(method);
                    }
                    paymentMethod.IsDefault = true;
                    dao.PaymentMethods.Update(paymentMethod);
                };

                // Show the dialog
                await dialog.ShowAsync();
            }
        }

        private void Momo_AddPaymentMethodClick(object sender, RoutedEventArgs e)
        {
            var addMomoPaymentMethodWindow = new AddMomoPaymentMethodWindow(ViewModel);
            addMomoPaymentMethodWindow.Activate();
        }

        private async void Momo_RemovePaymentMethodClick(object sender, RoutedEventArgs e)
        {
            // Get the selected payment method
            var paymentMethod = momoPaymentMethods.SelectedItem as PaymentMethod;
            if (paymentMethod != null)
            {
                // Remove the payment method from the list
                var dialog = new ContentDialog
                {
                    Title = "Xác nhận",
                    Content = "Bạn có chắc chắn muốn xóa phương thức thanh toán này?",
                    PrimaryButtonText = "Xóa",
                    CloseButtonText = "Hủy"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                dialog.PrimaryButtonClick += (s, args) =>
                {
                    var dao = Services.Services.GetKeyedSingleton<IDao>();
                    dao.PaymentMethods.Delete(paymentMethod.Id);
                    ViewModel.MomoPaymentMethods.Remove(paymentMethod);
                };
                await dialog.ShowAsync();
            }
        }

        private async void ShowBankQR(object sender, RoutedEventArgs e)
        {
            // Get the default payment method
            var paymentMethod = ViewModel.BankPaymentMethods.FirstOrDefault(pm => pm.IsDefault);
            if(paymentMethod == null)
            {
                // Show a message if no default payment method is set
                var dialog = new ContentDialog
                {
                    Title = "Thông báo",
                    Content = "Vui lòng chọn một phương thức thanh toán mặc định trước khi xem QR.",
                    CloseButtonText = "OK"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
                return;
            }
            var showBankQRWindow = new ShowBankQRWindow(paymentMethod, 0);
            showBankQRWindow.Activate();
        }

        private async void ShowMomoQR(object sender, RoutedEventArgs e)
        {
            // Get the default payment method
            var paymentMethod = ViewModel.MomoPaymentMethods.FirstOrDefault(pm => pm.IsDefault);
            if (paymentMethod == null)
            {
                // Show a message if no default payment method is set
                var dialog = new ContentDialog
                {
                    Title = "Thông báo",
                    Content = "Vui lòng chọn một phương thức thanh toán mặc định trước khi xem QR.",
                    CloseButtonText = "OK"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
                return;
            }
            var showMomoQRWindow = new ShowMomoQRWindow(paymentMethod, 0);
            showMomoQRWindow.Activate();
        }
    }
}
