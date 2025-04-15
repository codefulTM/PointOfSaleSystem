using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using PointOfSaleSystem.Models;

namespace PointOfSaleSystem.Views.ViewModels
{
    public class ChartViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<int?> ValuesCollection { get; set; } = new ObservableCollection<int?>();
        public ISeries[] Series { get; set; } 
        public IEnumerable<ICartesianAxis> XAxes { get; set; }
        public IEnumerable<ICartesianAxis> YAxes { get; set; }

        public ChartViewModel()
        {
            // Initialize the chart with empty values
            Series = new ISeries[]
            {
                new LineSeries<int?>
                {
                    Values = ValuesCollection,
                    Name = "Doanh thu theo ngày"
                }
            };
            XAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Ngày",
                    Labels = Array.Empty<string>()
                }
            }.Cast<ICartesianAxis>();
            YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Doanh thu"
                }
            }.Cast<ICartesianAxis>();
        }

        public void UpdateSeriesAndLabels(List<Order> ordersList)
        {
            // Get revenues by date
            var revenuesByDate = ordersList.GroupBy(o => o.OrderTime?.Date)
                                            .Select(g => new
                                            {
                                                Date = g.Key,
                                                Revenue = g.Sum(o => Math.Max((o.TotalPrice ?? 0) - (o.Discount ?? 0), 0))
                                            })
                                            .OrderBy(g => g.Date);
            // Create an array of labels
            var labels = revenuesByDate.Select(r => r.Date.ToString()).ToArray();

            // Create an array of revenues
            var revenues = revenuesByDate.Select(r => r.Revenue).ToArray();

            // Update Series' values collection
            ValuesCollection.Clear();
            foreach (var revenue in revenues)
            {
                ValuesCollection.Add(revenue);
            }

            // Update the label
            XAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Ngày",
                    Labels = labels
                }
            }.Cast<ICartesianAxis>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
