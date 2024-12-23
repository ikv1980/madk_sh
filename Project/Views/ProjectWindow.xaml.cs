using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernWpf.Controls;
using Project.Models;
using Project.Tools;
using Project.Views.Pages;
using Page = System.Windows.Controls.Page;

namespace Project.Views
{
    public partial class ProjectWindow : Window
    {
        public ProjectWindow(User user)
        {
            InitializeComponent();
            Global.CurrentUser = user;
            Global.ParsePermissions(user);
            this.Loaded += change_Screeen;
            MainTabControl.SelectedIndex = 1;
            SecondTabControl.SelectedIndex = 0;
            MainContent.Content = new UserPage();
            SetAccess();
        }

        // Доступ к вкладкам и справочникам пользователя
        private void SetAccess()
        {
            if (Global.ParsedPermissions?.Tabs == null || Global.ParsedPermissions.Tabs.Count == 0)
            {
                MessageBox.Show("Разрешения пользователя не определены. Доступ ограничен.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Доступ к вкладкам
            foreach (var tabPermission in Global.ParsedPermissions.Tabs)
            {
                var visibility = tabPermission.Permissions.Read ? Visibility.Visible : Visibility.Collapsed;

                switch (tabPermission.Name.ToLower())
                {
                    case "user":
                        UserPage.Visibility = visibility;
                        break;
                    case "order":
                        OrderPage.Visibility = visibility;
                        break;
                    case "report":
                        ReportPage.Visibility = visibility;
                        break;
                    case "setting":
                        SettingTab.Visibility = visibility;
                        break;
                    case "dict":
                        Directoryes.Visibility = visibility;
                        break;
                }
            }

            // Доступ к справочникам
            foreach (var directoryPermission in Global.ParsedPermissions.Directoryes)
            {
                var visibility = directoryPermission.Permissions.Read ? Visibility.Visible : Visibility.Collapsed;
               
                switch (directoryPermission.Name.ToLower())
                {
                    case "orders":
                        OrderMenuGroup.Visibility = visibility;
                        break;
                    case "cars":
                        CarMenuGroup.Visibility = visibility;
                        break;
                    case "users":
                        UserMenuGroup.Visibility = visibility;
                        break;
                    default:
                        Console.WriteLine($"Unknown directory: {directoryPermission.Name}");
                        break;
                }
            }
        }

        // Выбор вкладки
        private void TabControl_Select(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tabControl)
            {
                if (tabControl == MainTabControl)
                {
                    SecondTabControl.SelectedIndex = 0;
                }
            }

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
                    this.Close();
                }
            }
        }

        // Выбор справочника
        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender,
            ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;
            if (item.Tag is Type pageType && typeof(System.Windows.Controls.Page).IsAssignableFrom(pageType))
            {
                MainContent.Content = (System.Windows.Controls.Page)Activator.CreateInstance(pageType);
            }
            else if (item.Tag != null)
            {
                this.Close();
            }
        }

        // Нажатие на вкладку "Справочники"
        private void Directoryes_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SubMenuPopup.IsOpen = !SubMenuPopup.IsOpen;
            MainTabControl.SelectedIndex = -1;
            SecondTabControl.SelectedIndex = 1;
            CollapsedMenu();
        }

        private void SubMenuButton_Click(object sender, RoutedEventArgs e)
        {
            SubMenuPopup.IsOpen = !SubMenuPopup.IsOpen;
            var button = sender as Button;
            if (button != null)
            {
                var pageType = button.Tag as Type;
                if (pageType != null)
                {
                    MainContent.Content = (System.Windows.Controls.Page)Activator.CreateInstance(pageType);
                }
            }
        }

        // Контекстное меню "Заказы"
        private void VisibleOrderButton(object sender, RoutedEventArgs e)
        {
            ToggleMenuVisibility(OrderMenuGroup, true);
        }

        private void CollapsedOrderButton(object sender, RoutedEventArgs e)
        {
            ToggleMenuVisibility(OrderMenuGroup, false);
        }

        // Контекстное меню "Автомобили"
        private void VisibleCarButton(object sender, RoutedEventArgs e)
        {
            ToggleMenuVisibility(CarMenuGroup, true);
        }

        private void CollapsedCarButton(object sender, RoutedEventArgs e)
        {
            ToggleMenuVisibility(CarMenuGroup, false);
        }

        // Контекстное меню "Пользователь"
        private void VisibleUserButton(object sender, RoutedEventArgs e)
        {
            ToggleMenuVisibility(UserMenuGroup, true);
        }

        private void CollapsedUserButton(object sender, RoutedEventArgs e)
        {
            ToggleMenuVisibility(UserMenuGroup, false);
        }

        private void ToggleMenuVisibility(StackPanel group, bool isVisible)
        {
            foreach (var child in group.Children)
            {
                if (child is Button button)
                {
                    switch (button.Name)
                    {
                        case nameof(OrderVisible):
                        case nameof(CarVisible):
                        case nameof(UserVisible):
                            button.Visibility = isVisible ? Visibility.Collapsed : Visibility.Visible;
                            break;
                        case nameof(OrderCollapse):
                        case nameof(CarCollapse):
                        case nameof(UserCollapse):
                            button.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                            break;
                        default:
                            button.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                            break;
                    }
                }
            }
        }

        // Свернуть все меню
        private void CollapsedMenu()
        {
            ToggleMenuVisibility(OrderMenuGroup, false);
            ToggleMenuVisibility(CarMenuGroup, false);
            ToggleMenuVisibility(UserMenuGroup, false);
        }

        // Изменение размера рабочего экрана
        private void change_Screeen(object sender, RoutedEventArgs e)
        {
            if (SystemParameters.PrimaryScreenHeight > 1000)
            {
                this.Width = 1250;
                this.Height = 960;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
    }
}