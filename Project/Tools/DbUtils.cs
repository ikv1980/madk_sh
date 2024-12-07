using System.Windows;
using Project.Models;
using MessageBox = System.Windows.MessageBox;
using LinqExpression = System.Linq.Expressions.Expression;

namespace Project.Tools
{
    internal static class DbUtils
    {
        public static Db db;

        static DbUtils()
        {
            try
            {
                db = new Db();
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка подключения к БД\n${e}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        //  Уровни доступа в программе (users.users_permission)
        // В принципе можно не устанавливать тут, а просто сразу проверять на странице авторизации
        public static class Roles
        {
            public static string admin = "1";

            public static string manager = "2";

            public static string newuser = "3";
        }
        
        // Получить все значения из таблицы
        public static List<T> GetTableAllValues<T>() where T : class
        {
            return db.Set<T>().ToList();
        }
        
        // Поиск в таблице
        public static List<T> GetSearchingValues<T>(string searchText) where T : class
        {
            var parameter = LinqExpression.Parameter(typeof(T), "e");
            var stringProperties = typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string));

            if (!stringProperties.Any())
                return new List<T>();

            LinqExpression orExpression = null;

            foreach (var prop in stringProperties)
            {
                var propertyExpression = LinqExpression.Property(parameter, prop);
                var likeExpression = LinqExpression.Call(
                    propertyExpression,
                    nameof(string.Contains),
                    Type.EmptyTypes,
                    LinqExpression.Constant(searchText, typeof(string))
                );

                orExpression = orExpression == null
                    ? likeExpression
                    : LinqExpression.OrElse(orExpression, likeExpression);
            }

            var lambda = LinqExpression.Lambda<Func<T, bool>>(orExpression, parameter);
            return db.Set<T>().Where(lambda).ToList();
        }
        
        public static IEnumerable<object> GetTableAllValuesByName(string tableName)
        {
            using (var context = new Db())
            {
                var dbSet = context.GetType().GetProperty(tableName)?.GetValue(context) as IQueryable;
                return dbSet?.Cast<object>().ToList() ?? Enumerable.Empty<object>();
            }
        }
        
        public static int GetTableCount<TTable>() where TTable : class
        {
            // Здесь можно использовать Entity Framework для подсчета всех записей в таблице
            using (var context = new Db())
            {
                return context.Set<TTable>().Count();
            }
        }
        
        public static List<TTable> GetTablePagedValues<TTable>(int page, int pageSize) where TTable : class
        {
            // Получение данных для указанной страницы
            using (var context = new Db())
            {
                return context.Set<TTable>()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
        }
    }
}

