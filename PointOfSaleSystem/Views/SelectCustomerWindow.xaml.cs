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
    /// A window for selecting a customer.
    /// </summary>
    public sealed partial class SelectCustomerWindow : Window
    {
        public CustomerViewModel CustomerViewModel { get; set; }
        public Customer? SelectedCustomer { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <c>SelectCustomerWindow</c> class.
        /// </summary>
        /// <param name="viewModel">The customer view model.</param>
        public SelectCustomerWindow(CustomerViewModel viewModel)
        {
            this.InitializeComponent();
            CustomerViewModel = viewModel;
        }

        /// <summary>
        /// Handles the click event of the select button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void SelectCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCustomer != null)
            {
                this.Close(); // Đóng cửa sổ và trả về khách hàng đã chọn
            }
        }

        /// <summary>
        /// Handles the click event of the cancel button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedCustomer = null; // Không chọn khách hàng nào
            this.Close();
        }

        /// <summary>
        /// Handles the click event of the add customer button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            // Mở cửa sổ AddCustomerWindow
            var addCustomerWindow = new AddCustomerWindow(CustomerViewModel);
            addCustomerWindow.Activate();
        }
    }
}
