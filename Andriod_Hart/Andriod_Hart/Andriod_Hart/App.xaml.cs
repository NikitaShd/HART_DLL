using Andriod_Hart.Services;
using Andriod_Hart.Views;
using Android.Content;
using Android.Support.V4.Content;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Andriod_Hart
{
    public partial class App : Application
    {

        public App()
        {
            
            InitializeComponent();
           
            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
            
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
