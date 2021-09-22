using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Andriod_Hart.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public Command Comand_0 { get; }
     
        public AboutViewModel()
        {
            Title = "HART Devises";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            Comand_0 = new Command(OnAddItem);
        }
        private async void OnAddItem()
        {
            _Conect.Read_Fraim[] temp = new Class_HART.Conect.Read_Fraim[] { };
            await Task.Factory.StartNew(async () =>
            {
                lock (balanceLock)
                {
                  temp = Hart_conection.Comand_0_F(0, 0);
                }

            });
            await Task.Delay(100);
        }
        public ICommand OpenWebCommand { get; }
     
    }
}