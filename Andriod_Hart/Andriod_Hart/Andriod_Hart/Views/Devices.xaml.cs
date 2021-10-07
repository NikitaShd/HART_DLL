using Andriod_Hart.ViewModels;
using Andriod_Hart.Views.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Andriod_Hart.Views
{
  
    public partial class Devices : ContentPage
    {
        public Devices()
        {
            BindingContext = new DevicesModel();
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync(nameof(Page_MTM701_7),true);
        }    
    }
}