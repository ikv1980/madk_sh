﻿using Project.Models;
using Project.Tools;
using System.Windows;
using Wpf.Ui.Controls;
using Microsoft.EntityFrameworkCore;
using MessageBox = System.Windows.MessageBox;

namespace Project.Views
{
    public partial class AuthWindow : UiWindow
    {
        int _attemptCount;
        string _answerForCaptcha;
        private ValidateField _validator;
        private Helpers _helper;

        public AuthWindow()
        {
            InitializeComponent();
            _helper = new Helpers();
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
            //string enteredPassword = _helper.HashPassword(PasswordBox.Password);

            string login = "admin";
            string enteredPassword = _helper.HashPassword("Kostik80");

            var user = await DbUtils.db.Users
                .Where(u => u.UsersLogin == login)
                .Include(u => u.UsersDepartmentNavigation)
                .Include(u => u.UsersFunctionNavigation)
                .Include(u => u.UsersStatusNavigation)
                .SingleOrDefaultAsync();

            if (user != null && enteredPassword == user.UsersPassword)
            {
                // Проверяем удаленного пользователя
                if (user.Delete)
                {
                    MessageBox.Show(
                        $"Пользователь был удален из системы.\nОбратитесь к администратору.",
                        "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверяем статус пользователя
                if (user.UsersStatus == 1 || user.UsersStatus == 3)
                {
                    MessageBox.Show(
                        $"Ваш аккаунт заблокирован.\nСтатус в системе [{user.UsersStatusNavigation.StatusName}].\nОбратитесь к администратору.",
                        "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Запуск рабочего окна проекта
                new ProjectWindow(user).Show();
                Close();
            }
            else
            {
                _attemptCount++;
                if (_attemptCount == 3)
                {
                    GenerateCaptcha();
                    CaptchaDialog.Show();
                }
                else
                {
                    MessageBox.Show("Логин или пароль введены неверно.\nПроверьте введенные данные",
                        "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // Регистрация пользователя 
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string login = RegisterLoginTextBox.Text;
            string name = NameTextBox.Text;
            string surname = SurnameTextBox.Text;
            string password = _helper.HashPassword(RegisterPasswordBox.Password);

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname))
            {
                MessageBox.Show("Необходимо заполнить все поля.", "Ошибка регистрации", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            _validator = new ValidateField();
            if (!_validator.IsValid(RegisterPasswordBox.Password, "password"))
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (DbUtils.db.Users.Any(u => u.UsersLogin == login))
            {
                MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Добавление нового пользователя в базу данных
            var newUser = new User
            {
                UsersLogin = login,
                UsersPassword = password,
                UsersName = name,
                UsersSurname = surname,
                UsersDepartment = 1,
                UsersFunction = 1,
                UsersStatus = 1,
                UsersPermissions = DefaultPermissions.User,
            };

            await Task.Run(() =>
            {
                DbUtils.db.Users.Add(newUser);
                DbUtils.db.SaveChanges();
            });

            MessageBox.Show("Вы успешно зарегистрированы.\nДоступ будет разрешен, после подтверждения администратором.",
                "Успешная регистрация", MessageBoxButton.OK, MessageBoxImage.Information);
            RegisterLoginTextBox.Text = string.Empty;
            RegisterPasswordBox.Clear();
            MainTabControl.SelectedIndex = 0;
        }

        // Генерация капчи 
        private void GenerateCaptcha()
        {
            CaptchaDialog.Show();
            Captcha.CreateCaptcha(EasyCaptcha.Wpf.Captcha.LetterOption.Alphanumeric, 6);
            _answerForCaptcha = Captcha.CaptchaText;
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
            if (AnswerTextBox.Text == _answerForCaptcha)
            {
                CaptchaDialog.Hide();
                _attemptCount = 0;
            }
            else
            {
                GenerateCaptcha();
            }
        }
    }
}