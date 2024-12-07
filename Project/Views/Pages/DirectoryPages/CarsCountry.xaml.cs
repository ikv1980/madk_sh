﻿using Project.ViewModels;
using System.Windows.Controls;
using Project.Models;

namespace Project.Views.Pages.DirectoryPages
{
    public partial class CarsCountry : Page
    {
        public CarsCountry()
        {
            InitializeComponent();
            this.DataContext = new SomePagesViewModel<Models.CarsCountry>();
        }
    }
}