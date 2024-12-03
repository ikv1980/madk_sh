using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class PgCarsColor : Page
    {
        public PgCarsColor()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<Models.CarsColor>();
        }
    }
}