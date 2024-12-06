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

    public SomePagesViewModel()
    {
        TableValue = new ObservableCollection<TTable>();
        Refresh();
    }
    
    #endregion

    #region Commands
    public RelayCommand SearchDataCommand => new RelayCommand(obj => SearchData(SearchingText));

    public RelayCommand OpenAddDialogCommand => new RelayCommand(obj => AddDialog(obj));

    public RelayCommand OpenChangeDialogCommand => new RelayCommand(obj => ChangeDialog(obj));

    public RelayCommand RefreshCommand => new RelayCommand(obj => Refresh());

    #endregion

    #region Methods
    protected virtual void Refresh()
    {
        try
        {
            var values = DbUtils.GetTableAllValues<TTable>();
            
            var filteredValues = values.Where(item =>
            {
                var deleteProperty = item.GetType().GetProperty("Delete");
                if (deleteProperty != null && deleteProperty.PropertyType == typeof(bool))
                {
                    return !(bool)deleteProperty.GetValue(item);
                }
                return true;
            }).ToList();

            // Преобразуем отфильтрованные данные в ObservableCollection
            TableValue = new ObservableCollection<TTable>(filteredValues);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка запроса к серверу:\n {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    protected virtual void SearchData(string searchText)
    {
        try
        {
            var values = DbUtils.GetSearchingValues<TTable>(searchText);
            TableValue = new ObservableCollection<TTable>(values);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка запроса к серверу:\n {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AddDialog(object parameter)
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
                MessageBox.Show($"Ошибка при открытии страницы:\n {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ChangeDialog(object parameter)
    {
        if (parameter is object[] parameters && parameters.Length == 2)
        {
            if (parameters[1] is Type userControlType && typeof(Window).IsAssignableFrom(userControlType) && parameters[0] is Button button)
            {
                ConstructorInfo constructor = userControlType.GetConstructor(new Type[] { typeof(TTable), typeof(string) });
                if (constructor != null)
                {
                    TTable value = (TTable)button.DataContext;
                    var window = (Window)constructor.Invoke(new object[] { value, button.Name });
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