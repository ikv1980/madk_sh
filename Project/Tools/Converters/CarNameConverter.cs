using System;
using System.Globalization;
using System.Windows.Data;
using Project.Models;

namespace Project.Tools
{
    public class CarNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Car car)
            {
                return $"{car.CarMarkNavigation.MarkName} {car.CarModelNavigation.ModelName} ({car.CarColorNavigation.ColorName}, {car.CarDate}) ";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}