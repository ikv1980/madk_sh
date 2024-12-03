using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class PgCarsMark : Page
    {
        public PgCarsMark()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<Models.CarsMark>();
        }
    }
}

