using Project.Models;
using Wpf.Ui.Controls;

namespace Project.Tools
{
    static class DbUtils
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
                MessageBox box = new MessageBox();
                box.Title = "Ошибка";
                box.Content = "Ошибка подключения к БД";
                box.Show();
            }
        }
    }
}

