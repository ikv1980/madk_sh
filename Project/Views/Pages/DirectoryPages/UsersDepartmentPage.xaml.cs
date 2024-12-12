using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class UsersDepartmentPage: Page
    {
        public UsersDepartmentPage()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<UsersDepartment>();
        }
    }
}