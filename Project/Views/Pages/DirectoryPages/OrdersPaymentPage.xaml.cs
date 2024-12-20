using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class OrdersPaymentPage : Page
    {
        public OrdersPaymentPage()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<OrdersPayment>();
        }
    }
}