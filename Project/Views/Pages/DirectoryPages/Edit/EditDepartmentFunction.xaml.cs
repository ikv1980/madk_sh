﻿using System.Windows;
using Project.Interfaces;
using Project.Models;
using Project.Tools;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Project.Views.Pages.DirectoryPages.Edit
{
    public partial class EditDepartmentFunction : UiWindow, IRefresh
    {
        public event Action RefreshRequested;
        private readonly bool _isEditMode;
        private readonly bool _isDeleteMode;
        private readonly int _itemId;

        // Конструктор для добавления данных
        public EditDepartmentFunction()
        {
            InitializeComponent();
            Init();
            _itemId = -1;
            _isEditMode = false;
            _isDeleteMode = false;
            Title = "Добавление данных";
            SaveButton.Content = "Добавить";
            SaveButton.Icon = SymbolRegular.AddCircle24;
            EditFunctionName.Width = 255;
            EditDepartmentName.Width = 255;
            ButtonAddFunction.Visibility = Visibility.Visible;
            ButtonAddDepartment.Visibility = Visibility.Visible;
        }

        // Конструктор для изменения (удаления) данных
        public EditDepartmentFunction(MmDepartmentFunction item, string button) : this()
        {
            InitializeComponent();
            Init();
            _itemId = item.Id;
            EditDepartmentName.SelectedItem =
                DbUtils.db.UsersDepartments.FirstOrDefault(m => m.DepartmentId == item.DepartmentId);
            EditFunctionName.SelectedItem =
                DbUtils.db.UsersFunctions.FirstOrDefault(m => m.FunctionId == item.FunctionId);
            EditFunctionName.Width = 300;
            EditDepartmentName.Width = 300;
            ButtonAddFunction.Visibility = Visibility.Collapsed;
            ButtonAddDepartment.Visibility = Visibility.Collapsed;

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
            else if (button == "Delete")
            {
                _isDeleteMode = true;
                Title = "Удаление данных";
                SaveButton.Content = "Удалить";
                DeleteTextBlock.Visibility = Visibility.Visible;
                SaveButton.Icon = SymbolRegular.Delete24;
            }
        }

        // Обработка нажатия кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = (_isEditMode || _isDeleteMode)
                    ? DbUtils.db.MmDepartmentFunctions.FirstOrDefault(x => x.Id == _itemId)
                    : new MmDepartmentFunction();

                if (item == null)
                {
                    MessageBox.Show("Данные не найдены.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Удаление
                if (_isDeleteMode)
                {
                    DbUtils.db.MmDepartmentFunctions.Remove(item);
                }
                else
                {
                    if (!IsValidInput())
                        return;

                    // Изменение или добавление
                    UpdateItem(item);

                    if (!_isEditMode)
                    {
                        DbUtils.db.MmDepartmentFunctions.Add(item);
                    }
                }

                DbUtils.db.SaveChanges();
                RefreshRequested?.Invoke();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Закрытие окна
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Инициализация данных для списков
        private void Init()
        {
            EditDepartmentName.ItemsSource = DbUtils.db.UsersDepartments.Where(x => !x.Delete).ToList();
            EditFunctionName.ItemsSource = DbUtils.db.UsersFunctions.Where(x => !x.Delete).ToList();
        }

        // Валидация данных
        private bool IsValidInput()
        {
            if (EditDepartmentName.SelectedItem == null)
            {
                MessageBox.Show("Не выбран отдел", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (EditFunctionName.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана должность", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DbUtils.db.MmDepartmentFunctions.Any(x =>
                    x.DepartmentId == (int)EditDepartmentName.SelectedValue &&
                    x.FunctionId == (int)EditFunctionName.SelectedValue &&
                    x.Id != _itemId))
            {
                MessageBox.Show("Такая запись уже существует в базе данных.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // Обновление данных объекта
        private void UpdateItem(MmDepartmentFunction item)
        {
            item.DepartmentId = (EditDepartmentName.SelectedItem as UsersDepartment)?.DepartmentId ?? item.DepartmentId;
            item.FunctionId = (EditFunctionName.SelectedItem as UsersFunction)?.FunctionId ?? item.FunctionId;
        }

        // События после загрузки окна
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EditFunctionName.Focus();
        }

        private void AddFunction_Click(object sender, RoutedEventArgs e)
        {
            var addFunction = new EditFunction();
            this.Close();
            addFunction.ShowDialog();
        }

        private void AddDepartment_Click(object sender, RoutedEventArgs e)
        {
            var addDepartment = new EditDepartment();
            this.Close();
            addDepartment.ShowDialog();
        }
    }
}