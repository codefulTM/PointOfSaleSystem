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
    /// A window that allows the user to add a new MoMo payment method.
    /// </summary>
    public sealed partial class AddMomoPaymentMethodWindow : Window
    {
        public PaymentMethodViewModel _paymentMethodViewModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddMomoPaymentMethodWindow"/> class.
        /// </summary>
        /// <param name="paymentMethodViewModel">The view model used to update the list of payment methods.</param>
        public AddMomoPaymentMethodWindow(PaymentMethodViewModel paymentMethodViewModel)
        {
            _paymentMethodViewModel = paymentMethodViewModel;
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the click event for the Add MoMo Payment Method button.
        /// Retrieves information from the input fields, creates a new PaymentMethod object,
        /// saves it to the database, updates the ViewModel, displays a success dialog,
        /// and closes the window upon successful addition.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>Nothing.</returns>        
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
