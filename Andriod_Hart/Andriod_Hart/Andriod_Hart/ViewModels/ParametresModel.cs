using Andriod_Hart.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Andriod_Hart.ViewModels
{
    public class ParametresModel : BaseViewModel
    {
      
        public ObservableCollection<Param> Param_list { get; } = new ObservableCollection<Param> { };
        public Command Comand_Param_start { get; set; }
        public Command Comand_Param_stop  { get; set; }
        public Command Comand_Param_get { get; set; }
        bool comand_Param = true;
        public bool Comand_Param { get => comand_Param; set { comand_Param = value; OnPropertyChanged(); } }
        bool timers = true;
        public ParametresModel()
       {
            Comand_Param_get = new Command(Param_get, canExecute: () => Comand_Param);
            Comand_Param_start = new Command(Param_timers_start, canExecute: () => Comand_Param);
            Comand_Param_stop = new Command(Param_timers_stop);
            Title = "Parameteres";
       }

        async void Param_get()
        {
            Comand_Param = false;
            Comand_Param_get.ChangeCanExecute();
            Comand_Param_start.ChangeCanExecute();
            await Parameters_read();
            Comand_Param = true;
            Comand_Param_start.ChangeCanExecute();
            Comand_Param_get.ChangeCanExecute();
        }
        async void Param_timers_start()
        {
            Comand_Param = false;
            Comand_Param_get.ChangeCanExecute();
            Comand_Param_start.ChangeCanExecute();
            timers = true;
            while (timers)
            {
                await Parameters_read();
                await Task.Delay(1000);
            }
            Comand_Param = true;
            Comand_Param_start.ChangeCanExecute();
            Comand_Param_get.ChangeCanExecute();
        }
        void Param_timers_stop()
        {
            timers = false;   
        }
        async Task Parameters_read()
        {
            try
            {
                int i = 0;
                float tok = 0; float proc = 0;
                string kod_1 = ""; float par_1 = 0;
                string kod_2 = ""; float par_2 = 0;
                string kod_3 = ""; float par_3 = 0;
                string kod_4 = ""; float par_4 = 0;
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_3(Master_ID, Dev_Adres, ref tok, ref kod_1, ref par_1, ref kod_2, ref par_2, ref kod_3, ref par_3, ref kod_4, ref par_4);
                    }
                });
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_2(Master_ID, Dev_Adres, ref tok, ref proc);
                    }
                });
                Param_list.Clear();
                if (par_1 != -1) { Param_list.Add(new Param("#" + i.ToString(), par_1.ToString(), kod_1)); i++; }
                if (par_2 != -1) { Param_list.Add(new Param("#" + i.ToString(), par_2.ToString(), kod_2)); i++; }
                if (par_3 != -1) { Param_list.Add(new Param("#" + i.ToString(), par_3.ToString(), kod_3)); i++; }
                if (par_4 != -1) { Param_list.Add(new Param("#" + i.ToString(), par_4.ToString(), kod_4)); i++; }
                if (tok != -1)
                {
                    Param_list.Add(new Param("#" + i.ToString(), tok.ToString(), ".ma"));
                    i++;
                    Param_list.Add(new Param("#" + i.ToString(), proc.ToString(), ".%"));
                }
               
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
                timers = false;
            }
        }
    }
}
