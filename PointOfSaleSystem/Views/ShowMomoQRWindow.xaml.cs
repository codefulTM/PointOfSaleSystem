using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using PointOfSaleSystem.Models;
using QRCoder;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShowMomoQRWindow : Window
    {
        public PaymentMethod PaymentMethodInfo { get; set; }
        public int Amount { get; set; } = 0;
        public ShowMomoQRWindow(PaymentMethod paymentMethod, int amount)
        {
            this.PaymentMethodInfo = paymentMethod;
            this.Amount = amount;
            this.InitializeComponent();

            if(paymentMethod.Type != "momo")
            {
                return;
            }
            // Generate the QR code
            string qrCodeData = $"2|99|{paymentMethod.PhoneNumber}|||transfer_myqr|{amount}|Thanh toan hoa don";
            // Create QR code image using qrCodeData
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeDataObj = qrGenerator.CreateQrCode(qrCodeData, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeDataObj);
            // Convert QR code to bitmap
            var qrImage = qrCode.GetGraphic(20);
            // Set the image source to the QR code bitmap
            using (var stream = new InMemoryRandomAccessStream())
            {
                qrImage.Save(stream.AsStreamForWrite(), System.Drawing.Imaging.ImageFormat.Png);
                stream.Seek(0);
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(stream);
                qrCodeImage.Source = bitmapImage;
            }
        }
    }
}
