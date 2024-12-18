using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class MmDepartmentFunctionPage : Page
    {
        public MmDepartmentFunctionPage()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<MmDepartmentFunction>();
        }
    }
}