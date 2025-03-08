using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace PointOfSaleSystem.Views.Converters
{
    class CurrencyFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int amount = (int)value;
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            return amount.ToString("#,### đ", cul);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
