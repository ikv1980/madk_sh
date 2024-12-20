using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class OrdersStatusPage : Page
    {
        public OrdersStatusPage()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<OrdersStatus>();
        }
    }
}