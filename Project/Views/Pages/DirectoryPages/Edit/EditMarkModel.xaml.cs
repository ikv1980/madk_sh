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
            EditModelName.Width = 255;
            EditMarkName.Width = 255;
            EditCountryName.Width = 255;
            ButtonAddModel.Visibility = Visibility.Visible;
            ButtonAddMark.Visibility = Visibility.Visible;
            ButtonAddCountry.Visibility = Visibility.Visible;
        }

        // Конструктор для изменения (удаления) данных
        public EditMarkModel(MmMarkModel item, string button) : this()
        {
            InitializeComponent();
            Init();
            _itemId = item.Id;
            EditMarkName.SelectedItem = 
                DbUtils.db.CarsMarks.FirstOrDefault(m => m.MarkId == item.MarkId);
            EditModelName.SelectedItem = 
                DbUtils.db.CarsModels.FirstOrDefault(m => m.ModelId == item.ModelId);
            EditCountryName.SelectedItem = 
                DbUtils.db.CarsCountries.FirstOrDefault(m => m.CountryId == item.CountryId);
            EditModelName.Width = 300;
            EditMarkName.Width = 300;
            EditCountryName.Width = 300;
            ButtonAddModel.Visibility = Visibility.Collapsed;
            ButtonAddMark.Visibility = Visibility.Collapsed;
            ButtonAddCountry.Visibility = Visibility.Collapsed;
            
            // изменяем диалоговое окно, в зависимости от нажатой кнопки
            if (button == "Change")
            {
                _isEditMode = true;
                Title = "Изменение данных";
                SaveButton.Content = "Изменить";
                SaveButton.Icon = SymbolRegular.EditProhibited28;
            }
            else if (button == "Show")
            {
                _isEditMode = true;
                Title = "Просмотр данных";
                SaveButton.Visibility = Visibility.Collapsed;
            }
            else if (button == "Delete")
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
                var item = (_isEditMode || _isDeleteMode)
                    ? DbUtils.db.MmMarkModels.FirstOrDefault(x => x.Id == _itemId)
                    : new MmMarkModel();

                if (item == null)
                {
                    MessageBox.Show("Данные не найдены.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Удаление
                if (_isDeleteMode)
                {
                    DbUtils.db.MmMarkModels.Remove(item);
                }
                else
                {
                    if (!IsValidInput())
                        return;

                    // Изменение или добавление
                    UpdateItem(item);

                    if (!_isEditMode)
                    {
                        DbUtils.db.MmMarkModels.Add(item);
                    }
                }

                DbUtils.db.SaveChanges();
                RefreshRequested?.Invoke();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
            EditMarkName.ItemsSource = DbUtils.db.CarsMarks.Where(x => !x.Delete).ToList();
            EditModelName.ItemsSource = DbUtils.db.CarsModels.Where(x => !x.Delete).ToList();
            EditCountryName.ItemsSource = DbUtils.db.CarsCountries.Where(x => !x.Delete).ToList();
        }

        // Валидация данных
        private bool IsValidInput()
        {
            if (EditMarkName.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана марка авто", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (EditModelName.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана модель авто", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            if (EditCountryName.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана страна авто", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DbUtils.db.MmMarkModels.Any(x =>
                    x.MarkId == (int)EditMarkName.SelectedValue &&
                    x.ModelId == (int)EditModelName.SelectedValue &&
                    x.CountryId == (int)EditCountryName.SelectedValue &&
                    x.Id != _itemId))
            {
                MessageBox.Show("Такая запись уже существует в базе данных.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // Обновление данных объекта
        private void UpdateItem(MmMarkModel item)
        {
            item.MarkId = (EditMarkName.SelectedItem as CarsMark)?.MarkId ?? item.MarkId;
            item.ModelId = (EditModelName.SelectedItem as CarsModel)?.ModelId ?? item.ModelId;
            item.CountryId = (EditCountryName.SelectedItem as CarsCountry)?.CountryId ?? item.CountryId;
        }

        // События после загрузки окна
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EditModelName.Focus();
        }
        
        private void AddModel_Click(object sender, RoutedEventArgs e)
        {
            var addModel = new EditModel();
            this.Close();
            addModel.ShowDialog();
        }

        private void AddMark_Click(object sender, RoutedEventArgs e)
        {
            var addMark = new EditMark();
            this.Close();
            addMark.ShowDialog();
        }
        
        private void AddCountry_Click(object sender, RoutedEventArgs e)
        {
            var addCountry = new EditCountry();
            this.Close();
            addCountry.ShowDialog();
        }
    }
}