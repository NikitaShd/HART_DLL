using Andriod_Hart.ViewModels.DeviceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Andriod_Hart.Views.Device
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page_MTM701_7 : ContentPage
    {
        public Page_MTM701_7()
        {
            BindingContext = new Model_701_7();
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            header.Text = String.Format("Display Contrast: {0}",Convert.ToInt32(e.NewValue));
        }
    }
}