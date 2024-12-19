using System.ComponentModel;
using System.Windows;
using Project.Interfaces;
using Project.Models;
using Project.Tools;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Project.Views.Pages.DirectoryPages.Edit
{
    public partial class EditModelCountry : UiWindow, IRefresh
    {
        public event Action RefreshRequested;
        private readonly bool _isEditMode;
        private readonly bool _isDeleteMode;
        private readonly int _itemId;

        // Конструктор для добавления данных
        public EditModelCountry()
        {
            InitializeComponent();
            Init();
            _itemId = -1;
            _isEditMode = false;
            _isDeleteMode = false;
            Title = "Добавление данных";
            SaveButton.Content = "Добавить";
            SaveButton.Icon = SymbolRegular.AddCircle24;
        }

        // Конструктор для изменения (удаления) данных
        public EditModelCountry(MmModelCountry item, string button) : this()
        {
            InitializeComponent();
            Init();
            _itemId = item.Id;
            EditModelName.SelectedItem = DbUtils.db.CarsModels.FirstOrDefault(m => m.ModelId == item.ModelId);
            EditCountryName.SelectedItem = DbUtils.db.CarsCountries.FirstOrDefault(m => m.CountryId == item.CountryId);
            
            // изменяем диалоговое окно, в зависимости от нажатой кнопки
            if (button == "Change")
            {
                _isEditMode = true;
                Title = "Изменение данных";
                SaveButton.Content = "Изменить";
                SaveButton.Icon = SymbolRegular.EditProhibited28;
            }
            if (button == "Delete")
            {
                _isDeleteMode = true;
                Title = "Удаление данных";
                SaveButton.Content = "Удалить";
                DeleteTextBlock.Visibility = Visibility.Visible;
                SaveButton.Icon = SymbolRegular.Delete24;
            }
        }
        
        // Обработка нажатия кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsValidInput())
                    return;
                
                var item = (_isEditMode || _isDeleteMode)
                    ? DbUtils.db.MmModelCountries.FirstOrDefault(x => x.Id == _itemId) 
                    : new MmModelCountry();

                if (item == null)
                {
                    MessageBox.Show("Данные не найдены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Изменение
                if (_isEditMode)
                {
                    UpdateItem(item);
                }
                // Удаление
                if (_isDeleteMode){
                    DbUtils.db.MmModelCountries.Remove(item);
                }
                // Добавление
                if (!_isEditMode && !_isDeleteMode)
                {
                    UpdateItem(item);
                    DbUtils.db.MmModelCountries.Add(item);
                }
                
                DbUtils.db.SaveChanges();
                RefreshRequested?.Invoke();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Закрытие окна
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        // Инициализация данных для списков
        private void Init()
        {
            EditModelName.ItemsSource = DbUtils.db.CarsModels.Where(x => !x.Delete).ToList();
            EditCountryName.ItemsSource = DbUtils.db.CarsCountries.Where(x => !x.Delete).ToList();
        }
        
        // Валидация данных
        private bool IsValidInput()
        {
            if (EditModelName.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана модель авто", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            if (EditCountryName.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана страна авто", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            if (DbUtils.db.MmModelCountries.Any(x => 
                    x.CountryId == (int)EditCountryName.SelectedValue && 
                    x.ModelId == (int)EditModelName.SelectedValue && 
                    x.Id != _itemId))
            {
                MessageBox.Show("Такая запись уже существует в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        // Обновление данных объекта
        private void UpdateItem(MmModelCountry item)
        {
            item.ModelId = (EditModelName.SelectedItem as CarsModel)?.ModelId ?? item.ModelId;
            item.CountryId = (EditCountryName.SelectedItem as CarsCountry)?.CountryId ?? item.CountryId;
        }
        
        // События после загрузки окна
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EditModelName.Focus();
        }
    }
}