using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ModernWpf.Controls;
using Project.Models;
using Project.Tools;

using Page = System.Windows.Controls.Page;

namespace Project.Views.Pages
{
    public partial class SettingPage : Page
    {
        private readonly Db _dbContext = DbUtils.db;
        private bool _isEditMode = false;
        private Dictionary<string, object> _originalValues = new();

        public SettingPage()
        {
            InitializeComponent();
            InitializeDataAsync();
        }

        // Инициализация данных страницы
        private async void InitializeDataAsync()
        {
            await LoadPageAsync();
            await LoadUsersAsync();
        }

        // Загрузка пользователей из базы данных
        private async Task LoadUsersAsync()
        {
            await using var dbContext = new Db();
            UserPermissionsTable.ItemsSource = await dbContext.Users
                .Include(u => u.UsersDepartmentNavigation)
                .Include(u => u.UsersFunctionNavigation)
                .ToListAsync();
        }

        // Загрузка страниц из базы данных
        private async Task LoadPageAsync()
        {
            await using var dbContext = new Db();
            DataTable.ItemsSource = await dbContext.SitePages.ToListAsync();
        }
        
        // Изменение прав доступа пользователя
        private async void EditPermissions_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: User user })
            {
                var permissionWindow = new EditPermission(user);
                if (permissionWindow.ShowDialog() == true)
                {
                    await LoadUsersAsync();
                }
            }
        }

        // Обработка редактирования (вкл/выкл)
        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                _isEditMode = toggleSwitch.IsOn;
                saveButton.IsEnabled = _isEditMode;
                DataTable.IsReadOnly = !_isEditMode;
                DataTable.BorderBrush = _isEditMode ? Brushes.Red : Brushes.Gray;
                DataTable.BorderThickness = _isEditMode ? new Thickness(2) : new Thickness(1);
                if (!_isEditMode) SaveChangesAsync();
            }
        }

        // Сохранение изменений
        private async void SaveChangesAsync()
        {
            if (_dbContext.ChangeTracker.HasChanges())
            {
                await _dbContext.SaveChangesAsync();
                MessageBox.Show("Изменения сохранены", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Создание записи (потом редактируем)
        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var newPage = new SitePage
            {
                PageNumber = 0,
                PageNameEng = "New Page",
                PageNameRus = "Новая Страница",
                PageIcon = "default.png",
                PageShow = true,
            };

            _dbContext.SitePages.Add(newPage);
            await _dbContext.SaveChangesAsync();
            await LoadPageAsync();
        }

        // Удаление записи
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataTable.SelectedItem is SitePage selectedPage)
            {
                _dbContext.SitePages.Remove(selectedPage);
                await _dbContext.SaveChangesAsync();
                await LoadPageAsync();
            }
        }
    }
}