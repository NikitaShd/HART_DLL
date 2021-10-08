using Andriod_Hart.ViewModels;
using Andriod_Hart.Views.Device;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Andriod_Hart
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
              Routing.RegisterRoute(nameof(Page_MTM701_7), typeof(Page_MTM701_7));
           //   Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
          
        }
       
        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Browser.OpenAsync("https://github.com/TviZet/HART_DLL");
        }
        private async void OnMenuItemClicked2(object sender, EventArgs e)
        {
            await Browser.OpenAsync("https://github.com/TviZet/HART_DLL/wiki/Android-Program-Wiki");
        }
    }
}
