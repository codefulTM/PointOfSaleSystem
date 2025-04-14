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
using System.Data;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RevenuePage : Page
    {
        public RevenuePage()
        {
            this.InitializeComponent();
        }

        private void DailyRevenueDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            // Get all orders
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            var orders = dao.Orders.GetAll();

            // Get the current date
            DateTimeOffset? selectedDate = weeklyRevenueDatePicker.Date;
            if(selectedDate.HasValue)
            {
                DateTime date = selectedDate.Value.Date;
                // Filter orders by date
                var filteredOrders = orders.Where(order => order.OrderTime.Date == date).ToList();
                // Calculate total revenue
                int totalRevenue = filteredOrders.Sum(order => order.TotalPrice - order.Discount >= 0 ? order.TotalPrice - order.Discount : 0);
                // Display total revenue
                dailyTotalRevenue.Text = totalRevenue.ToString();
            }
        }

        private void WeeklyRevenueDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            // Get all orders
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            var orders = dao.Orders.GetAll();

            // Get the current date
            DateTimeOffset? selectedDate = weeklyRevenueDatePicker.Date;
            if(selectedDate.HasValue)
            {
                DateTime date = selectedDate.Value.Date;
                // Filter orders by date
                var filteredOrders = orders.Where(order => order.OrderTime.Date >= date && order.OrderTime.Date < date.AddDays(7)).ToList();
                // Calculate total revenue
                int totalRevenue = filteredOrders.Sum(order => order.TotalPrice - order.Discount >= 0 ? order.TotalPrice - order.Discount : 0);
                // Display total revenue
                weeklyTotalRevenue.Text = totalRevenue.ToString();
            }
        }

        private void MonthlyRevenueDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            // Get all orders
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            var orders = dao.Orders.GetAll();

            // Get the current date
            DateTimeOffset? selectedDate = weeklyRevenueDatePicker.Date;
            if (selectedDate.HasValue)
            {
                DateTime date = selectedDate.Value.Date;
                // Filter orders by date
                var filteredOrders = orders.Where(order => order.OrderTime.Date >= date && order.OrderTime.Date < date.AddDays(30)).ToList();
                // Calculate total revenue
                int totalRevenue = filteredOrders.Sum(order => order.TotalPrice - order.Discount >= 0 ? order.TotalPrice - order.Discount : 0);
                // Display total revenue
                monthlyTotalRevenue.Text = totalRevenue.ToString();
            }
        }
    }
}
