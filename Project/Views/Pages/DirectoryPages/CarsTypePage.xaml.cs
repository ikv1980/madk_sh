using System.Windows.Controls;
using Project.Models;
using Project.ViewModels;

namespace Project.Views.Pages.DirectoryPages;

public partial class CarsTypePage : Page
{
    public CarsTypePage()
    {
        InitializeComponent();
        this.DataContext = new SomePagesViewModel<CarsType>();
    }
}