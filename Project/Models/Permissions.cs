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
                { ""Name"": ""Order"", ""RusName"": ""Заказы"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""OrdersClient"", ""RusName"": ""Клиенты"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""OrdersDelivery"", ""RusName"": ""Доставки"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""OrdersPayment"", ""RusName"": ""Оплаты"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""OrdersStatus"", ""RusName"": ""Статусы"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""CarsCountry"", ""RusName"": ""Страны"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""CarsMark"", ""RusName"": ""Марки"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""CarsModel"", ""RusName"": ""Модели"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""CarsType"", ""RusName"": ""Типы"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""CarsColor"", ""RusName"": ""Цвета"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""MmMarkModel"", ""RusName"": ""Марка-Модель"", ""Permissions"": { ""Read"": false, ""Write"": false } },
                { ""Name"": ""MmMarkModel"", ""RusName"": ""Страна-Модель"", ""Permissions"": { ""Read"": false, ""Write"": false } }
                { ""Name"": ""Users"", ""RusName"": ""Сотрудники"", ""Permissions"": { ""Read"": false, ""Write"": false } }
                { ""Name"": ""UsersDepartment"", ""RusName"": ""Отделы"", ""Permissions"": { ""Read"": false, ""Write"": false } }
                { ""Name"": ""UsersFunction"", ""RusName"": ""Должности"", ""Permissions"": { ""Read"": false, ""Write"": false } }
                { ""Name"": ""UsersStatus"", ""RusName"": ""Статусы сотрудников"", ""Permissions"": { ""Read"": false, ""Write"": false } }
            ]
        }";
    }

    public class UserPermissions
    {
        public List<TabPermission> Tabs { get; set; }
        public List<DirectoryPermission> Directoryes { get; set; }
    }

    // Вкладки
    public class TabPermission
    {
        public string Name { get; set; }
        public string RusName { get; set; }
        public Permission Permissions { get; set; }
    }

    // Справочники
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