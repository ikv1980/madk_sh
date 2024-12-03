using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class PgCarsCountry : Page
    {
        public PgCarsCountry()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<Models.CarsCountry>();
        }
    }
}