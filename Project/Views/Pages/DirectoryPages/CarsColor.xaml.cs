using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class CarsColor : Page
    {
        public CarsColor()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<Models.CarsColor>();
        }
    }
}