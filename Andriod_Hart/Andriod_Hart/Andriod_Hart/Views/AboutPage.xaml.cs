using Android.Support.V7.App;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Andriod_Hart.Views
{
    public partial class AboutPage : ContentPage
    {
       
        public AboutPage()
        {
            InitializeComponent();
          
        }
        private double width;
        private double height;

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width != this.width || height != this.height)
            {
                this.width = width;
                this.height = height;
                AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                if (width > height)
                {
                    G_HART_DEV.RowDefinitions.Clear();
                    G_HART_DEV.ColumnDefinitions.Clear();
                   
                    G_HART_DEV.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.4, GridUnitType.Star) });
                    G_HART_DEV.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    G_HART_DEV.Children.Remove(controlsGrid);
                    G_HART_DEV.Children.Remove(controlsGrid_butons);
                    controlsGrid_butons.CornerRadius = new CornerRadius(0, 0, 0, 0);
                    controlsGrid_butons.Margin = new Thickness(-5, 0, 0, 0);
                    G_HART_DEV.Children.Add(controlsGrid, 0, 0);
                    G_HART_DEV.Children.Add(controlsGrid_butons, 1, 0);
                }
                else
                {
                    G_HART_DEV.RowDefinitions.Clear();
                    G_HART_DEV.ColumnDefinitions.Clear();
                    G_HART_DEV.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.7, GridUnitType.Star) });
                    G_HART_DEV.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    G_HART_DEV.Children.Remove(controlsGrid);
                    G_HART_DEV.Children.Remove(controlsGrid_butons);
                    controlsGrid_butons.CornerRadius = new CornerRadius(20, 20, 0, 0);
                    controlsGrid_butons.Margin = new Thickness(0, -20, 0, 0);
                    G_HART_DEV.Children.Add(controlsGrid, 0, 0);
                    G_HART_DEV.Children.Add(controlsGrid_butons, 0, 1);
                }
            }
        }



        private void Button_Clicked(object sender, EventArgs e)
        {
           
        }

        private void controlsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            return;
        }
    }
}