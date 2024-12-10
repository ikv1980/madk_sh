using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class OrdersClientPage : Page
    {
        public OrdersClientPage()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<OrdersClient>();
        }
    } 
}