﻿using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class CarsCountryPage : Page
    {
        public CarsCountryPage()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<CarsCountry>();
        }
    }
}