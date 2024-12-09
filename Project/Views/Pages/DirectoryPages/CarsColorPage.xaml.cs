using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class CarsColorPage : Page
    {
        public CarsColorPage()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<CarsColor>();
        }
    }
}