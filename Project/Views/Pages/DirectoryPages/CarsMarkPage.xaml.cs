using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class CarsMarkPage : Page
    {
        public CarsMarkPage()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<CarsMark>();
        }
    }
}

