﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            // Данные текущего пользователя
            Global.CurrentUser = user;
            Global.CurrentPermissions = Global.ParsePermissions(Global.CurrentUser);
            
            this.Loaded += change_Screeen;
            MainTabControl.SelectedIndex = 1;
            SecondTabControl.SelectedIndex = 0;
            MainContent.Content = new UserPage();
            SetAccess();
        }

        // Доступ к вкладкам и справочникам пользователя
        private void SetAccess()
        {
            if (Global.CurrentPermissions.Tabs == null || Global.CurrentPermissions.Tabs.Count == 0)
            {
                MessageBox.Show("Разрешения пользователя не определены. Доступ ограничен.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Доступ к вкладкам
            foreach (var tabPermission in Global.CurrentPermissions.Tabs)
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
                        Directories.Visibility = visibility;
                        break;
                }
            }

            // Доступ к справочникам
            foreach (var directoriesPermission in Global.CurrentPermissions.Directories)
            {
                var visibility = directoriesPermission.Permissions.Read ? Visibility.Visible : Visibility.Collapsed;

                switch (directoriesPermission.Name.ToLower())
                {
                    // для Заказов
                    case "order":
                        OrdersButton.Visibility = visibility;
                        break;
                    case "client":
                        OrdersClientButton.Visibility = visibility;
                        break;
                    case "delivery":
                        OrdersDeliveryButton.Visibility = visibility;
                        break;
                    case "payment":
                        OrdersPaymentButton.Visibility = visibility;
                        break;
                    case "orderstatus":
                        OrdersStatusButton.Visibility = visibility;
                        break;
                    // для Автомобилей
                    case "car":
                        CarsButton.Visibility = visibility;
                        break;
                    case "carmark":
                        CarsMarkButton.Visibility = visibility;
                        break;
                    case "carmodel":
                        CarsModelButton.Visibility = visibility;
                        break;
                    case "carcountry":
                        CarsCountryButton.Visibility = visibility;
                        break;
                    case "cartype":
                        CarsTypeButton.Visibility = visibility;
                        break;
                    case "carcolor":
                        CarsColorButton.Visibility = visibility;
                        break;
                    case "carmarkmodelcountry":
                        MmMarkModelButton.Visibility = visibility;
                        break;
                    // для Пользователей
                    case "user":
                        UsersButton.Visibility = visibility;
                        break;
                    case "userdepartment":
                        UsersDepartmentButton.Visibility = visibility;
                        break;
                    case "userposition":
                        UsersPositionButton.Visibility = visibility;
                        break;
                    case "userstatus":
                        UsersStatusButton.Visibility = visibility;
                        break;
                    case "userdepartmentposition":
                        MmDepartmentPositionButton.Visibility = visibility;
                        break;
                    // по умолчанию
                    default:
                        Console.WriteLine($"Unknown directory: {directoriesPermission.Name}");
                        break;
                }
            }

            // Обновление видимости всех Expander
            UpdateExpanderVisibility(OrdersButtonPanel, OrdersExpander);
            UpdateExpanderVisibility(CarsButtonPanel, CarsExpander);
            UpdateExpanderVisibility(UsersButtonPanel, UsersExpander);
        }

        // Скрытие панелей с кнопками
        private void UpdateExpanderVisibility(StackPanel buttonPanel, Expander expander)
        {
            bool allButtonsCollapsed = true;

            foreach (var child in buttonPanel.Children)
            {
                if (child is Button button && button.Visibility == Visibility.Visible)
                {
                    allButtonsCollapsed = false;
                    break;
                }
            }

            expander.Visibility = allButtonsCollapsed ? Visibility.Collapsed : Visibility.Visible;
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

            TabItem? selectedItem = MainTabControl.SelectedItem as TabItem;
            if (selectedItem != null)
            {
                // Проверяем, является ли Tag типом страницы
                if (selectedItem.Tag is string pageTypeString)
                {
                    Type? pageType = Type.GetType(pageTypeString);
                    if (pageType != null && typeof(Page).IsAssignableFrom(pageType))
                    {
                        // Создаем страницу и устанавливаем ее как содержимое TabItem
                        var page = (Page)Activator.CreateInstance(pageType)!;
                        MainContent.Content = page; // Устанавливаем содержимое в Frame
                    }
                }
                else
                {
                    this.Close();
                }
            }
        }

        // Нажатие на вкладку "Справочники"
        private void Directoryes_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SubMenuPopup.IsOpen = !SubMenuPopup.IsOpen;
            MainTabControl.SelectedIndex = -1;
            SecondTabControl.SelectedIndex = 1;
            // Сворачивание списков
            OrdersExpander.IsExpanded = false;
            CarsExpander.IsExpanded = false;
            UsersExpander.IsExpanded = false;
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