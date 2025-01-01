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
            Orders = new ObservableCollection<Order>(
                DbUtils.GetTablePagedValuesWithIncludes<Order>(1, 20)
            );
            SelectedOrderCars = new ObservableCollection<Car>();
            SelectedOrderStatuses = new ObservableCollection<MmOrdersStatus>();
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
            }
        }
    }
}