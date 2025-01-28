using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Project.Models;
using Project.Tools;

namespace Project.ViewModels
{
    public class ReportPageViewModel : INotifyPropertyChanged
    {
        private List<User> _managers;

        public List<User> Managers
        {
            get => _managers;
            set
            {
                _managers = value;
                OnPropertyChanged(nameof(Managers));
            }
        }

        private int _selectedManagerId;

        public int SelectedManagerId
        {
            get => _selectedManagerId;
            set
            {
                _selectedManagerId = value;
                OnPropertyChanged(nameof(SelectedManagerId));
            }
        }

        private DateTime? _startDate;

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        private DateTime? _endDate;

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        private SeriesCollection _salesSeries;

        public SeriesCollection SalesSeries
        {
            get => _salesSeries;
            set
            {
                _salesSeries = value;
                OnPropertyChanged(nameof(SalesSeries));
            }
        }

        public ICommand GenerateReportCommand { get; }
        public ICommand ExportToExcelCommand { get; }
        public ICommand GoBackCommand { get; }

        public ReportPageViewModel()
        {
            GenerateReportCommand = new RelayCommand(param => GenerateReport());
            ExportToExcelCommand = new RelayCommand(param => ExportToExcel());
            GoBackCommand = new RelayCommand(param => GoBack());
            LoadManagers();
        }

        private async void LoadManagers()
        {
            using (var context = new Db())
            {
                Managers = await context.Users
                    .Where(u => u.UsersDepartment == 4) // Фильтр по отделу менеджеров
                    .ToListAsync();
            }
        }

        private void GenerateReport()
        {
            using (var context = new Db())
            {
                var query = context.Orders
                    .Include(o => o.MmOrdersCars) // Включаем связанные автомобили
                    .ThenInclude(m => m.Car)      // Включаем данные об автомобилях
                    .Include(o => o.MmOrdersStatuses) // Включаем статусы заказов
                    .AsQueryable();

                if (SelectedManagerId != 0)
                {
                    query = query.Where(o => o.OrdersUser == SelectedManagerId);
                }

                if (StartDate.HasValue)
                {
                    query = query.Where(o => o.OrdersData >= StartDate.Value);
                }

                if (EndDate.HasValue)
                {
                    query = query.Where(o => o.OrdersData <= EndDate.Value);
                }

                var salesData = query
                    .GroupBy(o => o.OrdersData.Value.Date) // Группируем по дате
                    .Select(g => new
                    {
                        Date = g.Key,
                        Total = g.Sum(o =>
                            o.MmOrdersCars.Sum(m => (decimal)m.Car.CarPrice)) // Преобразуем CarPrice в decimal
                    })
                    .ToList(); // Преобразуем в список

                SalesSeries = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Продажи",
                        Values = new ChartValues<decimal>(salesData.Select(d => d.Total)),
                        DataLabels = true,
                        LabelPoint = point => point.Y.ToString("N2") // Форматируем значения в легенде
                    }
                };
            }
        }

        private void ExportToExcel()
        {
            using (var context = new Db())
            {
                var query = context.Orders
                    .Include(o => o.OrdersClientNavigation) // Включаем данные клиента
                    .Include(o => o.OrdersUserNavigation) // Включаем данные менеджера
                    .Include(o => o.MmOrdersCars) // Включаем связанные автомобили
                    .ThenInclude(m => m.Car) // Включаем данные об автомобилях
                    .AsQueryable();

                if (SelectedManagerId != 0)
                {
                    query = query.Where(o => o.OrdersUser == SelectedManagerId);
                }

                if (StartDate.HasValue)
                {
                    query = query.Where(o => o.OrdersData >= StartDate.Value);
                }

                if (EndDate.HasValue)
                {
                    query = query.Where(o => o.OrdersData <= EndDate.Value);
                }

                var orders = query.ToList();

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Отчет по заказам");

                    worksheet.Cells[1, 1].Value = "ID заказа";
                    worksheet.Cells[1, 2].Value = "Клиент";
                    worksheet.Cells[1, 3].Value = "Менеджер";
                    worksheet.Cells[1, 4].Value = "Дата заказа";
                    worksheet.Cells[1, 5].Value = "Сумма";

                    for (int i = 0; i < orders.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = orders[i].OrdersId;
                        worksheet.Cells[i + 2, 2].Value = orders[i].OrdersClientNavigation?.ClientName ?? "N/A";
                        worksheet.Cells[i + 2, 3].Value = orders[i].OrdersUserNavigation?.UsersName ?? "N/A";
                        worksheet.Cells[i + 2, 4].Value = orders[i].OrdersData;
                        worksheet.Cells[i + 2, 5].Value =
                            orders[i].MmOrdersCars.Sum(m => m.Car.CarPrice); // Сумма цен автомобилей
                    }

                    var fileInfo = new FileInfo("Отчет_по_заказам.xlsx");
                    package.SaveAs(fileInfo);

                    MessageBox.Show("Отчет успешно сохранен!");
                }
            }
        }

        private void GoBack()
        {
            // Логика для возврата на предыдущую страницу
            //var navigationService = System.Windows.Navigation.NavigationService.GetNavigationService(this);
            //navigationService?.GoBack();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}