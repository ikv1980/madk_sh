using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows;

namespace Project.Tools
{
    class ValidateField
    {
        // Метод для проверки полей
        public bool Validate(string value, string key, string title = "Ошибка данных")
        {
            var flag = false;

            switch (key)
            {
                // Проверка пароля
                case "password":
                    if (value.Length < 6)
                    {
                        MessageBox.Show("Пароль должен содержать не менее 6 символов.", title, MessageBoxButton.OK, MessageBoxImage.Warning);
                        flag = true;
                    }
                    break;
                // Проверка e-mail
                case "email":
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            new MailAddress(value);
                        }
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Некорректный e-mail.", title, MessageBoxButton.OK, MessageBoxImage.Error);
                        flag = true;
                    }
                    break;
                // Проверка телефона
                case "phone":
                    var regex = new Regex(@"^(\+7|8)?\d{10}$");
                    if (string.IsNullOrWhiteSpace(value) || !regex.IsMatch(value))
                    {
                        MessageBox.Show("Некорректный телефон.\nВ номере телефона допускаются цифры и знак +", title, MessageBoxButton.OK, MessageBoxImage.Error);
                        flag = true;
                    }
                    break;
                default:
                    break;
            }
            return flag;
        }
    }
}
