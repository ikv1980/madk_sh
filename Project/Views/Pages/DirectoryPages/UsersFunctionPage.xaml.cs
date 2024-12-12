using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class UsersFunctionPage: Page
    {
        public UsersFunctionPage()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<UsersFunction>();
        }
    }
}