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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// A window that displays the details of a specific customer.
    /// </summary>
    public sealed partial class CustomerDetailsWindow : Window
    {
        private Customer? _customer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerDetailsWindow"/> class.
        /// </summary>
        /// <param name="customer">The customer object whose details are to be displayed.</param>
        /// <returns>A new instance of the CustomerDetailsWindow.</returns>
        public CustomerDetailsWindow(Customer customer)
        {
            this.InitializeComponent();
            _customer = customer;
            customerInfo.DataContext = _customer;
        }
    }
}
