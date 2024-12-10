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
            ClientLoginTextBlock.Visibility = Visibility.Collapsed;
            ClientLoginTextBox.Visibility = Visibility.Visible;
            _validator = new ValidateField();
        }

        // Конструктор для изменения (удаления) данных
        public EditOrdersClient(OrdersClient item, string button) : this()
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _oldPassword = item.ClientPassword;
            _itemId = item.ClientId;
            ClientNameTextBox.Text = item.ClientName;
            ClientPhoneTextBox.Text = item.ClientPhone;
            ClientMailTextBox.Text = item.ClientMail;
            ClientAddDataTextBox.Text = item.ClientAddData;
            ClientDateRegistrationTextBlock.Text = item.ClientDateRegistration.ToString("dd.MM.yyyy");
            StatusComboBox.SelectedItem = StatusComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Tag.ToString() == (item.ClientStatus == true ? "1" : "0"));
            ClientLoginTextBlock.Text = item.ClientLogin;
            ClientLoginTextBox.Text = item.ClientLogin;

            // изменяем диалоговое окно, в зависимости от нажатой кнопки
            if (button == "Change")
            {
                _isEditMode = true;
                Title = "Изменение данных";
                SaveButton.Content = "Изменить";
                SaveButton.Icon = SymbolRegular.EditProhibited28;
                ClientLoginTextBlock.Visibility = Visibility.Visible;
                ClientLoginTextBox.Visibility = Visibility.Collapsed;
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

        // Изменение данных
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInputs())
                    return;

                var item = (_isEditMode || _isDeleteMode)
                    ? DbUtils.db.OrdersClients.FirstOrDefault(x => x.ClientId == _itemId)
                    : new OrdersClient();

                if (item == null)
                {
                    MessageBox.Show("Данные не найдены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Изменение
                if (_isEditMode)
                    UpdateItem(item);

                // Удаление
                if (_isDeleteMode)
                    item.Delete = true; //DbUtils.db.OrdersClients.Remove(item);

                // Добавление
                if (!_isEditMode && !_isDeleteMode)
                {
                    item.ClientLogin = ClientLoginTextBox.Text.Trim();
                    UpdateItem(item);
                    DbUtils.db.OrdersClients.Add(item);
                }

                DbUtils.db.SaveChanges();
                RefreshRequested?.Invoke();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Закрытие окна
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Валидация данных
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(ClientNameTextBox.Text))
            {
                MessageBox.Show("Имя клиента не может быть пустым.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            if (!_validator.IsValid(ClientPhoneTextBox.Text, "phone"))
            {
                MessageBox.Show("Некорректный телефон. В номере телефона допускаются цифры и знак +.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!_validator.IsValid(ClientMailTextBox.Text, "email"))
            {
                MessageBox.Show("Некорректный e-mail.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!_isEditMode && !_isDeleteMode)
            {
                var clientLogin = ClientLoginTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(clientLogin))
                {
                    MessageBox.Show("Логин клиента не может быть пустым.", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return false;
                }

                if (DbUtils.db.OrdersClients.Any(x => x.ClientLogin == clientLogin))
                {
                    MessageBox.Show("Клиент с таким логином уже существует.", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return false;
                }
                
                if (!_validator.IsValid(ClientPasswordBox.Password, "password"))
                {
                    MessageBox.Show("Пароль должен содержать не менее 6 символов.", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(ClientPasswordBox.Password.Trim()) &&
                !_validator.IsValid(ClientPasswordBox.Password, "password"))
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // Обновление данных объекта
        private void UpdateItem(OrdersClient item)
        {
            item.ClientName = ClientNameTextBox.Text.Trim();
            item.ClientPhone = ClientPhoneTextBox.Text.Trim();
            item.ClientMail = ClientMailTextBox.Text.Trim();
            item.ClientAddData = ClientAddDataTextBox.Text.Trim();
            item.ClientStatus = ((ComboBoxItem)StatusComboBox.SelectedItem)?.Tag.ToString() == "1";

            if (!string.IsNullOrWhiteSpace(ClientPasswordBox.Password))
            {
                Helpers helper = new Helpers();
                item.ClientPassword = helper.HashPassword(ClientPasswordBox.Password);
            }
            else
            {
                item.ClientPassword = _oldPassword;
            }
        }

        // Фокус на элементе
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ClientNameTextBox.Focus();
        }
    }
}