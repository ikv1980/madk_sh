﻿using System.Windows;
using Project.Interfaces;
using Project.Models;
using Project.Tools;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Project.Views.Pages.DirectoryPages.Edit
{
    public partial class EditType : UiWindow, IRefresh
    {
        public event Action RefreshRequested;
        private readonly bool _isEditMode;
        private readonly bool _isDeleteMode;
        private readonly int _itemId;
        
        // Конструктор для добавления данных
        public EditType()
        {
            InitializeComponent();
            _itemId = -1;
            _isEditMode = false;
            _isDeleteMode = false;
            Title = "Добавление данных";
            SaveButton.Content = "Добавить";
            SaveButton.Icon = SymbolRegular.AddCircle24;
        }

        // Конструктор для изменения (удаления) данных
        public EditType(CarsType item, string button) : this()
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _itemId = item.TypeId;
            ItemTextBox.Text = item.TypeName;
            
            // изменяем диалоговое окно, в зависимости от нажатой кнопки
            if (button == "Change")
            {
                _isEditMode = true;
                Title = "Изменение данных";
                SaveButton.Content = "Изменить";
                SaveButton.Icon = SymbolRegular.EditProhibited28;
            }
            if (button == "Delete")
            {
                _isDeleteMode = true;
                Title = "Удаление данных";
                SaveButton.Content = "Удалить";
                SaveButton.Icon = SymbolRegular.Delete24;
                TextBlock.Visibility = Visibility.Visible;
                ItemTextBox.Visibility = Visibility.Collapsed;
            }
        }
        
        // Изменение данных
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsValidInput())
                    return;

                var item = (_isEditMode || _isDeleteMode)
                    ? DbUtils.db.CarsTypes.FirstOrDefault(x => x.TypeId == _itemId) 
                    : new Models.CarsType();

                if (item == null)
                {
                    MessageBox.Show("Данные не найдены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Изменение
                if (_isEditMode)
                {
                    item.TypeName = ItemTextBox.Text.Trim();
                }
                // Удаление
                if (_isDeleteMode){
                    item.Delete = true; //DbUtils.db.CarsTypes.Remove(item);
                }
                // Добавление
                if (!_isEditMode && !_isDeleteMode)
                {
                    item.TypeName = ItemTextBox.Text.Trim();
                    DbUtils.db.CarsTypes.Add(item);
                }
                
                DbUtils.db.SaveChanges();
                RefreshRequested?.Invoke();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Закрытие окна
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        // Валидация данных
        private bool IsValidInput()
        {
            var item = ItemTextBox.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(item))
            {
                MessageBox.Show("Поле не должно быть пустым.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DbUtils.db.CarsTypes.Any(x => x.TypeName.Trim().ToLower() == item && x.TypeId != _itemId))
            {
                MessageBox.Show($"Запись '{ItemTextBox.Text}' уже существует в базе.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        // События после загрузки окна
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ItemTextBox.Focus();
        }
    }
}