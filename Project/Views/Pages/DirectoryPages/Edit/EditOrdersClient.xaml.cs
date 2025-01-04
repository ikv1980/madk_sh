using System.Windows;
using System.Windows.Controls;
using Project.Interfaces;
using Project.Models;
using Project.Tools;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Project.Views.Pages.DirectoryPages.Edit
{
    public partial class EditOrdersClient : UiWindow, IRefresh
    {
        public event Action RefreshRequested;
        private readonly bool _isEditMode;
        private readonly bool _isDeleteMode;
        private readonly int _itemId;
        private readonly string _oldPassword;
        private readonly ValidateField _validator;

        // Конструктор для добавления данных
        public EditOrdersClient()
        {
            InitializeComponent();
            _itemId = -1;
            _isEditMode = false;
            _isDeleteMode = false;
            Title = "Добавление данных";
            SaveButton.Content = "Добавить";
            SaveButton.Icon = SymbolRegular.AddCircle24;
            ShowClientLogin.Visibility = Visibility.Collapsed;
            EditClientLogin.Visibility = Visibility.Visible;
            _validator = new ValidateField();
        }

        // Конструктор для изменения (удаления) данных
        public EditOrdersClient(OrdersClient item, string button) : this()
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _oldPassword = item.ClientPassword;
            _itemId = item.ClientId;
            EditClientName.Text = item.ClientName;
            EditClientPhone.Text = item.ClientPhone;
            EditClientMail.Text = item.ClientMail;
            EditClientAddData.Text = item.ClientAddData;
            ClientDateRegistrationTextBlock.Text = item.ClientDateRegistration.ToString("dd.MM.yyyy");
            EditClientStatus.SelectedItem = EditClientStatus.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Tag.ToString() == (item.ClientStatus == true ? "1" : "0"));
            ShowClientLogin.Text = item.ClientLogin;
            EditClientLogin.Text = item.ClientLogin;

            // изменяем диалоговое окно, в зависимости от нажатой кнопки
            if (button == "Change")
            {
                _isEditMode = true;
                Title = "Изменение данных";
                SaveButton.Content = "Изменить";
                SaveButton.Icon = SymbolRegular.EditProhibited28;
                ShowClientLogin.Visibility = Visibility.Visible;
                EditClientLogin.Visibility = Visibility.Collapsed;
            }
            else if (button == "Show")
            {
                _isEditMode = true;
                Title = "Просмотр данных";
                SaveButton.Visibility = Visibility.Collapsed;
                ShowClientLogin.Visibility = Visibility.Visible;
                EditClientLogin.Visibility = Visibility.Collapsed;
            }

            if (button == "Delete")
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
                    ? DbUtils.db.OrdersClients.FirstOrDefault(x => x.ClientId == _itemId)
                    : new OrdersClient();

                if (item == null)
                {
                    MessageBox.Show("Данные не найдены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Удаление
                if (_isDeleteMode)
                {
                    item.Delete = true; //DbUtils.db.OrdersClients.Remove(item); 
                }
                else
                {
                    if (!IsValidInput())
                        return;
                }

                // Изменение
                if (_isEditMode)
                    UpdateItem(item);

                // Добавление
                if (!_isEditMode && !_isDeleteMode)
                {
                    item.ClientLogin = EditClientLogin.Text.Trim();
                    UpdateItem(item);
                    DbUtils.db.OrdersClients.Add(item);
                }

                DbUtils.db.SaveChanges();
                RefreshRequested?.Invoke();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Закрытие окна
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Валидация данных
        private bool IsValidInput()
        {
            if (string.IsNullOrWhiteSpace(EditClientName.Text))
            {
                MessageBox.Show("Имя клиента не может быть пустым.", "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            if (!_validator.IsValid(EditClientPhone.Text, "phone"))
            {
                MessageBox.Show("Некорректный телефон. В номере телефона допускаются цифры и знак +.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!_validator.IsValid(EditClientMail.Text, "email"))
            {
                MessageBox.Show("Некорректный e-mail.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!_isEditMode && !_isDeleteMode)
            {
                var clientLogin = EditClientLogin.Text.Trim();

                if (string.IsNullOrWhiteSpace(clientLogin))
                {
                    MessageBox.Show("Логин клиента не может быть пустым.", "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return false;
                }

                if (DbUtils.db.OrdersClients.Any(x => x.ClientLogin == clientLogin))
                {
                    MessageBox.Show("Клиент с таким логином уже существует.", "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return false;
                }

                if (!_validator.IsValid(EditClientPassword.Password, "password"))
                {
                    MessageBox.Show("Пароль должен содержать не менее 6 символов.", "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(EditClientPassword.Password.Trim()) &&
                !_validator.IsValid(EditClientPassword.Password, "password"))
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов.", "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // Обновление данных объекта
        private void UpdateItem(OrdersClient item)
        {
            item.ClientName = EditClientName.Text.Trim();
            item.ClientPhone = EditClientPhone.Text.Trim();
            item.ClientMail = EditClientMail.Text.Trim();
            item.ClientAddData = EditClientAddData.Text.Trim();
            item.ClientStatus = ((ComboBoxItem)EditClientStatus.SelectedItem)?.Tag.ToString() == "1";

            if (!string.IsNullOrWhiteSpace(EditClientPassword.Password))
            {
                Helpers helper = new Helpers();
                item.ClientPassword = helper.HashPassword(EditClientPassword.Password);
            }
            else
            {
                item.ClientPassword = _oldPassword;
            }
        }

        // Фокус на элементе
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EditClientName.Focus();
        }
    }
}