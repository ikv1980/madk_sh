using Project.Models;
using System.Windows;
using System.Windows.Controls;
using Project.Tools;
using Microsoft.EntityFrameworkCore;

namespace Project.Views.Pages
{
    public partial class UserPage : Page
    {
        private readonly UserViewModel _viewModel;
        private readonly User _user;
        private readonly ValidateField _validator;
        private readonly Helpers _helper;
        private bool _showButton;

        public UserPage()
        {
            InitializeComponent();
            _user = Global.CurrentUser;
            _viewModel = new UserViewModel(_user);
            DataContext = _viewModel;
            _validator = new ValidateField();
            _helper = new Helpers();

            // Отслеживание изменений
            NewPasswordBox.PasswordChanged += OnFieldChanged;
            NewEmailTextBox.TextChanged += OnFieldChanged;
            NewPhoneTextBox.TextChanged += OnFieldChanged;
        }

        public class UserViewModel
        {
            public string FullName { get; }
            public string UsersLogin { get; }
            public string UsersPassword { get; set; }
            public string UsersEmail { get; set; }
            public string UsersPhone { get; set; }
            public string UsersBirthday { get; }
            public string UsersDepartment { get; }
            public string UsersFunction { get; }
            public string UsersStatus { get; }
            public string UsersStartWork { get; }

            public UserViewModel(User user)
            {
                FullName = $"{user.UsersName} {user.UsersPatronymic} {user.UsersSurname}";
                UsersLogin = user.UsersLogin;
                UsersEmail = user.UsersMail;
                UsersPhone = user.UsersPhone;
                UsersBirthday = user.UsersBirthday.ToString("dd.MM.yyyy");
                UsersStartWork = user.UsersStartWork.ToString("dd.MM.yyyy");
                UsersStatus = $"{user.UsersStatusNavigation.StatusName} (с {user.UsersStatusChange:dd.MM.yyyy})";
                UsersDepartment = user.UsersDepartmentNavigation.DepartmentName;
                UsersFunction = user.UsersFunctionNavigation.FunctionName;
            }
        }

        // Отображение кнопки при изменениях в полях
        private void OnFieldChanged(object sender, RoutedEventArgs e)
        {
            _showButton = !string.IsNullOrWhiteSpace(NewPasswordBox.Password) ||
                          NewEmailTextBox.Text != _viewModel.UsersEmail ||
                          NewPhoneTextBox.Text != _viewModel.UsersPhone;
            UpdateButton.Visibility = _showButton ? Visibility.Visible : Visibility.Hidden;
        }

        // Сохранение изменений
        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            // Добавление новых данных
            if (!string.IsNullOrWhiteSpace(NewPasswordBox.Password))
                _viewModel.UsersPassword = NewPasswordBox.Password;

            if (NewEmailTextBox.Text != _viewModel.UsersEmail)
                _viewModel.UsersEmail = NewEmailTextBox.Text;

            if (NewPhoneTextBox.Text != _viewModel.UsersPhone)
                _viewModel.UsersPhone = NewPhoneTextBox.Text;

            try
            {
                var currentUser = await DbUtils.db.Users.SingleOrDefaultAsync(u => u.UsersLogin == _viewModel.UsersLogin);

                if (currentUser != null)
                {
                    if (!string.IsNullOrWhiteSpace(_viewModel.UsersPassword))
                    {
                        currentUser.UsersPassword = _helper.HashPassword(_viewModel.UsersPassword);
                    }

                    currentUser.UsersMail = _viewModel.UsersEmail;
                    currentUser.UsersPhone = _viewModel.UsersPhone;

                    await DbUtils.db.SaveChangesAsync();

                    MessageBox.Show("Данные успешно обновлены.", "Сохранение данных", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateButton.Visibility = Visibility.Hidden;
                    _showButton = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Валидация полей
        private bool ValidateInputs()
        {
            if (!string.IsNullOrWhiteSpace(NewPasswordBox.Password) && 
                !_validator.IsValid(NewPasswordBox.Password, "password"))
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов.", "Ошибка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!_validator.IsValid(NewEmailTextBox.Text, "email"))
            {
                MessageBox.Show("Некорректный e-mail.", "Ошибка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!_validator.IsValid(NewPhoneTextBox.Text, "phone"))
            {
                MessageBox.Show("Некорректный телефон. В номере телефона допускаются цифры и знак +.", "Ошибка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
