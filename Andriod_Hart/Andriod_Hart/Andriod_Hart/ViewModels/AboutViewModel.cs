using Android;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Andriod_Hart.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
       
        public Command _Comand_0_F { get; set; }
        public Command _Comand_0_F_stop { get; set; }
        public Command _Comand_0_F_adres { get; set; }
        public ObservableCollection<Models.DEVISE> DEV_list { get; } = new ObservableCollection<Models.DEVISE> { };
        public Command<Models.DEVISE> ItemTapped_dev { get; }
        List<string> numbers = new List<string>();
        bool testes = true;
        bool Bloc_butoon = true;
        bool ckan_cancel = false;
        public AboutViewModel()
        {
            Title = "HART";
            for (int i = 0; i <= 15; i++)
            {
                numbers.Add(i.ToString());
            }
            
            ItemTapped_dev = new Command<Models.DEVISE>(execute: (Models.DEVISE dEVISE) =>
            {
                OnItemSelected(dEVISE);
                ItemTapped_dev.ChangeCanExecute();
            },
            canExecute: (Models.DEVISE dEVISE) => testes);
            _Comand_0_F = new Command(execute: () =>
            {
                ckan_cancel = false;
                Comand_0_F(); 
            },
            canExecute: () => testes);
            _Comand_0_F_adres = new Command(execute: () =>
            {
                Comand_0_F_adres();
            },
           canExecute: () => Bloc_butoon);
            _Comand_0_F_stop = new Command(execute: () =>
            {
                ckan_cancel = true;
            },
            canExecute: () => !testes);
        }
        async void OnItemSelected(Models.DEVISE item)
        {
            Title = item.Long_Address;
           // Application.Current.MainPage.DisplayAlert("123", "123","123S");
           
        }
        private async void Comand_0_F_adres()
        {
           
            string action = await Application.Current.MainPage.DisplayActionSheet("ActionSheet: Send to?", "Cancel", null, numbers.ToArray());
            if (action != "Cancel")
            {
                Bloc_butoon = false;
                _Comand_0_F_adres.ChangeCanExecute();
                await Task.Factory.StartNew(async () =>
                {
                    lock (balanceLock)
                    {

                        Class_HART.Conect.Read_Fraim[] temp = Hart_conection.Comand_0_F(Master_ID, Convert.ToInt32(action));
                        foreach (var item in Models.DEVISE.Converter(temp))
                        {
                            DEV_list.Add(item);
                        }

                    }
                });
                Bloc_butoon = true;
                _Comand_0_F_adres.ChangeCanExecute();
            }
        }
        private async void Comand_0_F()
        {
            DEV_list.Clear();
            testes = false;
            _Comand_0_F_stop.ChangeCanExecute();
            _Comand_0_F.ChangeCanExecute();
            ItemTapped_dev.ChangeCanExecute();
            await Task.Factory.StartNew(async () =>
            {
                lock (balanceLock)
                {
                    for (int i = 0; i <= 15; i++)
                    {
                        if (ckan_cancel) return;
                        Class_HART.Conect.Read_Fraim[] temp = Hart_conection.Comand_0_F(Master_ID, 0);
                        foreach (var item in Models.DEVISE.Converter(temp))
                        {
                            DEV_list.Add(item);
                        }
                    }
                }
            });
            testes = true;
            ItemTapped_dev.ChangeCanExecute();
            _Comand_0_F.ChangeCanExecute();
            _Comand_0_F_stop.ChangeCanExecute();
        }

    }
}