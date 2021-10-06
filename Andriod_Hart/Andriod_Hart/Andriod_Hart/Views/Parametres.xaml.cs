using Andriod_Hart.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Andriod_Hart.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Parametres : ContentPage
    {
        public Parametres()
        {
            BindingContext = new ParametresModel();
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
               
                if (width > height)
                {
                    G_HART_DEV.RowDefinitions.Clear();
                    G_HART_DEV.ColumnDefinitions.Clear();

                    G_HART_DEV.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
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
                    G_HART_DEV.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
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
    }
}