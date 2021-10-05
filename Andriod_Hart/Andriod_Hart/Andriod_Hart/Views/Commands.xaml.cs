using Andriod_Hart.ViewModels;
using Android.Support.V7.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Andriod_Hart.Views
{
   // [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Commands : ContentPage
    {
        CommandsModel _viewModel;
        public Commands()
        {
            BindingContext = _viewModel = new CommandsModel();
            InitializeComponent();
         
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
                    Flex_L.Wrap = FlexWrap.Wrap;
                    Flex_L.Direction = FlexDirection.Row;
                    Flex_L.AlignItems = FlexAlignItems.Start;
                }
                else
                {
                    Flex_L.Wrap = FlexWrap.NoWrap;
                    Flex_L.Direction = FlexDirection.Column;
                    Flex_L.AlignItems = FlexAlignItems.Stretch;
                }
            }
        }
    }
}