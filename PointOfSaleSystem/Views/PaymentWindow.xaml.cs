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
using PointOfSaleSystem.Services;
using PointOfSaleSystem.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PaymentWindow : Window
    {
        public int TotalAmount { get; set; } = 0;
        public int Discount { get; set; } = 0;
        public int AmountDue { get; set; } = 0;

        public Order CurrentOrder { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } 

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
                    await dialog.ShowAsync();
                }
                else
                {
                    CurrentOrder.IsPaid = true;

                    var dao = Services.Services.GetKeyedSingleton<IDao>();
                    dao.Orders.Create(CurrentOrder);
                    foreach (var orderDetail in OrderDetails)
                    {
                        dao.OrderDetails.Create(orderDetail);
                    }

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
                await dialog.ShowAsync();
            }
        }

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
                await dialog.ShowAsync();
            }
        }

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
                await dialog.ShowAsync();
            }
        }
    }
}
