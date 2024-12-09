﻿using Project.Tools;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Project.Interfaces;

namespace Project.ViewModels
{
    internal class SomePagesViewModel<TTable> : ViewModelBase where TTable : class
    {
        #region fields
        private ObservableCollection<TTable> _tableValue;
        private string _searchingText;
        private int _currentPage;   // текущая страница
        private int _itemsPerPage;  // количество элементов на странице
        private int _totalItems;    // всего элементов
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

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(CurrentPageText));
            }
        }

        public int TotalPages => (_totalItems + _itemsPerPage - 1) / _itemsPerPage;

        public string CurrentPageText => $"{CurrentPage} из {TotalPages}";

        public SomePagesViewModel()
        {
            TableValue = new ObservableCollection<TTable>();
            _itemsPerPage = 100;
            _currentPage = 1;
            Refresh();
        }
        #endregion

        #region Commands
        public RelayCommand SearchCommand => new RelayCommand(obj => SearchData(SearchingText));

        public RelayCommand AddDialogButton => new RelayCommand(obj => AddDialog(obj));

        public RelayCommand ChangeDialogButton => new RelayCommand(obj => ChangeDialogBtn(obj));

        public RelayCommand ChangeDialogContextMenu => new RelayCommand(obj => ChangeDialogCtxMenu(obj));

        public RelayCommand RefreshCommand => new RelayCommand(obj => Refresh());

        public RelayCommand PreviousPageCommand => new RelayCommand(obj => ChangePage(CurrentPage - 1));

        public RelayCommand NextPageCommand => new RelayCommand(obj => ChangePage(CurrentPage + 1));
        #endregion

        #region Methods
        protected virtual void Refresh()
        {
            try
            {
                // Получаем общее количество элементов
                _totalItems = DbUtils.GetTableCount<TTable>();

                // Получаем элементы для текущей страницы с автоматической загрузкой навигационных свойств
                var values = DbUtils.GetTablePagedValuesWithIncludes<TTable>(CurrentPage, _itemsPerPage);

                // Фильтрация записей по полю Delete
                TableValue = new ObservableCollection<TTable>(
                    values.Where(item =>
                    {
                        var deleteProperty = item.GetType().GetProperty("Delete");
                        return deleteProperty == null || !(bool)deleteProperty.GetValue(item);
                    }));

                // Выводим данные в консоль
                /*
                foreach (var item in TableValue)
                {
                    var properties = item.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        var value = property.GetValue(item);
                        Console.WriteLine($"{property.Name}: {value}");
                    }
                    Console.WriteLine("-----------------"); // Разделитель между объектами
                }
                */
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
                    if (window is IRefresh dialog) dialog.RefreshRequested += Refresh;
                    window.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии страницы:\n {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ChangeDialogBtn(object parameter)
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
                        if (window is IRefresh dialog) dialog.RefreshRequested += Refresh;
                        window.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при открытии страницы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ChangeDialogCtxMenu(object parameter)
        {
            if (parameter is object[] parameters && parameters.Length > 1)
            {
                if (parameters[1] is Type userControlType && typeof(Window).IsAssignableFrom(userControlType))
                {
                    string sourceName = string.Empty;
                    object dataContext = null;

                    if (parameters[0] is MenuItem menuItem && menuItem.Parent is ContextMenu contextMenu)
                    {
                        if (contextMenu.PlacementTarget is DataGridRow row)
                        {
                            sourceName = menuItem.Tag.ToString();
                            dataContext = row.DataContext;
                        }
                    }

                    if (!string.IsNullOrEmpty(sourceName) && dataContext != null)
                    {
                        ConstructorInfo constructor = userControlType.GetConstructor(new Type[] { typeof(TTable), typeof(string) });
                        if (constructor != null)
                        {
                            TTable value = (TTable)dataContext;
                            var window = (Window)constructor.Invoke(new object[] { value, sourceName });
                            window.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show($"Ошибка при открытии страницы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Не удалось определить источник вызова.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ChangePage(int newPage)
        {
            if (newPage < 1 || newPage > TotalPages) return; // Проверка на валидность страницы
            CurrentPage = newPage;
            Refresh();
        }
        #endregion
    }
}