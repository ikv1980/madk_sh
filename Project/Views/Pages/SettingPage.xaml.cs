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
            LoadPageAsync();
            LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            await using var dbContext = new Db();
            UserPermissionsTable.ItemsSource = await dbContext.Users
                .Include(u => u.UsersDepartmentNavigation)
                .Include(u => u.UsersFunctionNavigation)
                .ToListAsync();
        }

        // Изменение прав доступа пользователя
        private async void EditPermissions_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                // Создаем окно EditPermission, передавая только объект User
                var permissionWindow = new EditPermission(user);
                var result = permissionWindow.ShowDialog();

                if (result == true)
                {
                    await LoadUsersAsync();
                }
            }
        }
        
        // Получение данных из БД
        private async Task LoadPageAsync()
        {
            DataTable.ItemsSource = await _dbContext.SitePages.ToListAsync();
        }

        // Обработка редактирования (вкл/выкл)
        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
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