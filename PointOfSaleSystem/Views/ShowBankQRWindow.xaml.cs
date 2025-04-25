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
using Microsoft.UI.Xaml.Media.Imaging;
using QRCoder;
using Windows.Storage.Streams;
using System.Security.Principal;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// A window that displays a bank QR code for payment based on the provided payment method and amount.
    /// </summary>
    public sealed partial class ShowBankQRWindow : Window
    {
        public PaymentMethod PaymentMethodInfo { get; set; }
        public int Amount { get; set; } = 0;
        public Dictionary<string, string> BankCodes = new Dictionary<string, string>
        {
            { "Vietcombank", "VCB" },
            { "VietinBank", "CTG" },
            { "BIDV", "BID" },
            { "Techcombank", "TCB" },
            { "MB Bank", "MBB" },
            { "ACB", "ACB" },
            { "VPBank", "VPB" },
            { "HDBank", "HDB" },
            { "Sacombank", "STB" },
            { "SHB", "SHB" },
            { "Eximbank", "EIB" },
            { "TPBank", "TPB" },
            { "SeABank", "SEAB" },
            { "VIB", "VIB" },
            { "OCB", "OCB" },
            { "ABBANK", "ABB" },
            { "SCB", "SCB" },
            { "Bac A Bank", "BAB" },
            { "PVcomBank", "PVC" },
            { "Nam A Bank", "NAB" }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowBankQRWindow"/> class.
        /// Generates and displays a bank QR code based on the provided payment method and amount.
        /// </summary>
        /// <param name="paymentMethod">The payment method containing bank details (BankName, AccountNumber).</param>
        /// <param name="amount">The amount to be included in the QR code for payment.</param>
        /// <returns>A new instance of the ShowBankQRWindow.</returns>
        public ShowBankQRWindow(PaymentMethod paymentMethod, int amount)
        {
            this.PaymentMethodInfo = paymentMethod;
            this.Amount = amount;
            this.InitializeComponent();

            // Generate the QR code
            if(paymentMethod.Type != "bank")
            {
                return;
            }
            string qrUrl = $"https://img.vietqr.io/image/{BankCodes[paymentMethod.BankName]}-{paymentMethod.AccountNumber}-compact2.png?amount={amount}&addInfo={Uri.EscapeDataString("Thanh toan hoa don")}";
            qrCodeImage.Source = new BitmapImage(new Uri(qrUrl));
        } 
    }
}
