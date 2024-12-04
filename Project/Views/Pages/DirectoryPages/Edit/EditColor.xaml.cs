using System.Windows;
using Project.Models;
using Project.Tools;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Project.Views.Pages.DirectoryPages.Edit
{
    public partial class EditColor : UiWindow
    {
        private readonly bool _isEditMode;
        private readonly int _colorId;

        public EditColor()
        {
            InitializeComponent();
            _colorId = -1; // Новый объект
            _isEditMode = false;
        }

        public EditColor(CarsColor color) : this()
        {
            if (color == null) throw new ArgumentNullException(nameof(color));

            _colorId = color.ColorId;
            ColorNameTextBox.Text = color.ColorName;
            _isEditMode = true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsValidInput())
                    return;

                var color = _isEditMode 
                    ? DbUtils.db.CarsColors.FirstOrDefault(x => x.ColorId == _colorId) 
                    : new CarsColor();

                if (color == null)
                {
                    MessageBox.Show("Цвет не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                color.ColorName = ColorNameTextBox.Text.ToLower();

                if (!_isEditMode)
                    DbUtils.db.CarsColors.Add(color);

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
            var colorName = ColorNameTextBox.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(colorName))
            {
                MessageBox.Show("Поле 'Название цвета' не должно быть пустым.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DbUtils.db.CarsColors.Any(x => x.ColorName == colorName && x.ColorId != _colorId))
            {
                MessageBox.Show($"Запись '{ColorNameTextBox.Text}' уже существует в базе.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
    }
}
