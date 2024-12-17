using System.Windows;
using Project.Interfaces;
using Project.Models;
using Project.Tools;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Project.Views.Pages.DirectoryPages.Edit
{
    public partial class EditModel : UiWindow, IRefresh
    {
        public event Action RefreshRequested;
        private readonly bool _isEditMode;
        private readonly bool _isDeleteMode;
        private readonly int _itemId;

        // Конструктор для добавления данных
        public EditModel()
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
        public EditModel(CarsModel item, string button) : this()
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            InitializeComponent();
            Init();
            _itemId = item.ModelId;
            EditModelName.Text = item.ModelName;

            // Получаем марку, связанную с моделью
            var selectedMark = DbUtils.db.MmMarkModels
                .Where(mm => mm.ModelId == item.ModelId)
                .Select(mm => mm.Mark)
                .FirstOrDefault();
            if (selectedMark != null)
            {
                EditMark.SelectedItem = selectedMark;
            }

            // Устанавливаем параметры в зависимости от нажатой кнопки
            if (button == "Change")
            {
                _isEditMode = true;
                Title = "Изменение данных";
                SaveButton.Content = "Изменить";
                SaveButton.Icon = SymbolRegular.EditProhibited28;
            }
            else if (button == "Delete")
            {
                _isDeleteMode = true;
                Title = "Удаление данных";
                SaveButton.Content = "Удалить";
                SaveButton.Icon = SymbolRegular.Delete24;
                DeleteTextBlock.Visibility = Visibility.Visible;
            }
        }

        // Обработка нажатия кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем элемент для редактирования или создания нового
                var item = (_isEditMode || _isDeleteMode)
                    ? DbUtils.db.CarsModels.FirstOrDefault(x => x.ModelId == _itemId)
                    : new Models.CarsModel();

                if (item == null)
                {
                    MessageBox.Show("Данные не найдены.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Удаление
                if (_isDeleteMode)
                {
                    item.Delete = true; // DbUtils.db.CarsMarks.Remove(item);   
                }
                else
                {
                    if (!IsValidInput())
                        return;

                    // Изменение данных
                    if (_isEditMode)
                    {
                        item.ModelName = EditModelName.Text.Trim();
                        // Обновление марки для модели
                        UpdateItem(item);
                    }
                    else
                    {
                        item.ModelName = EditModelName.Text.Trim();
                        DbUtils.db.CarsModels.Add(item);
                        DbUtils.db.SaveChanges();

                        // Получаем новый MarkId из выбранной марки и создаем новую связь
                        var mmMarkModel = new MmMarkModel
                        {
                            ModelId = item.ModelId,
                            MarkId = (EditMark.SelectedItem as CarsMark)?.MarkId ?? -1
                        };
                        DbUtils.db.MmMarkModels.Add(mmMarkModel);
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

        // Валидация данных
        private bool IsValidInput()
        {
            var item = EditModelName.Text.Trim().ToLower();

            // Проверка на заполнение модели
            if (string.IsNullOrWhiteSpace(item))
            {
                MessageBox.Show("Поле 'Название модели' не должно быть пустым.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Проверка на наличие выбранной марки
            if (EditMark.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите марку для модели.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Проверка на уникальность связки модель - марка
            var selectedMark = EditMark.SelectedItem as CarsMark;

            if (DbUtils.db.CarsModels.Any(m => m.ModelName.Trim().ToLower() == item) &&
                DbUtils.db.MmMarkModels.Any(mm => mm.MarkId == selectedMark.MarkId))
            {
                MessageBox.Show($"У марки '{selectedMark.MarkName}' уже есть такая модель.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // Инициализация данных для списков
        private void Init()
        {
            EditMark.ItemsSource = DbUtils.db.CarsMarks.Where(x => !x.Delete).ToList();
        }

        // Обновление данных объекта
        private void UpdateItem(CarsModel item)
        {
            // Обновляем идентификатор марки на основе выбранного элемента
            var selectedMark = EditMark.SelectedItem as CarsMark;
            if (selectedMark != null)
            {
                var mmMarkModel = DbUtils.db.MmMarkModels
                    .FirstOrDefault(mm => mm.ModelId == item.ModelId);
                if (mmMarkModel != null)
                {
                    mmMarkModel.MarkId = selectedMark.MarkId;
                }
            }
        }

        // События после загрузки окна
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EditMark.Focus();
        }
    }
}