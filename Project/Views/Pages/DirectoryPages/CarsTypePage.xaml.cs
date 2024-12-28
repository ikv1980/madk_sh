using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class CarsTypePage : Page
    {
        public CarsTypePage()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<CarsType>();
        }
    }
}