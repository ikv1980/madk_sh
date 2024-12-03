using Project.Tools;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Project.ViewModels;
internal class SomePagesViewModel<TTable> : ViewModelBase where TTable : class
{
    #region fields
    private ObservableCollection<TTable> _tableValue;

    private string _searchingText;

    #endregion

    #region props
    public ObservableCollection<TTable> TableValue
    {
        get => _tableValue;
        set
        {
            _tableValue = value;
            OnPropertyChanged(nameof(TableValue));
        }
    }

    public string SearchingText
    {
        get => _searchingText;
        set
        {
            _searchingText = value;
            OnPropertyChanged(nameof(SearchingText));
        }
    }

    #endregion

    public SomePagesViewModel()
    {
        TableValue = new ObservableCollection<TTable>();
        Refresh();
    }
    
    public void LoadDataForTable(string tableName)
    {
        try
        {
            var data = DbUtils.GetTableAllValuesByName(tableName);

            if (data != null && data is IEnumerable<TTable> typedData)
            {
                TableValue = new ObservableCollection<TTable>(typedData);
                Console.WriteLine($"Загружены данные для таблицы {tableName}: {string.Join(", ", typedData)}");
            }
            else
            {
                // Преобразуем данные через LINQ
                var castedData = ((IEnumerable<object>)data).Cast<TTable>();
                TableValue = new ObservableCollection<TTable>(castedData);
                Console.WriteLine($"Загружены данные для таблицы {tableName}: {string.Join(", ", castedData)}");
            }
        }
        catch (InvalidCastException ex)
        {
            Console.WriteLine($"Ошибка приведения типов:\n {ex.Message}");
            MessageBox.Show($"Ошибка приведения типов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки данных:\n {ex.Message}");
            MessageBox.Show($"Ошибка загрузки данных:\n {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    #region Commands
    public RelayCommand SearchDataCommand => new RelayCommand(obj => SearchData(SearchingText));

    public RelayCommand OpenAddDialogCommand => new RelayCommand(obj => OpenAddDialog(obj));

    public RelayCommand OpenChangeDialogCommand => new RelayCommand(obj => OpenChangeDialog(obj));

    public RelayCommand RefreshCommand => new RelayCommand(obj => Refresh());

    #endregion

    #region Methods
    protected virtual void Refresh()
    {
        try
        {
            var values = DbUtils.GetTableAllValues<TTable>();
            TableValue = new ObservableCollection<TTable>(values);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка запроса к серверу:\n {ex.Message}");
            MessageBox.Show($"Ошибка запроса к серверу:\n {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

    protected virtual void SearchData(string searchText)
    {
        try
        {
            var values = DbUtils.GetSearchingValues<TTable>(searchText);
            TableValue = new ObservableCollection<TTable>(values);
            Console.WriteLine($"Результаты поиска для '{searchText}': {string.Join(", ", values)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка запроса к серверу:\n {ex.Message}");
            MessageBox.Show($"Ошибка запроса к серверу:\n {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OpenAddDialog(object parameter)
    {
        if (parameter is Type userControlType && typeof(Window).IsAssignableFrom(userControlType))
        {
            try
            {
                var window = (Window)Activator.CreateInstance(userControlType);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при открытии страницы:\n {ex.Message}");
                MessageBox.Show($"Ошибка при открытии страницы:\n {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void OpenChangeDialog(object parameter)
    {
        if (parameter is object[] parameters && parameters.Length == 2)
        {
            if (parameters[1] is Type userControlType && typeof(Window).IsAssignableFrom(userControlType) && parameters[0] is Button button)
            {
                ConstructorInfo constructor = userControlType.GetConstructor(new Type[] { typeof(TTable) });
                if (constructor != null)
                {
                    TTable value = (TTable)button.DataContext;
                    var window = (Window)constructor.Invoke(new object[] { value });
                    window.ShowDialog();
                }
                else
                {
                    MessageBox.Show($"Ошибка при открытии страницы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
    #endregion
}
