using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Project.ViewModels;

namespace Project.Views.Pages
{
    public partial class DirectoryPages : Page
    {
        internal object ViewModel { get; set; }

        public DirectoryPages()
        {
            InitializeComponent();
            this.Loaded += Load_Default_Page;
            InitializeTabs();
        }

        private void InitializeTabs()
        {
            var tabs = new[]
            {
                new { Header = "Страны", Tag = "CarsCountry" },
                new { Header = "Цвета", Tag = "CarsColor" },
                new { Header = "Тип кузова", Tag = "CarsType" },
                new { Header = "Страницы", Tag = "SitePage" },
            };

            foreach (var tab in tabs)
            {
                TabItem tabItem = new TabItem
                {
                    Header = tab.Header,
                    Tag = tab.Tag,
                };
                DictionaryTabs.Items.Add(tabItem);
            }

            // Установка первой вкладки по умолчанию
            if (DictionaryTabs.Items.Count > 0)
            {
                DictionaryTabs.SelectedIndex = 0;
            }
        }

        private void Load_Default_Page(object sender, RoutedEventArgs e)
        {
            LoadDataForSelectedTab();
        }

        private void Load_Select_Page(object sender, SelectionChangedEventArgs e)
        {
            LoadDataForSelectedTab();
        }

        private void LoadDataForSelectedTab()
        {
            if (DictionaryTabs.SelectedItem is TabItem selectedTab && selectedTab.Tag is string tag)
            {
                // Получение типа модели на основе тега
                Type modelType = Type.GetType($"Project.Models.{tag}");
                if (modelType != null)
                {
                    // Создание экземпляра ViewModel с использованием обобщенного типа
                    var viewModelType = typeof(SomePagesViewModel<>).MakeGenericType(modelType);
                    ViewModel = Activator.CreateInstance(viewModelType);
                    DataContext = ViewModel;

                    // Вызов метода для загрузки данных
                    MethodInfo loadDataMethod = viewModelType.GetMethod("LoadDataForTable");
                    if (loadDataMethod != null)
                    {
                        try
                        {
                            loadDataMethod.Invoke(ViewModel, new object[] { tag });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Метод 'LoadDataForTable' не найден в {viewModelType.Name}.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"Не удалось загрузить данные для {tag}.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
