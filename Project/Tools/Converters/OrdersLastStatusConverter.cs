using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Project.Models;

// Находим последний статус заказа
namespace Project.Tools
{
    public class OrderLastStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int orderId)
            {
                using (var db = new Db())
                {
                    var lastStatus = db.MmOrdersStatuses
                        .Where(s => s.OrderId == orderId)
                        .OrderByDescending(s => s.Date)
                        .Select(s => s.Status)
                        .FirstOrDefault();

                    return lastStatus?.OrderStatusName ?? "Нет статуса";
                }
            }

            return "Нет статуса"; // Возвращаем значение по умолчанию
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}