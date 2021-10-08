using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android;
using Android.Content;
using Android.Bluetooth;
using Xamarin.Forms;
using Android.Support.V7.App;
using System.Threading.Tasks;
using Android.Util;

namespace Andriod_Hart.Droid
{
    [Activity(Label = "Andriod_Hart", Icon = "@mipmap/ic_launcher",Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
          //  Log.Debug(TAG, "SplashActivity.OnCreate");
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        // Simulates background work that happens behind the splash screen
        async void SimulateStartup()
        {
          //  Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
          //  await Task.Delay(8000); // Simulate a bit of startup work.
            //Log.Debug(TAG, "Startup work is finished - starting MainActivity.");
            StartActivity(new Intent( Android.App.Application.Context, typeof(MainActivity)));
        }
    }

    [Activity( Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private readonly string[] Permissions =
         {
          Manifest.Permission.Bluetooth,
          Manifest.Permission.BluetoothAdmin,
          Manifest.Permission.AccessCoarseLocation,
          Manifest.Permission.AccessFineLocation
         };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            CheckPermissions();
            
            LoadApplication(new App());
          
        }
        private void CheckPermissions()
        {
            bool minimumPermissionsGranted = true;

            foreach (string permission in Permissions)
            {
                if (CheckSelfPermission(permission) != Permission.Granted)
                {
                    minimumPermissionsGranted = false;
                }
            }

            // If any of the minimum permissions aren't granted, we request them from the user
            if (!minimumPermissionsGranted)
            {
                RequestPermissions(Permissions, 0);
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}