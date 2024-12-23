using System.Text.Json;
using System.Windows;
using Project.Models;

namespace Project.Tools
{
    internal class Global
    {
        public static User CurrentUser { get; set; }
        public static UserPermissions ParsedPermissions { get; private set; }

        public static void ParsePermissions(User user)
        {
            if (user == null)
            {
                MessageBox.Show("Пользователь не указан.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrWhiteSpace(user.UsersPermissions))
            {
                try
                {
                    ParsedPermissions = JsonSerializer.Deserialize<UserPermissions>(user.UsersPermissions);

                    // Проверка на наличие вкладок
                    if (ParsedPermissions?.Tabs == null || ParsedPermissions.Tabs.Count == 0)
                    {
                        MessageBox.Show("JSON разобран, но вкладки отсутствуют.", "Отладка", MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }

                    // Проверка на наличие справочников
                    if (ParsedPermissions?.Directoryes == null || ParsedPermissions.Directoryes.Count == 0)
                    {
                        MessageBox.Show("JSON разобран, но справочники отсутствуют.", "Отладка", MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    ParsedPermissions = new UserPermissions
                    {
                        Tabs = new List<TabPermission>(),
                        Directoryes = new List<DirectoryPermission>()
                    };
                    MessageBox.Show($"Ошибка парсинга JSON: {ex.Message}\nJSON: {user.UsersPermissions}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("UsersPermissions пусто или отсутствует.", "Отладка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ParsedPermissions = new UserPermissions
                {
                    Tabs = new List<TabPermission>()
                };
            }
        }
    }
}