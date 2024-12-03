using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class MmMarkModelPage : Page
    {
        public MmMarkModelPage()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<MmMarkModel>();
        }
    }
}