using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Project.Tools;

namespace Project.Views.Pages
{
    public partial class SettingPage : Page
    {
        private readonly Db _dbContext = DbUtils.db;
        private bool _isEditMode = false;

        public SettingPage()
        {
            InitializeComponent();
            LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            DataTable.ItemsSource = await _dbContext.SitePages.ToListAsync();
        }

        private void EditModeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _isEditMode = true;
            DataTable.IsReadOnly = false;
        }

        private void EditModeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _isEditMode = false;
            DataTable.IsReadOnly = true;
            SaveChangesAsync(); // Save changes when edit mode is disabled
        }

        private async void SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
            MessageBox.Show("Изменения сохранены", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            //var createPage = new CreateReсord(); // Navigate to create page dialog
            //createPage.ShowDialog();
            LoadDataAsync(); // Reload data after creation
        }

        private void DataTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_isEditMode)
                e.Row.Background = System.Windows.Media.Brushes.LightYellow;
        }
    }
}