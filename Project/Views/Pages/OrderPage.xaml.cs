using System.Windows.Controls;
using Project.ViewModels;

namespace Project.Views.Pages
{
    public partial class OrderPage : Page
    {
        public OrderPage()
        {
            InitializeComponent();
            this.DataContext = new OrderPageViewModel();
        }
    }
}
