using Andriod_Hart.Models;
using Andriod_Hart.ViewModels;

using Android.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Xaml;


namespace Andriod_Hart.Views
{
    
    public partial class BaseInfo : ContentPage
    {
        public Item Item { get; set; }
        BaseInfoModel _viewModel;
        public BaseInfo()
        {
            InitializeComponent();
            BindingContext = _viewModel = new BaseInfoModel();
            //  BindingContext = new BaseInfoModel();
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
                    S_container.Orientation = StackOrientation.Horizontal;
                }
                else
                {
                    S_container.Orientation = StackOrientation.Vertical;
                }
            }
        }
    }
}