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
        private ValidateField checkField;
        private Helpers helper;
        private bool showButton;

        public UserPage()
        {
            InitializeComponent();
            _user = Global.CurrentUser;
            _viewModel = new UserViewModel(_user);
            DataContext = _viewModel;
            helper = new Helpers();
            
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
                UsersStatus = $"{user.UsersStatusNavigation.StatusName} (c {user.UsersStatusChange.ToString("dd.MM.yyyy")})";
                UsersDepartment = user.UsersDepartmentNavigation.DepartmentName;
                UsersFunction = user.UsersFunctionNavigation.FunctionName;
            }
        }
        // Отображение кнопки при изменениях в полях
        private void OnFieldChanged(object sender, RoutedEventArgs e)
        {
            showButton = NewPasswordBox.Password != "" ||
                   NewEmailTextBox.Text != _viewModel.UsersEmail ||
                   NewPhoneTextBox.Text != _viewModel.UsersPhone;
            UpdateButton.Visibility = showButton ?  Visibility.Visible : Visibility.Hidden;
        }
        // Сохранение изменений
        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка корректности заполнения полей
            checkField = new ValidateField();
            if (
                (NewPasswordBox.Password != "" && checkField.Validate(NewPasswordBox.Password, "password")) ||
                checkField.Validate(NewEmailTextBox.Text, "email") ||
                checkField.Validate(NewPhoneTextBox.Text, "phone")
                )
            {
                return;
            }
            // Добавление новых данных
            if (NewPasswordBox.Password != "")
                _viewModel.UsersPassword = NewPasswordBox.Password;

            if (NewEmailTextBox.Text != _viewModel.UsersEmail)
                _viewModel.UsersEmail = NewEmailTextBox.Text;

            if (NewPhoneTextBox.Text != _viewModel.UsersPhone)
                _viewModel.UsersPhone = NewPhoneTextBox.Text;

            try
            {
                var currentUser = await DbConnect.db.Users.SingleOrDefaultAsync(u => u.UsersLogin == _viewModel.UsersLogin);

                if (currentUser != null)
                {
                    if (_viewModel.UsersPassword != "" && NewPasswordBox.Password != "")
                    {
                        currentUser.UsersPassword = helper.HashPassword(_viewModel.UsersPassword);
                    }
                  
                    currentUser.UsersMail = _viewModel.UsersEmail;
                    currentUser.UsersPhone = _viewModel.UsersPhone;

                    await DbConnect.db.SaveChangesAsync();

                    MessageBox.Show("Данные успешно обновленны.", "Сохранение данных", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateButton.Visibility = Visibility.Hidden;
                    showButton = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
