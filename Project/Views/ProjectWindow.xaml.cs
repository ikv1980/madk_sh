using System.Windows;
using System.Windows.Controls;
using Project.Models;
using Project.Views.Pages;

namespace Project.Views
{
    public partial class ProjectWindow : Window
    {
        public ProjectWindow(User user)
        {
            InitializeComponent();
            // Устанавливаем начальную страницу
            UserContent.Content = new UserPage();
        }

        private void TabControl_Select(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem is TabItem selectedTab)
            {
                string tag = selectedTab.Tag?.ToString();

                // Переключение содержимого на основе тега вкладки
                switch (tag)
                {
                    case "UserPage":
                        UserContent.Content = new UserPage();
                        break;
                    case "OrdersPage":
                        OrdersContent.Content = new OrdersPage();
                        break;
                    case "DictionaryPage":
                        DictionaryContent.Content = new DictionaryPage();
                        break;
                    case "SettingsPage":
                        SettingsContent.Content = new SettingsPage();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
