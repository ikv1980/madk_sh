using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
   public partial class MmModelCountry : Page
   {
       public MmModelCountry()
       {
           InitializeComponent();
           this.DataContext = new SomePagesViewModel<Models.MmModelCountry>();
       }
   } 
}

