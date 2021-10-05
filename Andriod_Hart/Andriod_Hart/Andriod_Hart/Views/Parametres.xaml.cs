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
    }
}