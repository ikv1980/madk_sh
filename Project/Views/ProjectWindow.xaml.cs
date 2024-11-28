using System.Windows;
using System.Windows.Controls;
using Project.Models;
using Project.Tools;
using Project.Views.Pages;

namespace Project.Views
{
    public partial class ProjectWindow : Window
    {
        public ProjectWindow(User user)
        {
            InitializeComponent();
            Global.CurrentUser = user;
            this.Loaded += change_Screeen;
            // Можно добавить доп.инфу (ФИО, кнопку выхода, логотип)
            MainTabControl.SelectedIndex = 1;
            MainContent.Content = new UserPage();
            SetAccess(user);
        }
        
        // Доступ к вкладкам пользователя
        private void SetAccess(User user)
        {
            if(user.UsersPermissions == "3")
            {
                OrderTab.Visibility = Visibility.Collapsed;
                ReportTab.Visibility = Visibility.Collapsed;
                DirectoryTab.Visibility = Visibility.Collapsed;
                SettingTab.Visibility = Visibility.Collapsed;
            }
            if(user.UsersPermissions == "2")
            {
                SettingTab.Visibility = Visibility.Collapsed;
            }
        }

        // Выбор вкладки
        private void TabControl_Select(object sender, SelectionChangedEventArgs e)
        {
            TabItem selectedItem = MainTabControl.SelectedItem as TabItem;
            if (selectedItem != null)
            {
                // Проверяем, является ли Tag типом страницы
                if (selectedItem.Tag is string pageTypeString)
                {
                    Type pageType = Type.GetType(pageTypeString);
                    if (pageType != null && typeof(Page).IsAssignableFrom(pageType))
                    {
                        // Создаем страницу и устанавливаем ее как содержимое TabItem
                        var page = (Page)Activator.CreateInstance(pageType);
                        MainContent.Content = page; // Устанавливаем содержимое в Frame
                    }
                }
                else
                {
                    new AuthWindow().Show();
                    this.Close();
                }
            }
        }
        
        // Изменение размера рабочего экрана
        private void change_Screeen(object sender, RoutedEventArgs e)
        {
            if (SystemParameters.PrimaryScreenHeight > 1000)
            {
                this.Width = 800;
                this.Height = 960;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
    }
}
