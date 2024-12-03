using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class PgCarsModel : Page
    {
        public PgCarsModel()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<Models.CarsModel>();
        }
    }
}
