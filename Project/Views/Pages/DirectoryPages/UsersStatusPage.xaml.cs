using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class UsersStatusPage: Page
    {
        public UsersStatusPage()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<UsersStatus>();
        }
    }
}