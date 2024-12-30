using Project.Models;

namespace Project.Tools
{
    // поля для сортировки в справочниках
    public static class SortConfig
    {
        public static readonly Dictionary<Type, string> DefaultSortProperties = new Dictionary<Type, string>
        {
            { typeof(UsersDepartment), nameof(UsersDepartment.DepartmentName) },
            { typeof(UsersFunction), nameof(UsersFunction.FunctionName) },
            { typeof(UsersStatus), nameof(UsersStatus.StatusName)},
            { typeof(CarsColor), nameof(CarsColor.ColorName)},
        };
    }
}