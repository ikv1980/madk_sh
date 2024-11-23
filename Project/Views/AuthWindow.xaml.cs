using Project.Models;
using Project.Tools;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Security.Cryptography;
using Wpf.Ui.Animations;
using Wpf.Ui.Controls;

namespace Project.Views
{
    public partial class AuthWindow : UiWindow
    {
        int attemptCount;
        string answerForCaptcha;

        public AuthWindow()
        {
            InitializeComponent();
        }
        // Загрузка окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Transitions.ApplyTransition(this, TransitionType.FadeInWithSlide, 1000);
        }

        // Авторизация пользователя 
        private async void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            //string login = LoginTextBox.Text;
            //string enteredPassword = HashPassword(PasswordBox.Password);

            string login = "ikv1980";
            string enteredPassword = HashPassword("Kostik80");

            var user = await Task.Run(() => DbUtils.db.Users.SingleOrDefault(u => u.UsersLogin == login));

            if (user != null && enteredPassword == user.UsersPassword)
            {
                // Проверяем статус пользователя
                if (user.UsersStatus == 1)
                {
                    MessageOk.Show("Доступ запрещён", "Ваш аккаунт заблокирован. Обратитесь к администратору.", "danger");
                    return;
                }
                // Запуск рабочего окна проекта
                new ProjectWindow(user).Show();
                Close();
            }
            else
            {
                attemptCount++;
                if (attemptCount == 3)
                {
                    GenerateCaptcha();
                    CaptchaDialog.Show();
                }
                else
                {
                    MessageOk.Show("Ошибка авторизации", "Логин или пароль введены неверно.\nПроверьте введенные данные", "attention");
                }
            }
        }

        // Регистрация пользователя 
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string login = RegisterLoginTextBox.Text;
            string name = NameTextBox.Text;
            string surename = SurnameTextBox.Text;
            string password = HashPassword(RegisterPasswordBox.Password);

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surename))
            {
                MessageOk.Show("Ошибка регистрации", "Необходимо заполнить все поля", "attention");
                return;
            }

            if (RegisterPasswordBox.Password.Length < 6)
            {
                MessageOk.Show("Ошибка регистрации", "Пароль должен содержать не менее 6 символов.", "attention");
                return;
            }

            if (DbUtils.db.Users.Any(u => u.UsersLogin == login))
            {
                MessageOk.Show("Ошибка регистрации", "Пользователь с таким логином уже существует.", "danger");
                return;
            }

            // Добавление нового пользователя в базу данных
            var newUser = new User
            {
                UsersLogin = login,
                UsersPassword = password,
                UsersName = name,
                UsersSurname = surename,
                UsersDepartment = 1,
                UsersFunction = 1,
                UsersStatus = 1
            };

            await Task.Run(() => { 
                DbUtils.db.Users.Add(newUser);
                DbUtils.db.SaveChanges();                
            });

            MessageOk.Show("Успешно", "Вы успешно зарегистрированы!", "success");
            RegisterLoginTextBox.Text = string.Empty;
            RegisterPasswordBox.Clear();
            MainTabControl.SelectedIndex = 0;
        }

        // Шифрование пароля
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Генерация соли
                byte[] salt = Encoding.UTF8.GetBytes("ikv1980MadK");
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = passwordBytes.Concat(salt).ToArray();

                // Хэширование
                byte[] hash = sha256.ComputeHash(saltedPassword);
                return Convert.ToBase64String(hash);
            }
        }

        // Генерация капчи 
        private void GenerateCaptcha()
        {
            CaptchaDialog.Show();
            Captcha.CreateCaptcha(EasyCaptcha.Wpf.Captcha.LetterOption.Alphanumeric, 6);
            answerForCaptcha = Captcha.CaptchaText;
            AnswerTextBox.Text = string.Empty;
        }
        // генерация новой капчи
        private void CaptchaLeftClick(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
        }
        // проверка капчи
        private void CaptchaRightClick(object sender, RoutedEventArgs e)
        {
            if (AnswerTextBox.Text == answerForCaptcha)
            {
                CaptchaDialog.Hide();
                attemptCount = 0;
            }
            else
            {
                GenerateCaptcha();
            }
        }
    }
}
