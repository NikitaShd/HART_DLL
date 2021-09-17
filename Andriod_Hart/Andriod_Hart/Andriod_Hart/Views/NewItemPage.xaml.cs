using Andriod_Hart.Models;
using Andriod_Hart.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Andriod_Hart.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}