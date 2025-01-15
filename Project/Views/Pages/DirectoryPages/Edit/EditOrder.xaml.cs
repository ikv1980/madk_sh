using System.Collections.ObjectModel;
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
    public partial class EditOrder : UiWindow, IRefresh
    {
        public event Action RefreshRequested;
        private readonly bool _isEditMode;
        private readonly bool _isDeleteMode;
        private readonly int _itemId;
        private readonly int _currentStatus;

        // Конструктор для добавления данных
        public EditOrder()
        {
            InitializeComponent();
            Init();
            _itemId = -1;
            _isEditMode = false;
            _isDeleteMode = false;
            // Установка значений в форму
            Title = "Добавление данных";
            SaveButton.Content = "Добавить";
            SaveButton.Icon = SymbolRegular.AddCircle24;
            ShowStatus.Visibility = Visibility.Collapsed;
            EditOrdersStatus.Visibility = Visibility.Collapsed;
            EditOrdersStatus.SelectedItem = DbUtils.db.OrdersStatuses
                .FirstOrDefault(m => m.OrderStatusId == 1);
            EditOrdersData.SelectedDate = DateTime.Now;
            EditOrdersUsers.SelectedItem = DbUtils.db.Users
                .FirstOrDefault(m => m.UsersId == Global.CurrentUser.UsersId);
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
            ShowOrdersData.Text = item.OrdersData.HasValue
                ? item.OrdersData.Value.ToString("dd.MM.yyyy")
                : DateTime.Now.ToString("dd.MM.yyyy");
            EditOrdersData.SelectedDate = item.OrdersData.HasValue
                ? item.OrdersData.Value.Date
                : DateTime.Now;
            EditOrdersPayment.SelectedItem =
                DbUtils.db.OrdersPayments.FirstOrDefault(m => m.PaymentId == item.OrdersPayment);
            EditOrdersDelivery.SelectedItem =
                DbUtils.db.OrdersDeliveries.FirstOrDefault(m => m.DeliveryId == item.OrdersDelivery);
            EditOrdersAddress.Text = item.OrdersAddress;
            // Получение статуса Заказа
            ShowStatus.Visibility = Visibility.Visible;
            EditOrdersStatus.Visibility = Visibility.Visible;
            var latestStatusId = item.MmOrdersStatuses
                .OrderByDescending(s => s.Date)
                .FirstOrDefault()?.StatusId;
            _currentStatus = (int)latestStatusId;
            EditOrdersStatus.SelectedItem = 
                DbUtils.db.OrdersStatuses.FirstOrDefault(m => m.OrderStatusId == latestStatusId);

            // изменяем диалоговое окно, в зависимости от нажатой кнопки
            if (button == "Change")
            {
                _isEditMode = true;
                Title = "Изменение данных";
                SaveButton.Content = "Изменить";
                SaveButton.Icon = SymbolRegular.EditProhibited28;
                ShowOrdersData.Visibility = Visibility.Collapsed;
                EditOrdersData.Visibility = Visibility.Visible;
            }
            else if (button == "Show")
            {
                _isEditMode = true;
                Title = "Просмотр данных";
                SaveButton.Visibility = Visibility.Collapsed;
                ShowOrdersData.Visibility = Visibility.Visible;
                EditOrdersData.Visibility = Visibility.Collapsed;
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
                    
                    DbUtils.db.SaveChanges();
                    
                    // Сохранение нового статуса
                    var selectedStatus = (EditOrdersStatus.SelectedItem as OrdersStatus)?.OrderStatusId;

                    if (selectedStatus == null)
                    {
                        selectedStatus = 1; // Статус "Создан"
                    }
                    
                    if (_currentStatus != selectedStatus.Value)
                    {
                        var newStatus = new MmOrdersStatus
                        {
                            OrderId = item.OrdersId,
                            StatusId = selectedStatus.Value,
                            Date = DateTime.Now
                        };

                        DbUtils.db.MmOrdersStatuses.Add(newStatus);
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

        // Инициализация данных
        private void Init()
        {
            EditOrdersClient.ItemsSource = DbUtils.db.OrdersClients.Where(x => !x.Delete).ToList();
            // Только из Отдела продаж
            EditOrdersUsers.ItemsSource = DbUtils.db.Users.Where(x => !x.Delete && x.UsersDepartment == 4).ToList();
            EditOrdersPayment.ItemsSource = DbUtils.db.OrdersPayments.Where(x => !x.Delete).ToList();
            EditOrdersDelivery.ItemsSource = DbUtils.db.OrdersDeliveries.Where(x => !x.Delete).ToList();
            EditOrdersStatus.ItemsSource = DbUtils.db.OrdersStatuses.Where(x => !x.Delete).ToList();
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
            
            if (EditOrdersStatus.SelectedItem == null)
            {
                MessageBox.Show("Не выбран статус заказа", "Ошибка",
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
                    EditAddressName.Visibility = Visibility.Collapsed;
                    EditAddressData.Visibility = Visibility.Collapsed;
                    EditOrdersAddress.Text = "Москва. Основной склад";
                }
                else
                {
                    EditAddressName.Visibility = Visibility.Visible;
                    EditAddressData.Visibility = Visibility.Visible;
                }
            }
        }

        private void AddClient(object sender, RoutedEventArgs e)
        {
            var addClient = new EditOrdersClient();
            this.Close();
            addClient.ShowDialog();
        }
    }
}