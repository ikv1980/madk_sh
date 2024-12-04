using System.Windows;
using Project.Models;
using Project.Tools;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Project.Views.Pages.DirectoryPages.Edit
{
    public partial class EditCountry : UiWindow
    {
        private readonly bool _isEditMode;
        private readonly int _countryId;

        public EditCountry()
        {
            InitializeComponent();
            _countryId = -1;
            _isEditMode = false;
        }

        public EditCountry(CarsCountry country) : this()
        {
            if (country == null) throw new ArgumentNullException(nameof(country));

            _countryId = country.CountryId;
            CountryNameTextBox.Text = country.CountryName;
            _isEditMode = true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsValidInput())
                    return;

                var country = _isEditMode 
                    ? DbUtils.db.CarsCountries.FirstOrDefault(x => x.CountryId == _countryId) 
                    : new CarsCountry();

                if (country == null)
                {
                    MessageBox.Show("Цвет не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                country.CountryName = CountryNameTextBox.Text.ToLower();

                if (!_isEditMode)
                    DbUtils.db.CarsCountries.Add(country);

                DbUtils.db.SaveChanges();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Валидация данных
        private bool IsValidInput()
        {
            var countryName = CountryNameTextBox.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(countryName))
            {
                MessageBox.Show("Поле 'Название цвета' не должно быть пустым.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DbUtils.db.CarsCountries.Any(x => x.CountryName == countryName && x.CountryId != _countryId))
            {
                MessageBox.Show($"Запись '{CountryNameTextBox.Text}' уже существует в базе.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
    }
}
