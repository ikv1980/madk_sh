using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class CarsMark : Page
    {
        public CarsMark()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<Models.CarsMark>();
        }
    }
}

