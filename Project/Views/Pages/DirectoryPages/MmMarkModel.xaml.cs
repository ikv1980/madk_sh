using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class MmMarkModel : Page
    {
        public MmMarkModel()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<Models.MmMarkModel>();
        }
    }
}