using Project.Models;

namespace Project.Tools
{
    // настройка для сортировки в справочниках
    public static class Config
    {
        public static readonly Dictionary<Type, string> DefaultSortProperties = new Dictionary<Type, string>
        {
            { typeof(UsersDepartment), nameof(UsersDepartment.DepartmentName) },
            { typeof(UsersFunction), nameof(UsersFunction.FunctionName) },
            { typeof(UsersStatus), nameof(UsersStatus.StatusName) },
            { typeof(CarsColor), nameof(CarsColor.ColorName) },
        };
    }

    // настройка для поиска в связанных таблицах
    public static class SearchConfig
    {
        public static readonly Dictionary<Type, List<string>> SearchNavigationProperties =
            new Dictionary<Type, List<string>>
            {
                {
                    typeof(Car),
                    new List<string>
                    {
                        "CarColorNavigation.ColorName",
                        "CarCountryNavigation.CountryName",
                        "CarMarkNavigation.MarkName",
                        "CarModelNavigation.ModelName",
                        "CarTypeNavigation.TypeName",
                    }
                },
                {
                    typeof(MmMarkModel),
                    new List<string>
                    {
                        "Mark.MarkName",
                        "Model.ModelName",
                        "Country.CountryName",
                    }
                },
                {
                    typeof(User),
                    new List<string>
                    {
                        "UsersDepartmentNavigation.DepartmentName",
                        "UsersFunctionNavigation.FunctionName",
                        "UsersStatusNavigation.StatusName",
                    }
                },
                {
                    typeof(MmDepartmentFunction),
                    new List<string>
                    {
                        "Department.DepartmentName",
                        "Function.FunctionName",
                    }
                },
                {
                    typeof(Order),
                    new List<string>
                    {
                        "OrdersClientNavigation.ClientName",
                        "OrdersDeliveryNavigation.DeliveryName",
                        "OrdersPaymentNavigation.PaymentName",
                        "OrdersUserNavigation.UsersName",
                        "OrdersUserNavigation.UsersSurname",
                    }
                },
            };
    }
}