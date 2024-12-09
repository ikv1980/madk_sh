using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class CarsModelPage : Page
    {
        public CarsModelPage()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<CarsModel>();
        }
    }
}
