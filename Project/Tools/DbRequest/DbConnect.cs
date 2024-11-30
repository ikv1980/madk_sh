using System.Windows;
using Project.Models;
using MessageBox = System.Windows.MessageBox;

namespace Project.Tools
{
    internal static class DbConnect
    {
        public static Db db;

        static DbConnect()
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
    }
}

