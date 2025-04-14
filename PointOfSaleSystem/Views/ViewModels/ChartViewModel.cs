using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using PointOfSaleSystem.Models;

namespace PointOfSaleSystem.Views.ViewModels
{
    public class ChartViewModel
    {
        public ISeries[] Series { get; set; }

        public string[] Labels { get; set; }

        public ChartViewModel(List<Order> ordersList)
        {
            // Get revenues by date
            var revenuesByDate = ordersList.GroupBy(o => o.OrderTime.Date)
                                            .Select(g => new
                                            {
                                                Date = g.Key,
                                                Revenue = g.Sum(o => o.TotalPrice - o.Discount >= 0 ? o.TotalPrice - o.Discount : 0)
                                            });
            // Create an array of labels
            var labels = revenuesByDate.Select(r => r.Date.ToString("dd/MM")).ToArray();

            // Create an array of revenues
            var revenues = revenuesByDate.Select(r => r.Revenue).ToArray();

            Series = new ISeries[]
            {
                new LineSeries<int>
                {
                    Values = revenues,
                    Name = "Doanh thu theo ngày"
                }
            };

            // Set the labels for the X-axis
            Labels = labels;
        }
    }
}
