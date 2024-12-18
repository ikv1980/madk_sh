using System.Windows;
using Project.Interfaces;
using Project.Models;
using Project.Tools;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Project.Views.Pages.DirectoryPages.Edit
{
    public partial class EditFunction : UiWindow, IRefresh
    {
        public event Action RefreshRequested;
        private readonly bool _isEditMode;
        private readonly bool _isDeleteMode;
        private readonly int _itemId;

        // Конструктор для добавления данных
        public EditFunction()
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
        public EditFunction(UsersFunction item, string button) : this()
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            InitializeComponent();
            Init();
            _itemId = item.FunctionId;
            EditFunctionName.Text = item.FunctionName;

            // Получаем марку, связанную с моделью
            var selectedDepartment = DbUtils.db.MmDepartmentFunctions
                .Where(mm => mm.FunctionId == item.FunctionId)
                .Select(mm => mm.Department)
                .FirstOrDefault();
            if (selectedDepartment != null)
            {
                EditDepartmentName.SelectedItem = selectedDepartment;
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
                var item = (_isEditMode || _isDeleteMode)
                    ? DbUtils.db.UsersFunctions.FirstOrDefault(x => x.FunctionId == _itemId)
                    : new Models.UsersFunction();

                if (item == null)
                {
                    MessageBox.Show("Данные не найдены.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Удаление
                if (_isDeleteMode)
                {
                    item.Delete = true; // DbUtils.db.UsersFunctions.Remove(item);   
                }
                else
                {
                    if (!IsValidInput())
                        return;

                    // Изменение данных
                    if (_isEditMode)
                    {
                        item.FunctionName = EditFunctionName.Text.Trim();
                        // Обновление марки для модели
                        UpdateItem(item);
                    }
                    else
                    {
                        item.FunctionName = EditFunctionName.Text.Trim();
                        DbUtils.db.UsersFunctions.Add(item);
                        DbUtils.db.SaveChanges();

                        // Получаем новый MarkId из выбранной марки и создаем новую связь
                        var mmDepartmentFunction = new MmDepartmentFunction
                        {
                            FunctionId = item.FunctionId,
                            DepartmentId = (EditDepartmentName.SelectedItem as UsersDepartment)?.DepartmentId ?? -1
                        };
                        DbUtils.db.MmDepartmentFunctions.Add(mmDepartmentFunction);
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
            var item = EditFunctionName.Text.Trim().ToLower();

            // Проверка на заполнение должности
            if (string.IsNullOrWhiteSpace(item))
            {
                MessageBox.Show("Поле 'Должность' не должно быть пустым.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            // Проверка на уникальность должности
            if (DbUtils.db.UsersFunctions.Any(x => x.FunctionName.Trim().ToLower() == item && x.FunctionId != _itemId))
            {
                MessageBox.Show($"Запись '{EditFunctionName.Text}' уже существует в базе.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Проверка на наличие выбранного отдела
            if (EditDepartmentName.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите отдел.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Проверка на уникальность связки отдел - должность
            var selectedDepartment = EditDepartmentName.SelectedItem as UsersDepartment;

            if (DbUtils.db.UsersFunctions.Any(m => m.FunctionName.Trim().ToLower() == item) &&
                DbUtils.db.MmDepartmentFunctions.Any(mm => mm.DepartmentId == selectedDepartment.DepartmentId))
            {
                MessageBox.Show($"У отдела '{selectedDepartment.DepartmentName}' уже есть такая должность.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // Инициализация данных для списков
        private void Init()
        {
            EditDepartmentName.ItemsSource = DbUtils.db.UsersDepartments.Where(x => !x.Delete).ToList();
        }

        // Обновление данных объекта
        private void UpdateItem(UsersFunction item)
        {
            // Обновляем идентификатор марки на основе выбранного элемента
            var selectedDepartment = EditDepartmentName.SelectedItem as UsersDepartment;
            if (selectedDepartment != null)
            {
                var mmDepartmentFunction = DbUtils.db.MmDepartmentFunctions
                    .FirstOrDefault(mm => mm.FunctionId == item.FunctionId);
                if (mmDepartmentFunction != null)
                {
                    mmDepartmentFunction.DepartmentId = selectedDepartment.DepartmentId;
                }
            }
        }

        // События после загрузки окна
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EditDepartmentName.Focus();
        }
    }
}