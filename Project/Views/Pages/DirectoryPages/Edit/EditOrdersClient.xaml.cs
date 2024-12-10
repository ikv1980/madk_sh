using System.Windows;
using Project.Interfaces;
using Project.Models;
using Project.Tools;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Project.Views.Pages.DirectoryPages.Edit
{
    public partial class EditOrdersClient : UiWindow, IRefresh
    {
        public event Action RefreshRequested;
        private readonly bool _isEditMode;
        private readonly bool _isDeleteMode;
        private readonly int _itemId;
        
        // Конструктор для добавления данных
        public EditOrdersClient()
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
        public EditOrdersClient(OrdersClient item, string button) : this()
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _itemId = item.ClientId;
            NameTextBox.Text = item.ClientName;
            SurnameTextBox.Text = item.ClientSurname;
            
            
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
                NameTextBox.Visibility = Visibility.Visible;
                SurnameTextBox.Visibility = Visibility.Collapsed;
            }
        }

        
        // События после загрузки окна
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NameTextBox.Focus();
        }
        
    } 
}

