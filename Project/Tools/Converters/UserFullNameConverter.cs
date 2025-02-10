using System;
using System.Globalization;
using System.Windows.Data;
using Project.Models;

namespace Project.Tools
{
    public class UserFullNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is User user)
            {
                return $"{user.UsersSurname} {user.UsersName}";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}