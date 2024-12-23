using System.Collections.Generic;

namespace Project.Tools
{
    // Права по умолчанию
    public class DefaultPermissions
    {
        public static string User { get; } = @"{
            ""Tabs"": [
                { ""Name"": ""user"", ""RusName"": ""Пользователь"", ""Permissions"": { ""Read"": true, ""Write"": true } },
                { ""Name"": ""order"", ""RusName"": ""Заказы"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""dict"", ""RusName"": ""Словари"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""setting"", ""RusName"": ""Настройки"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""report"", ""RusName"": ""Отчеты"", ""Permissions"": { ""Read"": false, ""Write"": false } }
            ],
            ""Directoryes"": [
                { ""Name"": ""Orders"", ""RusName"": ""Заказы"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""Cars"", ""RusName"": ""Автомобили"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""Users"", ""RusName"": ""Пользователи"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""carsCountry"", ""RusName"": ""Страны"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""carsMark"", ""RusName"": ""Марки"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""carsModel"", ""RusName"": ""Модели"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""carsType"", ""RusName"": ""Типы"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""carsColor"", ""RusName"": ""Цвета"", ""Permissions"": { ""Read"": false, ""Write"": false } }
            ]
        }";
    }
    
    public class UserPermissions
    {
        public List<TabPermission> Tabs { get; set; }
        public List<DirectoryPermission> Directoryes { get; set; }
    }

    public class TabPermission
    {
        public string Name { get; set; }
        public string RusName { get; set; }
        public Permission Permissions { get; set; }
    }

    public class DirectoryPermission
    {
        public string Name { get; set; }
        public string RusName { get; set; }
        public Permission Permissions { get; set; }
    }

    public class Permission
    {
        public bool Read { get; set; }
        public bool Write { get; set; }
    }
}