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

        private void Bank_SetDefaultClick(object sender, RoutedEventArgs e)
        {

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

        private void Momo_SetDefaultClick(object sender, RoutedEventArgs e)
        {

        }

        private void Momo_AddPaymentMethodClick(object sender, RoutedEventArgs e)
        {

        }

        private void Momo_RemovePaymentMethodClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
