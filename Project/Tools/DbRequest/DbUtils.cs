using System.Windows;
using Project.Models;
using MessageBox = System.Windows.MessageBox;

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
        
        public static List<T> GetTableAllValues<T>() where T : class
        {
            return db.Set<T>().ToList();
        }
        
        public static List<T> GetSearchingValues<T>(string searchText) where T : class
        {
            return db.Set<T>().ToList().Where(p => p.ToString().ToLower().Contains(searchText.ToLower())).ToList();
        }
        
        public static IEnumerable<object> GetTableAllValuesByName(string tableName)
        {
            using (var context = new Db())
            {
                var dbSet = context.GetType().GetProperty(tableName)?.GetValue(context) as IQueryable;
                return dbSet?.Cast<object>().ToList() ?? Enumerable.Empty<object>();
            }
        }
    }
}

