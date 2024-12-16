using System.Windows;
using Project.Interfaces;
using Project.Models;
using Project.Tools;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Project.Views.Pages.DirectoryPages.Edit
{
    public partial class EditUser : UiWindow, IRefresh
    {
        public event Action RefreshRequested;
        private readonly bool _isEditMode;
        private readonly bool _isDeleteMode;
        private readonly int _itemId;
        private readonly string _oldPassword;
        private readonly ValidateField _validator;

        // Конструктор для добавления данных
        public EditUser()
        {
            InitializeComponent();
            Init();
            _itemId = -1;
            _isEditMode = false;
            _isDeleteMode = false;
            Title = "Добавление данных";
            SaveButton.Content = "Добавить";
            SaveButton.Icon = SymbolRegular.AddCircle24;
            ShowUsersLogin.Visibility = Visibility.Collapsed;
            EditUsersLogin.Visibility = Visibility.Visible;
            _validator = new ValidateField();
        }

        // Конструктор для изменения (удаления) данных
        public EditUser(User item, string button) : this()
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            InitializeComponent();
            Init();
            _oldPassword = item.UsersPassword;
            _itemId = item.UsersId;

            // Установка значвений в форму
            EditUsersSurname.Text = item.UsersSurname;
            EditUsersName.Text = item.UsersName;
            EditUsersPatronymic.Text = item.UsersPatronymic;
            EditUsersBirthday.SelectedDate = item.UsersBirthday.HasValue
                ? item.UsersBirthday.Value.ToDateTime(TimeOnly.MinValue)
                : (DateTime?)null;
            EditUsersPhone.Text = item.UsersPhone;
            EditUsersMail.Text = item.UsersMail;
            EditUsersDepartment.SelectedItem =
                DbUtils.db.UsersDepartments.FirstOrDefault(m => m.DepartmentId == item.UsersDepartment);
            EditUsersFunction.SelectedItem =
                DbUtils.db.UsersFunctions.FirstOrDefault(m => m.FunctionId == item.UsersFunction);
            EditUsersStatus.SelectedItem =
                DbUtils.db.UsersStatuses.FirstOrDefault(m => m.StatusId == item.UsersStatus);
            EditUsersStartWork.SelectedDate = item.UsersStartWork.HasValue
                ? item.UsersStartWork.Value.ToDateTime(TimeOnly.MinValue)
                : (DateTime?)null;
            EditUsersStatusChange.SelectedDate = item.UsersStatusChange.HasValue
                ? item.UsersStatusChange.Value.ToDateTime(TimeOnly.MinValue)
                : (DateTime?)null;
            EditUsersLogin.Text = item.UsersLogin;
            ShowUsersLogin.Text = item.UsersLogin;

            // изменяем диалоговое окно, в зависимости от нажатой кнопки
            if (button == "Change")
            {
                _isEditMode = true;
                Title = "Изменение данных";
                SaveButton.Content = "Изменить";
                SaveButton.Icon = SymbolRegular.EditProhibited28;
                ShowUsersLogin.Visibility = Visibility.Visible;
                EditUsersLogin.Visibility = Visibility.Collapsed;
            }

            if (button == "Delete")
            {
                _isDeleteMode = true;
                Title = "Удаление данных";
                SaveButton.Content = "Удалить";
                SaveButton.Icon = SymbolRegular.Delete24;
                EditUsersLogin.Visibility = Visibility.Collapsed;
            }
        }

        // Изменение данных
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = (_isEditMode || _isDeleteMode)
                    ? DbUtils.db.Users.FirstOrDefault(x => x.UsersId == _itemId)
                    : new User();

                if (item == null)
                {
                    MessageBox.Show("Данные не найдены.", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Удаление
                if (_isDeleteMode)
                {
                    item.Delete = true; //DbUtils.db.Users.Remove(item);   
                }
                else
                {
                    if (!ValidateInputs())
                        return;
                }

                // Изменение
                if (_isEditMode)
                    UpdateItem(item);

                // Добавление
                if (!_isEditMode && !_isDeleteMode)
                {
                    item.UsersLogin = EditUsersLogin.Text.Trim();
                    UpdateItem(item);
                    DbUtils.db.Users.Add(item);
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
            Close();
        }

        // Инициализация данных для списков
        private void Init()
        {
            EditUsersDepartment.ItemsSource = DbUtils.db.UsersDepartments.Where(x => !x.Delete).ToList();
            EditUsersFunction.ItemsSource = DbUtils.db.UsersFunctions.Where(x => !x.Delete).ToList();
            EditUsersStatus.ItemsSource = DbUtils.db.UsersStatuses.Where(x => !x.Delete).ToList();
        }

        // Валидация данных
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(EditUsersName.Text) || string.IsNullOrWhiteSpace(EditUsersSurname.Text))
            {
                MessageBox.Show("Требуется заполнить Имя и Фамилию", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(EditUsersBirthday.Text))
            {
                MessageBox.Show("Требуется заполнить дату рождения", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!_validator.IsValid(EditUsersPhone.Text, "phone"))
            {
                MessageBox.Show("Некорректный телефон. В номере телефона допускаются цифры и знак +.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!_validator.IsValid(EditUsersMail.Text, "email"))
            {
                MessageBox.Show("Некорректный e-mail.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!_isEditMode && !_isDeleteMode)
            {
                var userLogin = EditUsersLogin.Text.Trim();

                if (string.IsNullOrWhiteSpace(userLogin))
                {
                    MessageBox.Show("Логин клиента не может быть пустым.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (DbUtils.db.Users.Any(x => x.UsersLogin == userLogin))
                {
                    MessageBox.Show("Клиент с таким логином уже существует.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (!_validator.IsValid(EditUsersPassword.Password, "password"))
                {
                    MessageBox.Show("Пароль должен содержать не менее 6 символов.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(EditUsersPassword.Password.Trim()) &&
                !_validator.IsValid(EditUsersPassword.Password, "password"))
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // Обновление данных объекта
        private void UpdateItem(User item)
        {
            item.UsersSurname = EditUsersSurname.Text.Trim();
            item.UsersName = EditUsersName.Text.Trim();
            item.UsersPatronymic = EditUsersPatronymic.Text.Trim();
            item.UsersBirthday = EditUsersBirthday.SelectedDate.HasValue
                ? DateOnly.FromDateTime(EditUsersBirthday.SelectedDate.Value)
                : (DateOnly?)null;
            item.UsersPhone = EditUsersPhone.Text.Trim();
            item.UsersMail = EditUsersMail.Text.Trim();
            item.UsersDepartment = (EditUsersDepartment.SelectedItem as UsersDepartment)?.DepartmentId ??
                                   item.UsersDepartment;
            item.UsersFunction = (EditUsersFunction.SelectedItem as UsersFunction)?.FunctionId ?? item.UsersFunction;
            item.UsersStartWork = EditUsersStartWork.SelectedDate.HasValue 
                ? DateOnly.FromDateTime(EditUsersStartWork.SelectedDate.Value) 
                : DateOnly.FromDateTime(DateTime.Now);
            item.UsersStatus = (EditUsersStatus.SelectedItem as UsersStatus)?.StatusId ?? item.UsersStatus;            
            item.UsersStatusChange = EditUsersStatusChange.SelectedDate.HasValue 
                ? DateOnly.FromDateTime(EditUsersStatusChange.SelectedDate.Value) 
                : DateOnly.FromDateTime(DateTime.Now);

            if (!string.IsNullOrWhiteSpace(EditUsersPassword.Password))
            {
                Helpers helper = new Helpers();
                item.UsersPassword = helper.HashPassword(EditUsersPassword.Password);
            }
            else
            {
                item.UsersPassword = _oldPassword;
            }
        }

        // Фокус на элементе
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EditUsersSurname.Focus();
        }
    }
}