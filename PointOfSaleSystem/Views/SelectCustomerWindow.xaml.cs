using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PointOfSaleSystem.Models;
using PointOfSaleSystem.Views.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectCustomerWindow : Window
    {
        public CustomerViewModel CustomerViewModel { get; set; }
        public Customer? SelectedCustomer { get; private set; }

        public SelectCustomerWindow(CustomerViewModel viewModel)
        {
            this.InitializeComponent();
            CustomerViewModel = viewModel;
        }

        private void SelectCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCustomer != null)
            {
                this.Close(); // Đóng cửa sổ và trả về khách hàng đã chọn
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedCustomer = null; // Không chọn khách hàng nào
            this.Close();
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            // Mở cửa sổ AddCustomerWindow
            var addCustomerWindow = new AddCustomerWindow(CustomerViewModel);
            addCustomerWindow.Activate();
        }
    }
}
