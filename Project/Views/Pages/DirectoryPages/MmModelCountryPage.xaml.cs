using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
   public partial class MmModelCountryPage : Page
   {
       public MmModelCountryPage()
       {
           InitializeComponent();
           this.DataContext = new SomePagesViewModel<MmModelCountry>();
       }
   } 
}

