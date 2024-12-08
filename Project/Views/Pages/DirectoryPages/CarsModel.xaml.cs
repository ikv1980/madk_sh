using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class CarsModel : Page
    {
        public CarsModel()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<Models.CarsModel>();
        }
    }
}
