using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Project.Interfaces;
using Project.Models;
using Project.Tools;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Project.Views.Pages.DirectoryPages.Edit
{
    public partial class EditOrder : UiWindow, IRefresh
    {
        public event Action RefreshRequested;
        private readonly bool _isEditMode;
        private readonly bool _isDeleteMode;
        private readonly bool _isShowMode;
        private readonly int _itemId;
        private readonly ValidateField _validator;

        // Конструктор для добавления данных
        public EditOrder()
        {
            InitializeComponent();
            Init();
            _itemId = -1;
            _isEditMode = false;
            _isDeleteMode = false;
            Title = "Добавление данных";
            SaveButton.Content = "Добавить";
            SaveButton.Icon = SymbolRegular.AddCircle24;
            _validator = new ValidateField();
            EditOrdersData.SelectedDate = DateTime.Now;
        }

        // Конструктор для изменения (удаления) данных
        public EditOrder(Order item, string button) : this()
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            InitializeComponent();
            Init();
            _itemId = item.OrdersId;

            // Установка значений в форму
            ShowId.Visibility = Visibility.Visible;
            ShowOrdersId.Text = item.OrdersId.ToString();
            EditOrdersClient.SelectedItem =
                DbUtils.db.OrdersClients.FirstOrDefault(m => m.ClientId == item.OrdersClient);
            EditOrdersUsers.SelectedItem =
                DbUtils.db.Users.FirstOrDefault(m => m.UsersId == item.OrdersUser);
            EditOrdersData.SelectedDate = item.OrdersData.HasValue
                ? item.OrdersData.Value.Date
                : DateTime.Now;
            EditOrdersPayment.SelectedItem =
                DbUtils.db.OrdersPayments.FirstOrDefault(m => m.PaymentId == item.OrdersPayment);
            EditOrdersDelivery.SelectedItem =
                DbUtils.db.OrdersDeliveries.FirstOrDefault(m => m.DeliveryId == item.OrdersDelivery);
            EditOrdersAddress.Text = item.OrdersAddress.ToString();

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
            if (button == "Delete")
            {
                _isDeleteMode = true;
                Title = "Удаление данных";
                SaveButton.Content = "Удалить";
                SaveButton.Icon = SymbolRegular.Delete24;
            }
        }

        // Обработка нажатия кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = (_isEditMode || _isDeleteMode)
                    ? DbUtils.db.Orders.FirstOrDefault(x => x.OrdersId == _itemId)
                    : new Order();

                if (item == null)
                {
                    MessageBox.Show("Данные не найдены.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Удаление
                if (_isDeleteMode)
                {
                    item.Delete = true; //DbUtils.db.Orders.Remove(item);   
                }
                else
                {
                    if (!IsValidInput())
                        return;

                    // Изменение или добавление
                    UpdateItem(item);

                    if (!_isEditMode)
                    {
                        DbUtils.db.Orders.Add(item);
                    }
                }

                DbUtils.db.SaveChanges();
                RefreshRequested?.Invoke();
                Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
            EditOrdersClient.ItemsSource = DbUtils.db.OrdersClients.Where(x => !x.Delete).ToList();
            // Только из Отдела продаж
            EditOrdersUsers.ItemsSource = DbUtils.db.Users.Where(x => !x.Delete && x.UsersDepartment == 4).ToList();
            EditOrdersPayment.ItemsSource = DbUtils.db.OrdersPayments.Where(x => !x.Delete).ToList();
            EditOrdersDelivery.ItemsSource = DbUtils.db.OrdersDeliveries.Where(x => !x.Delete).ToList();
        }

        // Валидация данных
        private bool IsValidInput()
        {
            if (EditOrdersClient.SelectedItem == null)
            {
                MessageBox.Show("Не выбран клиент", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (EditOrdersUsers.SelectedItem == null)
            {
                MessageBox.Show("Не выбран менеджер", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (EditOrdersPayment.SelectedItem == null)
            {
                MessageBox.Show("Не выбран тип оплаты", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (EditOrdersDelivery.SelectedItem == null)
            {
                MessageBox.Show("Не выбран тип доставки", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(EditOrdersAddress.Text))
            {
                MessageBox.Show("Требуется заполнить адрес доставки.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // Обновление данных объекта
        private void UpdateItem(Order item)
        {
            item.OrdersClient = (EditOrdersClient.SelectedItem as OrdersClient)?.ClientId ??
                                item.OrdersClient;
            item.OrdersUser = (EditOrdersUsers.SelectedItem as User)?.UsersId ??
                              item.OrdersClient;
            item.OrdersData = EditOrdersData.SelectedDate.HasValue
                ? (DateTime?)EditOrdersData.SelectedDate.Value
                : DateTime.Now;
            item.OrdersPayment = (EditOrdersPayment.SelectedItem as OrdersPayment)?.PaymentId ??
                                 item.OrdersPayment;
            item.OrdersDelivery = (EditOrdersDelivery.SelectedItem as OrdersDelivery)?.DeliveryId ??
                                  item.OrdersDelivery;
            item.OrdersAddress = (item.OrdersDelivery == 1)
                ? "Москва. Основной склад"
                : EditOrdersAddress.Text.Trim();
        }

        // Фокус на элементе
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EditOrdersClient.Focus();
        }

        // Выбор типа доставки
        private void SelectionDelivery(object sender, SelectionChangedEventArgs e)
        {
            var selectDelivery = EditOrdersDelivery.SelectedItem as OrdersDelivery;

            if (selectDelivery != null)
            {
                if (selectDelivery.DeliveryId == 1)
                {
                    EditAddress.Visibility = Visibility.Collapsed;
                    EditOrdersAddress.Text = "Москва. Основной склад";
                }
                else
                {
                    EditAddress.Visibility = Visibility.Visible;
                }
            }
        }
    }
}