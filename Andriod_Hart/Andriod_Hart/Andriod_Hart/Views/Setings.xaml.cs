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
    public partial class Setings : ContentPage
    {
        SetingsModel _viewModel;
        public Setings()
        {
            BindingContext = _viewModel = new SetingsModel();
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

    }
}