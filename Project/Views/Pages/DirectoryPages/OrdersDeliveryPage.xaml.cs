using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class OrdersDeliveryPage : Page
    {
        public OrdersDeliveryPage()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<OrdersDelivery>();
        }
    }
}