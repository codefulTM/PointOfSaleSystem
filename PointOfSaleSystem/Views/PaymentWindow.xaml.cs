﻿using System;
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
using PointOfSaleSystem.Services;
using PointOfSaleSystem.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// A window used for processing payments for an order.
    /// It displays the total amount, discount, amount due, and handles payment input and processing.
    /// </summary>
    public sealed partial class PaymentWindow : Window
    {
        public int TotalAmount { get; set; } = 0;
        public int Discount { get; set; } = 0;
        public int AmountDue { get; set; } = 0;

        public Order CurrentOrder { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } 

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentWindow"/> class.
        /// Sets the order details and updates the UI with the total amount, discount, and amount due.
        /// </summary>
        /// <param name="order">The order object for which payment is being processed.</param>
        /// <returns>A new instance of the PaymentWindow.</returns>
        public PaymentWindow(Order order)
        {
            this.InitializeComponent();
            this.TotalAmount = (int)order.TotalPrice;
            this.Discount = (int)order.Discount;
            this.AmountDue = this.TotalAmount - this.Discount;
            this.CurrentOrder = order;

            totalAmount.Text = TotalAmount.ToString();
            discount.Text = Discount.ToString();
            amountDue.Text = AmountDue.ToString();
            if (AmountDue <= 0)
            {
                amountDue.Text = "0";
            }
            else
            {
                amountDue.Text = AmountDue.ToString();
            }
        }

        /// <summary>
        /// Handles the TextChanged event for the Amount Paid TextBox.
        /// Calculates and displays the change based on the amount paid and the amount due.
        /// Displays a message if the amount paid is insufficient.
        /// </summary>
        /// <param name="sender">The source of the event, the Amount Paid TextBox.</param>
        /// <param name="e">Event data.</param>
        /// <returns>This method does not return a value.</returns>        
        private void AmountPaidTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(amountPaid.Text, out int amountPaidValue))
            {
                int changeValue = amountPaidValue - AmountDue;
                if (changeValue < 0)
                {
                    change.Text = "Không đủ số tiền thanh toán";
                }
                else
                {
                    change.Text = changeValue.ToString();
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Handles the click event for the Pay Button.
        /// Validates the amount paid, updates the order status to paid if sufficient,
        /// saves the updated order to the database, displays a success or error dialog,
        /// and closes the window upon successful payment.
        /// </summary>
        /// <param name="sender">The source of the event, the Pay Button.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(amountPaid.Text, out int amountPaidValue))
            {
                if (amountPaidValue < AmountDue)
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Thông báo",
                        Content = "Số tiền khách trả ít hơn số tiền cần trả",
                        CloseButtonText = "OK"
                    };
                    dialog.XamlRoot = this.Content.XamlRoot;
                    await dialog.ShowAsync();
                }
                else
                {
                    CurrentOrder.IsPaid = true;

                    var dao = Services.Services.GetKeyedSingleton<IDao>();
                    dao.Orders.Update(CurrentOrder);

                    // Notify that the payment is successful
                    var dialog = new ContentDialog
                    {
                        Title = "Thông báo",
                        Content = "Thanh toán thành công",
                        CloseButtonText = "OK"
                    };
                    dialog.XamlRoot = this.Content.XamlRoot;
                    await dialog.ShowAsync();

                    this.Close();
                }
            }
            else
            {
                var dialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Số tiền khách trả phải là một số nguyên",
                    CloseButtonText = "OK"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
            }
        }

        /// <summary>
        /// Handles the click event for showing the Bank QR code.
        /// Finds the default bank payment method and opens the ShowBankQRWindow with the amount due.
        /// Displays an error message if no default bank payment method is found.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async void ShowBankQR(object sender, RoutedEventArgs e)
        {
            // Find the default bank payment method
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            var defaultBankPaymentMethod = dao.PaymentMethods.GetAll().FirstOrDefault(m => m.IsDefault && m.Type == "bank");
            if(defaultBankPaymentMethod != null)
            {
                // Show the QR code for the default bank payment method
                var qrCodeWindow = new ShowBankQRWindow(defaultBankPaymentMethod, AmountDue);
                qrCodeWindow.Activate();
            }
            else
            {
                var dialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Không tìm thấy phương thức thanh toán ngân hàng mặc định.",
                    CloseButtonText = "OK"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
            }
        }

        /// <summary>
        /// Handles the click event for showing the MoMo QR code.
        /// Finds the default MoMo payment method and opens the ShowMomoQRWindow with the amount due.
        /// Displays an error message if no default MoMo payment method is found.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async void ShowMomoQR(object sender, RoutedEventArgs e)
        {
            // Find the default bank payment method
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            var defaultMomoPaymentMethod = dao.PaymentMethods.GetAll().FirstOrDefault(m => m.IsDefault && m.Type == "momo");
            if (defaultMomoPaymentMethod != null)
            {
                // Show the QR code for the default bank payment method
                var qrCodeWindow = new ShowMomoQRWindow(defaultMomoPaymentMethod, AmountDue);
                qrCodeWindow.Activate();
            }
            else
            {
                var dialog = new ContentDialog
                {
                    Title = "Lỗi",
                    Content = "Không tìm thấy phương thức thanh toán Momo mặc định.",
                    CloseButtonText = "OK"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
            }
        }
    }
}
