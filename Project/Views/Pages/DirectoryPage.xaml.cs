using System.Data;
using System.Windows;
using Project.Tools;
using System.Windows.Controls;
using Project.Models;
using Project.Tools.DbRequest;

namespace Project.Views.Pages
{
    public partial class DirectoryPage : Page
    {
        private TablesRequest _currentDict;    // Активный справочник
        private int _currentPage = 1;           // Текущая страница
        private int _pageSize = 20;             // Значение по умолчанию
        private bool _isDataLoaded = false;     // Флаг загрузки данных

        public DirectoryPage()
        {
            InitializeComponent();
            this.Loaded += Load_Default_Page;
            
            // Инициализация вкладок
            InitializeTabs();
        }

        // Построение вкладок справочников
        private void InitializeTabs()
        {
            // Определяем массив объектов с заголовками и тегами
            // в теории так же можно вывести в БД, с возможностью управления.
            // например `Dictionary` (dictionary_id, dictionary_number, dictionary_name_eng, dictionary_name_rus, dictionary_show)
            var tabs = new[]
            {
                new { Header = "Страны", Tag = "country" },
                new { Header = "Цвета", Tag = "color" },
                new { Header = "Тип кузова", Tag = "type" },
                new { Header = "Страницы", Tag = "page" },
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

            if (DictionaryTabs.Items.Count > 0)
            {
                DictionaryTabs.SelectedIndex = 0;
            }
        }
        
        private void Load_Default_Page(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DictionaryTabs.Items.Count > 0 && DictionaryTabs.Items[0] is TabItem firstTab && firstTab.Tag is string tag)
            {
                _currentDict = new TablesRequest(tag);
                LoadData();
            }
        }

        private void Load_Select_Page(object sender, SelectionChangedEventArgs e)
        {
            if (DictionaryTabs.SelectedItem is TabItem selectedTab && selectedTab.Tag is string tag)
            {
                _currentDict = new TablesRequest(tag);
                _currentPage = 1;
                _isDataLoaded = false; // Сбросить флаг при смене вкладки
                LoadData();
            }
        }
        
        private void LoadData()
        {
            // Если объект не создан или данные уже загружены, то выход
            if (_currentDict == null || _isDataLoaded) return;

            //var searchQuery = SearchBox.Text;
            var searchQuery = "";
            //var searchQuery = SearchBox?.Text ?? string.Empty; // Проверка на null
            var data = _currentDict.GetPageData(_currentPage, _pageSize, searchQuery);

            DataTable.Columns.Clear();
            foreach (var column in _currentDict.GetColumns())
            {
                DataTable.Columns.Add(new DataGridTextColumn
                {
                    Header = column.DisplayName,
                    Binding = new System.Windows.Data.Binding(column.ColumnName)
                });
            }

            DataTable.ItemsSource = data;
            _isDataLoaded = true;  // Устанавливаем флаг, что данные загружены

            UpdatePaginationButtons();
        }
        
        // Номер текущей строки
        private void DataTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = $"{((_currentPage - 1) * _pageSize) + e.Row.GetIndex() + 1}";
        }

        // Выбор количества отображаемых записей из БД
        private void PageSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PageSizeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (int.TryParse(selectedItem.Content.ToString(), out int newPageSize))
                {
                    _pageSize = newPageSize;
                    _currentPage = 1;
                    _isDataLoaded = false;
                    LoadData();
                }
            }
        }
        
        // Предыдущая страница
        private void PreviousPageButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                _isDataLoaded = false;
                LoadData();
            }
        }
        
        // Следующая страница
        private void NextPageButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_currentDict.HasMoreData(_currentPage, _pageSize))
            {
                _currentPage++;
                _isDataLoaded = false;
                LoadData();
            }
        }
        
        // Обновление кнопок пагинации
        private void UpdatePaginationButtons()
        {
            if (_currentDict == null) return;

            PreviousPageButton.IsEnabled = _currentPage > 1;
            NextPageButton.IsEnabled = _currentDict.HasMoreData(_currentPage, _pageSize);
            PageInfo.Text = $"Страница {_currentPage}";
        }
        
        
        
        
        
        
        // Добавление новой записи в БД
        private void AddRecord(object sender, System.Windows.RoutedEventArgs e)
        {
            return;
        }
        private void SearchText(object sender, TextChangedEventArgs e)
        {
            Console.WriteLine($"-----------------------{SearchBox.Text}");
        }
    }
}
