using System.Data;
using System.Windows;
using Project.Tools;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Project.Views.Pages
{
    public partial class DirectoryPage : Page
    {
        private Tables_Request _currentDict;       // Активный справочник
        private int _currentPage = 1;           // Текущая страница
        private int _pageSize = 20;             // Значение по умолчанию
        private bool _isDataLoaded = false;     // Флаг загрузки данных
        private DispatcherTimer _searchTimer;   // Таймер

        public DirectoryPage()
        {
            InitializeComponent();
            this.Loaded += Load_Default_Page;
            
            // Инициализация вкладок
            InitializeTabs();
            
            // Инициализация таймера
            _searchTimer = new DispatcherTimer();
            _searchTimer.Interval = TimeSpan.FromMilliseconds(500); // Задержка 300 мс
            _searchTimer.Tick += OnSearchTimerTick;
        }

        private void InitializeTabs()
        {
            // Определяем массив объектов с заголовками и тегами
            var tabs = new[]
            {
                new { Header = "Страны", Tag = "CarsCountry" },
                new { Header = "Цвета", Tag = "CarsColor" },
                new { Header = "Тип кузова", Tag = "CarsType" },
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
                _currentDict = new Tables_Request(tag);
                LoadData();
            }
        }

        private void Load_Select_Page(object sender, SelectionChangedEventArgs e)
        {
            if (DictionaryTabs.SelectedItem is TabItem selectedTab && selectedTab.Tag is string tag)
            {
                _currentDict = new Tables_Request(tag);
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
            var searchQuery = SearchBox?.Text ?? string.Empty; // Проверка на null
            var data = _currentDict.GetPageData(_currentPage, _pageSize, searchQuery);

            DataTable.Columns.Clear();
            foreach (var column in _currentDict.Columns.Where(c => c.IsVisible))
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

        
      
        // Добавление новой записи в БД
        private void AddRecordButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _currentDict?.AddRecord();
            _isDataLoaded = false;
            LoadData();
        }

        // Выбор количества отображаемых записей из БД
        private void PageSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PageSizeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (int.TryParse(selectedItem.Content.ToString(), out int newPageSize))
                {
                    _pageSize = newPageSize;
                    Console.WriteLine($"++++++++++++ Count Items for Page: {_pageSize}");
                    _currentPage = 1;
                    _isDataLoaded = false; // Сбрасываем флаг загрузки данных
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
        
        

        private void SearchText(object sender, TextChangedEventArgs e)
        {
            // Сбрасываем таймер при каждом изменении текста
            _searchTimer.Stop();
            _currentPage = 1;
            _isDataLoaded = false;
    
            // Запускаем таймер
            _searchTimer.Start();
        }
        private void OnSearchTimerTick(object sender, EventArgs e)
        {
            _searchTimer.Stop();
            LoadData();
        }
    }
}
