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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// The main window of the Point of Sale system application.
    /// It hosts the navigation view and displays different pages within a frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// Sets the initial page displayed in the content frame to ProductPage.
        /// </summary>
        /// <returns>A new instance of the MainWindow.</returns>
        public MainWindow()
        {
            this.InitializeComponent();
            contentFrame.Navigate(typeof(ProductPage));
        }

        /// <summary>
        /// Handles the ItemInvoked event of the NavigationView.
        /// Navigates to the corresponding page based on the invoked navigation item's tag.
        /// If the settings item is invoked, it prints a message to the console.
        /// </summary>
        /// <param name="sender">The source of the event, the NavigationView.</param>
        /// <param name="args">Event data that provides information about the invoked item.</param>
        /// <returns>This method does not return a value.</returns>
        private void nav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                Console.WriteLine("Settings clicked");
                return;
            }
            else
            {
                var item = (NavigationViewItem)args.InvokedItemContainer;
                var page = item.Tag.ToString();

                contentFrame.Navigate(Type.GetType($"PointOfSaleSystem.Views.{page}"));
            }
        }
    }
}
