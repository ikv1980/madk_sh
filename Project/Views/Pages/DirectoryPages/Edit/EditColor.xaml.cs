using System.Windows;
using Project.Models;
using Project.Tools;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Project.Views.Pages.DirectoryPages.Edit
{
    public partial class EditColor : UiWindow
    {
        private readonly bool _isEditMode;
        private readonly bool _isDeleteMode;
        private readonly int _itemId;


        // Конструктор для добавления данных
        public EditColor()
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
        public EditColor(CarsColor item, string button) : this()
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _itemId = item.ColorId;
            ItemTextBox.Text = item.ColorName;
            
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
                    ? DbUtils.db.CarsColors.FirstOrDefault(x => x.ColorId == _itemId) 
                    : new CarsColor();

                if (item == null)
                {
                    MessageBox.Show("Данные не найдены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Изменение
                if (_isEditMode)
                {
                    item.ColorName = ItemTextBox.Text.Trim().ToLower();
                }
                // Удаление
                if (_isDeleteMode){
                    item.Delete = true; //DbUtils.db.CarsColors.Remove(item);
                }
                // Добавление
                if (!_isEditMode && !_isDeleteMode)
                {
                    item.ColorName = ItemTextBox.Text.Trim().ToLower();
                    DbUtils.db.CarsColors.Add(item);
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

            if (DbUtils.db.CarsColors.Any(x => x.ColorName == item && x.ColorId != _itemId))
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