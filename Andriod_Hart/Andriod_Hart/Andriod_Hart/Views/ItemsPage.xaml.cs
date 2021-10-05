using Andriod_Hart.Models;
using Andriod_Hart.ViewModels;
using Andriod_Hart.Views;
using Android.Support.V7.App;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Andriod_Hart.Views
{
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel _viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new ItemsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        private double width;
        private double height;

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width != this.width || height != this.height)
            {
                AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                this.width = width;
                this.height = height;
                if (width > height)

                {
                    G_bluet.RowDefinitions.Clear();
                    G_bluet.ColumnDefinitions.Clear();
                    G_bluet.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    G_bluet.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    G_bluet.Children.Remove(G_bluet_bundet);
                    G_bluet.Children.Remove(G_bluet_skan);
                    G_bluet.Children.Add(G_bluet_bundet, 0, 0);
                    G_bluet.Children.Add(G_bluet_skan, 1, 0);
                }
                else
                {
                    G_bluet.RowDefinitions.Clear();
                    G_bluet.ColumnDefinitions.Clear();
                    G_bluet.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.5, GridUnitType.Star) });
                    G_bluet.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                    G_bluet.Children.Remove(G_bluet_bundet);
                    G_bluet.Children.Remove(G_bluet_skan);
                    G_bluet.Children.Add(G_bluet_bundet, 0, 0);
                    G_bluet.Children.Add(G_bluet_skan, 0, 1);
                }
            }
        }

    }
}