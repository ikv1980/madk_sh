﻿using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
   public partial class PgMmModelCountry : Page
   {
       public PgMmModelCountry()
       {
           InitializeComponent();
           this.DataContext = new SomePagesViewModel<Models.MmModelCountry>();
       }
   } 
}

