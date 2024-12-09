using System.Windows;
using Project.Tools;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Project.Views.Pages.DirectoryPages.Edit
{
    public partial class EditCountry : UiWindow
    {
        private readonly bool _isEditMode;
        private readonly bool _isDeleteMode;
        private readonly int _itemId;


        // Конструктор для добавления данных
        public EditCountry()
        {
            InitializeComponent();
            _itemId = -1;
            _isEditMode = false;
            _isDeleteMode = false;
            Title = "Добавление данных";
            SaveButton.Content = "Добавить";
            SaveButton.Icon = SymbolRegular.AddCircle24;
        }

        // Конструктор для изменения (удаления) данных
        public EditCountry(Models.CarsCountry item, string button) : this()
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _itemId = item.CountryId;
            ItemTextBox.Text = item.CountryName;
            
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
                SaveButton.Icon = SymbolRegular.Delete24;
                TextBlock.Visibility = Visibility.Visible;
                ItemTextBox.Visibility = Visibility.Collapsed;
            }
        }
        
        // Изменение данных
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsValidInput())
                    return;

                var item = (_isEditMode || _isDeleteMode)
                    ? DbUtils.db.CarsCountries.FirstOrDefault(x => x.CountryId == _itemId) 
                    : new Models.CarsCountry();

                if (item == null)
                {
                    MessageBox.Show("Данные не найдены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Изменение
                if (_isEditMode)
                {
                    item.CountryName = ItemTextBox.Text.Trim();
                }
                // Удаление
                if (_isDeleteMode){
                    DbUtils.db.CarsCountries.Remove(item);
                }
                // Добавление
                if (!_isEditMode && !_isDeleteMode)
                {
                    item.CountryName = ItemTextBox.Text.Trim();
                    DbUtils.db.CarsCountries.Add(item);
                }
                
                DbUtils.db.SaveChanges();
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
        
        // Валидация данных
        private bool IsValidInput()
        {
            var item = ItemTextBox.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(item))
            {
                MessageBox.Show("Поле не должно быть пустым.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DbUtils.db.CarsCountries.Any(x => x.CountryName.Trim().ToLower() == item && x.CountryId != _itemId))
            {
                MessageBox.Show($"Запись '{ItemTextBox.Text}' уже существует в базе.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        // События после загрузки окна
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ItemTextBox.Focus();
        }
    }
}