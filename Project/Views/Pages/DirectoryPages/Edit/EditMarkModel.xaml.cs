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
    public partial class EditMarkModel : UiWindow, IRefresh
    {
        public event Action RefreshRequested;
        private readonly bool _isEditMode;
        private readonly bool _isDeleteMode;
        private readonly int _itemId;

        // Конструктор для добавления данных
        public EditMarkModel()
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
        public EditMarkModel(MmMarkModel item, string button) : this()
        {
            InitializeComponent();
            Init();
            _itemId = item.Id;
            ComboBoxMark.SelectedItem = DbUtils.db.CarsMarks.FirstOrDefault(m => m.MarkId == item.MarkId);
            ComboBoxModel.SelectedItem = DbUtils.db.CarsModels.FirstOrDefault(m => m.ModelId == item.ModelId);
            
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
                TextBlock.Visibility = Visibility.Visible;
                SaveButton.Icon = SymbolRegular.Delete24;
                ComboBoxMark.Visibility = Visibility.Collapsed;
                ComboBoxModel.Visibility = Visibility.Collapsed;
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
                    ? DbUtils.db.MmMarkModels.FirstOrDefault(x => x.Id == _itemId) 
                    : new MmMarkModel();

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
                    DbUtils.db.MmMarkModels.Remove(item);
                }
                // Добавление
                if (!_isEditMode && !_isDeleteMode)
                {
                    UpdateItem(item);
                    DbUtils.db.MmMarkModels.Add(item);
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
            ComboBoxMark.ItemsSource = DbUtils.db.CarsMarks.Where(x => !x.Delete).ToList();
            ComboBoxModel.ItemsSource = DbUtils.db.CarsModels.Where(x => !x.Delete).ToList();
        }
        
        // Валидация данных
        private bool IsValidInput()
        {
            if (ComboBoxMark.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана марка авто", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            if (ComboBoxModel.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана модель авто", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            if (DbUtils.db.MmMarkModels.Any(x => 
                    x.MarkId == (int)ComboBoxMark.SelectedValue && 
                    x.ModelId == (int)ComboBoxModel.SelectedValue && 
                    x.Id != _itemId))
            {
                MessageBox.Show("Такая запись уже существует в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        // Обновление данных объекта
        private void UpdateItem(MmMarkModel item)
        {
            item.MarkId = (ComboBoxMark.SelectedItem as CarsMark)?.MarkId ?? item.MarkId;
            item.ModelId = (ComboBoxModel.SelectedItem as CarsModel)?.ModelId ?? item.ModelId;
        }
        
        // События после загрузки окна
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxModel.Focus();
        }
    }
}