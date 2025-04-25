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
using PointOfSaleSystem.Views.ViewModels;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Kernel.Sketches;
using System.Collections.Specialized;
using System.ComponentModel;
using PointOfSaleSystem.Views.Converters;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PointOfSaleSystem.Views
{
    /// <summary>
    /// A page that displays revenue information, including daily, weekly, and monthly revenue summaries and charts.
    /// </summary>
    public sealed partial class RevenuePage : Page
    {
        public ChartViewModel WeeklyChartViewModel { get; set; } = new ChartViewModel();
        public ChartViewModel MonthlyChartViewModel { get; set; } = new ChartViewModel();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RevenuePage"/> class.
        /// </summary>
        /// <returns>A new instance of the RevenuePage.</returns>        
        public RevenuePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the DateChanged event for the daily revenue DatePicker.
        /// Filters orders by the selected date, calculates the total revenue for that day,
        /// and displays the formatted total revenue.
        /// </summary>
        /// <param name="sender">The source of the event, the dailyRevenueDatePicker.</param>
        /// <param name="e">Event data containing the new date value.</param>
        /// <returns>This method does not return a value.</returns>
        private void DailyRevenueDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            // Get all orders
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            var orders = dao.Orders.GetAll();

            // Get the current date
            DateTimeOffset? selectedDate = dailyRevenueDatePicker.Date;
            if(selectedDate.HasValue)
            {
                DateTime date = selectedDate.Value.Date;
                // Filter orders by date
                var filteredOrders = orders.Where(order => order.OrderTime?.Date == date).ToList();
                // Calculate total revenue
                int totalRevenue = (int)filteredOrders.Sum(order => order.TotalPrice - order.Discount >= 0 ? order.TotalPrice - order.Discount : 0);
                // Display total revenue
                var formatter = new CurrencyFormatter();
                string? formattedRevenue = formatter.Convert(totalRevenue, null, null, "vi-VN") as string;
                dailyTotalRevenue.Text = formattedRevenue;
            }
        }

        /// <summary>
        /// Handles the DateChanged event for the weekly revenue DatePicker.
        /// Filters orders for the week starting from the selected date, updates the weekly chart ViewModel,
        /// calculates the total revenue for that week, and displays the formatted total revenue.
        /// </summary>
        /// <param name="sender">The source of the event, the weeklyRevenueDatePicker.</param>
        /// <param name="e">Event data containing the new date value.</param>
        /// <returns>This method does not return a value.</returns>
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
                var filteredOrders = orders.Where(order => order.OrderTime?.Date >= date && order.OrderTime?.Date < date.AddDays(7)).ToList();
                //// Testing
                //testBlock.Text = date.ToString();
                // Initialize weekly chart view model
                WeeklyChartViewModel.UpdateSeriesAndLabels(filteredOrders);
                // Calculate total revenue
                int totalRevenue = (int)filteredOrders.Sum(order => order.TotalPrice - order.Discount >= 0 ? order.TotalPrice - order.Discount : 0);
                // Display total revenue
                var formatter = new CurrencyFormatter();
                string? formattedRevenue = formatter.Convert(totalRevenue, null, null, "vi-VN") as string;
                weeklyTotalRevenue.Text = formattedRevenue;
            }
        }

        /// <summary>
        /// Handles the DateChanged event for the monthly revenue DatePicker.
        /// Filters orders for the month starting from the selected date, updates the monthly chart ViewModel,
        /// calculates the total revenue for that month, and displays the formatted total revenue.
        /// </summary>
        /// <param name="sender">The source of the event, the monthlyRevenueDatePicker.</param>
        /// <param name="e">Event data containing the new date value.</param>
        /// <returns>This method does not return a value.</returns>
        private void MonthlyRevenueDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            // Get all orders
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            var orders = dao.Orders.GetAll();

            // Get the current date
            DateTimeOffset? selectedDate = monthlyRevenueDatePicker.Date;
            if (selectedDate.HasValue)
            {
                DateTime date = selectedDate.Value.Date;
                // Filter orders by date
                var filteredOrders = orders.Where(order => order.OrderTime?.Date >= date && order.OrderTime?.Date < date.AddDays(30)).ToList();
                // Initialize monthly chart view model
                MonthlyChartViewModel.UpdateSeriesAndLabels(filteredOrders);
                // Calculate total revenue
                int totalRevenue = (int)filteredOrders.Sum(order => order.TotalPrice - order.Discount >= 0 ? order.TotalPrice - order.Discount : 0);
                // Display total revenue
                var formatter = new CurrencyFormatter();
                string? formattedRevenue = formatter.Convert(totalRevenue, null, null, "vi-VN") as string;
                monthlyTotalRevenue.Text = formattedRevenue;
            }
        }
    }
}
