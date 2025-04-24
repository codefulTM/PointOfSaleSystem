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
    /// A page that allows users to manage payment methods, including bank and MoMo options.
    /// It provides functionality to set default methods, add new methods, remove methods, and view QR codes.
    /// </summary>
    public sealed partial class PaymentMethodPage : Page
    {
        public PaymentMethodViewModel ViewModel { get; set; } = new PaymentMethodViewModel();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethodPage"/> class.
        /// Sets the DataContext for the page.
        /// </summary>
        /// <returns>A new instance of the PaymentMethodPage.</returns>
        public PaymentMethodPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        /// <summary>
        /// Handles the click event for setting a bank payment method as default.
        /// Displays a confirmation dialog and updates the default status in the database and ViewModel if confirmed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Handles the click event for adding a new bank payment method.
        /// Opens a new window to add a new bank payment method.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void Bank_AddPaymentMethodClick(object sender, RoutedEventArgs e)
        {
            var addBankPaymentMethodWindow = new AddBankPaymentMethodWindow(ViewModel);
            addBankPaymentMethodWindow.Activate();
        }

        /// <summary>
        /// Handles the click event for removing a bank payment method.
        /// Removes the selected bank payment method from the list and updates the data source.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
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

        /// <summary>
        /// Handles the click event for setting a MoMo payment method as default.
        /// Displays a confirmation dialog and updates the default status in the database and ViewModel if confirmed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>        
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
        
        /// <summary>
        /// Handles the click event for adding a new MoMo payment method.
        /// Opens the AddMomoPaymentMethodWindow.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>This method does not return a value.</returns>
        private void Momo_AddPaymentMethodClick(object sender, RoutedEventArgs e)
        {
            var addMomoPaymentMethodWindow = new AddMomoPaymentMethodWindow(ViewModel);
            addMomoPaymentMethodWindow.Activate();
        }

        /// <summary>
        /// Handles the click event for removing a MoMo payment method.
        /// Displays a confirmation dialog and removes the selected payment method from the database and ViewModel if confirmed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Handles the click event for showing the default bank QR code.
        /// Finds the default bank payment method and opens the ShowBankQRWindow.
        /// Displays a message if no default method is set.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Handles the click event for showing the default MoMo QR code.
        /// Finds the default MoMo payment method and opens the ShowMomoQRWindow.
        /// Displays a message if no default method is set.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>        
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
