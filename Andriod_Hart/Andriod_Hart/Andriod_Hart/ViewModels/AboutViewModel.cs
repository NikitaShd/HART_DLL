using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Andriod_Hart.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        List<string> bluet { get; } = new List<string> { "1", "2", "3" };
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
        }
        
        public ICommand OpenWebCommand { get; }
    }
}