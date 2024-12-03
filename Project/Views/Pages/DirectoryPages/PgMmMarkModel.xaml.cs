using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class PgMmMarkModel : Page
    {
        public PgMmMarkModel()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<Models.MmMarkModel>();
        }
    }
}