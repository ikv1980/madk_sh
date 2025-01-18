using Project.Tools;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Project.Interfaces;
using Project.Models;

namespace Project.ViewModels
{
    internal class OrderPageViewModel : SomePagesViewModel<Order>
    {
        public ObservableCollection<Order> Orders { get; set; }
        public ObservableCollection<Car> SelectedOrderCars { get; set; }
        public ObservableCollection<MmOrdersStatus> SelectedOrderStatuses { get; set; }

        private Order _selectedOrder;

        public Visibility OrderDetailsVisibility => SelectedOrder != null ? Visibility.Visible : Visibility.Collapsed;

        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (_selectedOrder != value)
                {
                    _selectedOrder = value;
                    OnPropertyChanged(nameof(SelectedOrder));
                    LoadOrderDetails();
                }
            }
        }

        public OrderPageViewModel()
        {
            SelectedOrderCars = new ObservableCollection<Car>();
            SelectedOrderStatuses = new ObservableCollection<MmOrdersStatus>();

            // Инициализация данных
            InitializeOrdersAsync();
        }

        private async void InitializeOrdersAsync()
        {
            var orders = await DbUtils.GetTablePagedValuesWithIncludes<Order>(1, 20);
            Orders = new ObservableCollection<Order>(orders);
        }

        protected override async Task Refresh()
        {
            await base.Refresh(); // Загружаем базовые данные

            var orderValues = TableValue.Cast<Order>().Where(order =>
            {
                var lastStatus = order.MmOrdersStatuses
                    .OrderByDescending(status => status.Date)
                    .FirstOrDefault();

                if (lastStatus != null)
                {
                    if (lastStatus.StatusId == 4)
                        return false;

                    if (lastStatus.StatusId == 5)
                    {
                        var daysSinceCompleted = (DateTime.Now - lastStatus.Date).TotalDays;
                        if (daysSinceCompleted >= 5)
                            return false;
                    }
                }

                return true;
            }).ToList();

            TableValue = new ObservableCollection<Order>(orderValues);
        }

        private void LoadOrderDetails()
        {
            // Если заказ выбран, загрузим связанные данные
            if (SelectedOrder != null)
            {
                SelectedOrderCars.Clear();
                SelectedOrderStatuses.Clear();

                // Загрузка автомобилей
                var cars = DbUtils.db.MmOrdersCars
                    .Include(m => m.Car)
                    .ThenInclude(c => c.CarMarkNavigation)
                    .Include(m => m.Car.CarModelNavigation)
                    .Include(m => m.Car.CarColorNavigation)
                    .Where(m => m.OrderId == SelectedOrder.OrdersId)
                    .Select(m => m.Car)
                    .ToList();

                foreach (var car in cars)
                    SelectedOrderCars.Add(car);

                // Загрузка статусов
                var statuses = DbUtils.db.MmOrdersStatuses
                    .Include(s => s.Status)
                    .Where(s => s.OrderId == SelectedOrder.OrdersId)
                    .OrderBy(s => s.Date)
                    .ToList();

                foreach (var status in statuses)
                    SelectedOrderStatuses.Add(status);

                // Уведомление об изменении видимости
                OnPropertyChanged(nameof(OrderDetailsVisibility));
            }
        }
    }
}