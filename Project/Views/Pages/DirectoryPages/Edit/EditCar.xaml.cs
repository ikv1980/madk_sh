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
    public partial class EditCar : UiWindow, IRefresh
    {
        public event Action RefreshRequested;
        private readonly bool _isEditMode;
        private readonly bool _isDeleteMode;
        private readonly bool _isShowMode;
        private readonly int _itemId;
        private readonly string _oldPassword;
        private readonly ValidateField _validator;
        private byte[] _carImageBytes;

        // Конструктор для добавления данных
        public EditCar()
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
        }

        // Конструктор для изменения (удаления) данных
        public EditCar(Car item, string button) : this()
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            InitializeComponent();
            Init();
            _itemId = item.CarId;

            // Установка значений в форму
            EditCarMark.SelectedItem =
                DbUtils.db.CarsMarks.FirstOrDefault(m => m.MarkId == item.CarMark);
            EditCarModel.SelectedItem =
                DbUtils.db.CarsModels.FirstOrDefault(m => m.ModelId == item.CarModel);
            EditCarCountry.SelectedItem =
                DbUtils.db.CarsCountries.FirstOrDefault(m => m.CountryId == item.CarCountry);
            EditCarType.SelectedItem =
                DbUtils.db.CarsTypes.FirstOrDefault(m => m.TypeId == item.CarType);
            EditCarColor.SelectedItem =
                DbUtils.db.CarsColors.FirstOrDefault(m => m.ColorId == item.CarColor);
            EditCarVin.Text = item.CarVin;
            EditCarPts.Text = item.CarPts;
            EditCarDate.SelectedDate = item.CarDate.HasValue
                ? item.CarDate.Value.ToDateTime(TimeOnly.MinValue)
                : (DateTime?)null;
            EditCarBlock.SelectedItem = EditCarBlock.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Tag.ToString() == (item.CarBlock == true ? "1" : "0"));
            EditCarBlock.Background = item.CarBlock ? Brushes.Pink : Brushes.LightGreen;
            EditPrice.Text = item.CarPrice.ToString();
            _carImageBytes = item.CarPhoto;
            DisplayImage(item.CarPhoto);

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
                    ? DbUtils.db.Cars.FirstOrDefault(x => x.CarId == _itemId)
                    : new Car();

                if (item == null)
                {
                    MessageBox.Show("Данные не найдены.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Удаление
                if (_isDeleteMode)
                {
                    item.Delete = true; //DbUtils.db.Cars.Remove(item);   
                }
                else
                {
                    if (!IsValidInput())
                        return;

                    // Изменение или добавление
                    UpdateItem(item);

                    if (!_isEditMode)
                    {
                        DbUtils.db.Cars.Add(item);
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
            EditCarMark.ItemsSource = DbUtils.db.CarsMarks.Where(x => !x.Delete).ToList();
            EditCarModel.ItemsSource = DbUtils.db.CarsModels.Where(x => !x.Delete).ToList();
            EditCarCountry.ItemsSource = DbUtils.db.CarsCountries.Where(x => !x.Delete).ToList();
            EditCarType.ItemsSource = DbUtils.db.CarsTypes.Where(x => !x.Delete).ToList();
            EditCarColor.ItemsSource = DbUtils.db.CarsColors.Where(x => !x.Delete).ToList();
        }

        // Валидация данных
        private bool IsValidInput()
        {
            if (EditCarMark.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана марка", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (EditCarModel.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана модель", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (EditCarCountry.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана страна", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (EditCarType.SelectedItem == null)
            {
                MessageBox.Show("Не выбран тип кузова", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (EditCarColor.SelectedItem == null)
            {
                MessageBox.Show("Не выбран цвет", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(EditCarVin.Text))
            {
                MessageBox.Show("Требуется заполнить VIN код", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(EditCarPts.Text))
            {
                MessageBox.Show("Требуется заполнить ПТС", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(EditCarDate.Text))
            {
                MessageBox.Show("Требуется заполнить дату производства", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(EditPrice.Text))
            {
                MessageBox.Show("Требуется указать цену", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!_isEditMode && !_isDeleteMode)
            {
                var vin = EditCarVin.Text.Trim();
                var pts = EditCarPts.Text.Trim();

                if (DbUtils.db.Cars.Any(x => x.CarVin == vin))
                {
                    MessageBox.Show("Такой VIN уже существует", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (DbUtils.db.Cars.Any(x => x.CarPts == pts))
                {
                    MessageBox.Show("Такой ПТС уже существует", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        // Обновление данных объекта
        private void UpdateItem(Car item)
        {
            item.CarMark = (EditCarMark.SelectedItem as CarsMark)?.MarkId ?? item.CarMark;
            item.CarModel = (EditCarModel.SelectedItem as CarsModel)?.ModelId ?? item.CarModel;
            item.CarCountry = (EditCarCountry.SelectedItem as CarsCountry)?.CountryId ?? item.CarCountry;
            item.CarType = (EditCarType.SelectedItem as CarsType)?.TypeId ?? item.CarType;
            item.CarColor = (EditCarColor.SelectedItem as CarsColor)?.ColorId ?? item.CarColor;
            item.CarVin = EditCarVin.Text.Trim().ToUpper();;
            item.CarPts = EditCarPts.Text.Trim().ToUpper();;
            item.CarDate = EditCarDate.SelectedDate.HasValue
                ? DateOnly.FromDateTime(EditCarDate.SelectedDate.Value)
                : (DateOnly?)null;
            item.CarBlock = ((ComboBoxItem)EditCarBlock.SelectedItem)?.Tag.ToString() == "1";
            item.CarPrice = int.TryParse(EditPrice.Text.Trim(), out int price) ? price : 0;
            item.CarPhoto = EditCarImage.Source != null ? _carImageBytes : null;
        }

        // Фокус на элементе
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EditCarMark.Focus();
        }

        // Выбор марки ТС
        private void SelectionMark(object sender, SelectionChangedEventArgs e)
        {
            EditCarModel.IsEnabled = true;

            var selectMark = EditCarMark.SelectedItem as CarsMark;

            if (selectMark != null)
            {
                var models = DbUtils.db.MmMarkModels
                    .Where(x => x.MarkId == selectMark.MarkId)
                    .Select(x => x.Model)
                    .ToList();

                if (models.Any())
                {
                    EditCarModel.ItemsSource = models;
                }
                else
                {
                    EditCarModel.ItemsSource = null;
                    MessageBox.Show("Для выбранной марки нет доступных моделей.", "Информация");
                }
            }
            else
            {
                EditCarModel.ItemsSource = null;
                EditCarModel.IsEnabled = false;
            }
        }

        // Выбор страны производства
        private void SelectionModel(object sender, SelectionChangedEventArgs e)
        {
            EditCarCountry.IsEnabled = true;

            var selectMark = EditCarMark.SelectedItem as CarsMark;
            var selectModel = EditCarModel.SelectedItem as CarsModel;

            if (selectMark != null && selectModel != null)
            {
                var countries = DbUtils.db.MmMarkModels
                    .Where(x => x.MarkId == selectMark.MarkId && x.ModelId == selectModel.ModelId)
                    .Select(x => x.Country)
                    .ToList();

                if (countries.Count > 1)
                {
                    EditCarCountry.ItemsSource = countries;
                    EditCarCountry.SelectedItem = null;
                }
                else if (countries.Count == 1)
                {
                    EditCarCountry.ItemsSource = countries;
                    EditCarCountry.SelectedItem = countries.First();
                }
                else
                {
                    EditCarCountry.ItemsSource = null;
                    MessageBox.Show("Для выбранной марки и модели нет данных о странах производства.", "Информация");
                }
            }
            else
            {
                EditCarCountry.ItemsSource = null;
                EditCarCountry.IsEnabled = false;
            }
        }

        // Добавление фотографии
        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            // Открываем диалог выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Выберите изображение автомобиля"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Загружаем изображение
                    string filePath = openFileDialog.FileName;
                    BitmapImage bitmap = new BitmapImage(new Uri(filePath));
                    EditCarImage.Source = bitmap;

                    // Сжимаем изображение и конвертируем в массив байтов
                    //_carImageBytes = File.ReadAllBytes(filePath);
                    _carImageBytes = CompressImage(filePath, 50);
                    //_carImageBytes = File.ReadAllBytes(filePath);

                    MessageBox.Show("Фото успешно загружено и сохранено в базе данных.", "Успех", 
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении фото: {ex.Message}", "Ошибка", 
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        // Сжатие изображения
        private byte[] CompressImage(string filePath, int quality)
        {
            using (var originalImage = System.Drawing.Image.FromFile(filePath))
            {
                var jpegEncoder = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()
                    .FirstOrDefault(codec => codec.FormatID == System.Drawing.Imaging.ImageFormat.Jpeg.Guid);
                if (jpegEncoder == null)
                    throw new Exception("JPEG encoder not found");

                var encoderParameters = new System.Drawing.Imaging.EncoderParameters(1)
                {
                    Param = { [0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality) }
                };

                using (var memoryStream = new MemoryStream())
                {
                    originalImage.Save(memoryStream, jpegEncoder, encoderParameters);
                    return memoryStream.ToArray();
                }
            }
        }
        
        // Отображение фотографии
        private void DisplayImage(byte[] imageBytes)
        {
            if (imageBytes != null && imageBytes.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    EditCarImage.Source = bitmap;
                }
            }
            else
            {
                MessageBox.Show("Фото отсутствует.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Цена: ввод только цифр
        private void EditPrice_Input(object sender, TextCompositionEventArgs e)
        {
            char inputChar = e.Text[0];
            if (!char.IsDigit(inputChar))
            {
                e.Handled = true; // Отменяем ввод, если не цифра
            }
        }
    }
}