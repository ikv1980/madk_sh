﻿using System.Text.Json;
using System.Windows;
using Project.Models;

namespace Project.Tools
{
    internal class Global
    {
        public static User CurrentUser { get; set; }
        public static UserPermissions ParsedPermissions { get; private set; }

        // Разбор JSON (поле `users`.`users_permission`)
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
                        MessageBox.Show("JSON разобран, но вкладки отсутствуют.", "Отладка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }

                    // Проверка на наличие справочников
                    if (ParsedPermissions?.Directoryes == null || ParsedPermissions.Directoryes.Count == 0)
                    {
                        MessageBox.Show("JSON разобран, но справочники отсутствуют.", "Отладка",
                            MessageBoxButton.OK,
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

        // Вкладки - проверка прав на запись 
        public static bool GetWritePermissionForTab(string tabName)
        {
            if (ParsedPermissions == null)
            {
                MessageBox.Show("Права пользователя отсутствуют.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            var tabPermission = ParsedPermissions.Tabs.FirstOrDefault(tab => tab.Name == tabName);

            if (tabPermission != null)
            {
                return tabPermission.Permissions.Write;
            }

            MessageBox.Show($"Вкладка с именем \"{tabName}\" не найдена.", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return false;
        }

        // Справочники - проверка прав на запись
        public static bool GetWritePermissionForDict(string dictName)
        {
            // справочники - исключения
            if(dictName == "Order") return true;
            
            if (ParsedPermissions == null)
            {
                MessageBox.Show("Права пользователя отсутствуют.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            var tabPermission = ParsedPermissions.Directoryes.FirstOrDefault(dict => dict.Name == dictName);

            if (tabPermission != null)
            {
                return tabPermission.Permissions.Write;
            }

            MessageBox.Show($"Справочник с именем \"{dictName}\" не найден.", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return false;
        }
    }
}