using System.Windows;
using Project.Tools;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Project.Views.Pages.DirectoryPages.Edit
{
    public partial class EditMarkModel : UiWindow
    {
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
        public EditMarkModel(Models.MmMarkModel item, string button) : this()
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            
            InitializeComponent();
            Init();
            _itemId = item.Id;
            ComboBoxMark.SelectedItem = item.MarkId;
            ComboBoxModel.SelectedItem = item.ModelId;
            
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
                    : new Models.MmMarkModel();

                if (item == null)
                {
                    MessageBox.Show("Данные не найдены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Изменение
                if (_isEditMode)
                {
                    item.MarkId = (ComboBoxMark.SelectedItem as Models.MmMarkModel).MarkId;
                    item.ModelId = (ComboBoxModel.SelectedItem as Models.MmMarkModel).ModelId;
                }
                // Удаление
                if (_isDeleteMode){
                    DbUtils.db.MmMarkModels.Remove(item);
                }
                // Добавление
                if (!_isEditMode && !_isDeleteMode)
                {
                    item.MarkId = (ComboBoxMark.SelectedItem as Models.MmMarkModel).MarkId;
                    item.ModelId = (ComboBoxModel.SelectedItem as Models.MmMarkModel).ModelId;
                    DbUtils.db.MmMarkModels.Add(item);
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
                MessageBox.Show("Не выбрана марка авто", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            if (ComboBoxModel.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана модель авто", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            if (DbUtils.db.MmMarkModels.Any(x => 
                    x.MarkId == (int)ComboBoxMark.SelectedValue && 
                    x.ModelId == (int)ComboBoxModel.SelectedValue && 
                    x.Id != _itemId))
            {
                MessageBox.Show("Такая запись уже существует в базе данных.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        // События после загрузки окна
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxModel.Focus();
        }
    }
}