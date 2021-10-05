using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Andriod_Hart.Models;
using System.Windows.Input;
using Class_HART;
using System.Threading;

namespace Andriod_Hart.ViewModels
{
    public class CommandsModel : BaseViewModel
    {
        public Command Fixed_Current_set { get; }
        public Command Fixed_Current_exit { get; }
        bool fixed_Current = true;
        public bool Fixed_Current{get => fixed_Current; set  { fixed_Current = value;OnPropertyChanged();}}
        string fixed_Current_str;
        public string Fixed_Current_str { get => fixed_Current_str; set { fixed_Current_str = value; OnPropertyChanged(); } }

        public Command Zero_ACP_set { get; }
        bool zero_ACP = true;
        public bool Zero_ACP { get => zero_ACP; set { zero_ACP = value; OnPropertyChanged(); } }
        string zero_ACP_str;
        public string Zero_ACP_str { get => zero_ACP_str; set { zero_ACP_str = value; OnPropertyChanged(); } }

        public Command Gain_ACP_set { get; }
        bool gain_ACP = true;
        public bool Gain_ACP { get => gain_ACP; set { gain_ACP = value; OnPropertyChanged(); } }
        string gain_ACP_str;
        public string Gain_ACP_str { get => gain_ACP_str; set { gain_ACP_str = value; OnPropertyChanged(); } }

        public Command Short_AD_set { get; }
        bool short_AD = true;
        public bool Short_AD { get => gain_ACP; set { gain_ACP = value; OnPropertyChanged(); } }
        string short_AD_str;
        public string Short_AD_str { get => short_AD_str; set { short_AD_str = value; OnPropertyChanged(); } }
        public Command U_range_value_set { get; }
        bool u_range_value = true;
        public bool U_range_value { get => u_range_value; set { u_range_value = value; OnPropertyChanged(); } }

        public Command L_range_value_set { get; }
        bool l_range_value = true;
        public bool L_range_value { get => l_range_value; set { l_range_value = value; OnPropertyChanged(); } }
        public Command Prim_value_set { get; }
        bool prim_value = true;
        public bool Prim_value { get => prim_value; set { prim_value = value; OnPropertyChanged(); } }

        public Command Preamb_Leng_set { get; }
        public Command Preamb_Leng_get { get; }
        bool preamb_Leng = true;
        public bool Preamb_Leng { get => preamb_Leng; set { preamb_Leng = value; OnPropertyChanged(); } }
        string preamb_Leng_str;
        public string Preamb_Leng_str { get => preamb_Leng_str; set { preamb_Leng_str = value; OnPropertyChanged(); } }

        public Command Serial_Num_get { get; }
        public Command Serial_Num_set { get; }
        bool serial_Num = true;
        public bool Serial_Num { get => serial_Num; set { serial_Num = value; OnPropertyChanged(); } }
        string serial_Num_str;
        public string Serial_Num_str { get => serial_Num_str; set { serial_Num_str = value; OnPropertyChanged(); } }

        public Command Reset_flag_set { get; }
        bool reset_flag = true;
        public bool Reset_flag { get => reset_flag; set { reset_flag = value; OnPropertyChanged(); } }
        public Command Reset_set { get; }
        bool reset = true;
        public bool Reset { get => reset; set { reset = value; OnPropertyChanged(); } }


        public Command Restor_EEPROM_set { get; }
        public Command Burn_EEPROM_set { get; }
        bool _EEPROM = true;
        public bool EEPROM { get => _EEPROM; set { _EEPROM = value; OnPropertyChanged(); } }

       
      
        public CommandsModel()
        { 
            Title = "Commands";
             Fixed_Current_set = new Command(_Fixed_Current_set, canExecute: () => Fixed_Current);
            Fixed_Current_exit = new Command(_Fixed_Current_exit, canExecute: () => Fixed_Current);
                  Zero_ACP_set = new Command(_Zero_ACP, canExecute: () => Zero_ACP);
                  Gain_ACP_set = new Command(_Gain_ACP, canExecute: () => Gain_ACP);
                  Short_AD_set = new Command(_Short_AD, canExecute: () => Short_AD);
             U_range_value_set = new Command(_U_range_value, canExecute: () => U_range_value);
             L_range_value_set = new Command(_L_range_value, canExecute: () => L_range_value);
                Prim_value_set = new Command(_Prim_value, canExecute: () => Prim_value);
               Preamb_Leng_set = new Command(_Preamb_Leng_set, canExecute: () => Preamb_Leng);
               Preamb_Leng_get = new Command(_Preamb_Leng_get, canExecute: () => Preamb_Leng);
                Serial_Num_set = new Command(_Serial_Num_set, canExecute: () => Serial_Num);
                Serial_Num_get = new Command(_Serial_Num_get, canExecute: () => Serial_Num);
                Reset_flag_set = new Command(_Reset_flag, canExecute: () => Reset_flag);
                     Reset_set = new Command(_Reset, canExecute: () => Reset);
               Burn_EEPROM_set = new Command(_Burn_EEPROM, canExecute: () => EEPROM);
             Restor_EEPROM_set = new Command(_Restor_EEPROM, canExecute: () => EEPROM);
        }
        async void _Fixed_Current_set()
        {
            Fixed_Current = false;
            Fixed_Current_set.ChangeCanExecute();
            Fixed_Current_exit.ChangeCanExecute();
            try
            {
                float temp = Convert.ToSingle(Fixed_Current_str.Replace(".", ","));
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_40(Master_ID, Dev_Adres, temp);
                    }
                });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Fixed Current Value :", ex.Message, "Cancel");
            }
        
            Fixed_Current = true;
            Fixed_Current_set.ChangeCanExecute();
            Fixed_Current_exit.ChangeCanExecute();
        }
        async void _Fixed_Current_exit()
        {
            Fixed_Current = false;
            Fixed_Current_set.ChangeCanExecute();
            Fixed_Current_exit.ChangeCanExecute();
            try { 
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    Hart_conection.Comand_40(Master_ID, Dev_Adres, 0);
                }
            });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Fixed Current Value :", ex.Message, "Cancel");
            }
            Fixed_Current = true;
            Fixed_Current_set.ChangeCanExecute();
            Fixed_Current_exit.ChangeCanExecute();
        }
        async void _Zero_ACP()
        {
            Zero_ACP = false;
            Zero_ACP_set.ChangeCanExecute();
            try
            {
                float temp = Convert.ToSingle(Zero_ACP_str);
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_45(Master_ID, Dev_Adres, temp);
                    }
                });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            Zero_ACP = true;
            Zero_ACP_set.ChangeCanExecute();
          
        }
         async void _Gain_ACP()
        {
            Gain_ACP = false;
            Gain_ACP_set.ChangeCanExecute();
            try
            {
                float temp = Convert.ToSingle(Gain_ACP_str);
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_46(Master_ID, Dev_Adres, temp);
                    }
                });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            Gain_ACP = true;
            Gain_ACP_set.ChangeCanExecute();
        }
        async void _Short_AD()
        {
            Short_AD = false;
            Short_AD_set.ChangeCanExecute();
            try
            {
                int temp = Convert.ToInt32(Short_AD_str);
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_6(Master_ID, Dev_Adres, temp);
                    }
                });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            Short_AD = true;
            Short_AD_set.ChangeCanExecute();
        }
        async void _U_range_value()
        {

            U_range_value = false;
            U_range_value_set.ChangeCanExecute();
            try
            {
             
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_36(Master_ID, Dev_Adres);
                    }
                });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            U_range_value = true;
            U_range_value_set.ChangeCanExecute();
        }
        async void _L_range_value()
        {
            L_range_value = false;
            L_range_value_set.ChangeCanExecute();
            try
            {

                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_37(Master_ID, Dev_Adres);
                    }
                });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            L_range_value = true;
            L_range_value_set.ChangeCanExecute();
        }
        
        async void _Prim_value()
        {
            Prim_value = false;
            Prim_value_set.ChangeCanExecute();
            try
            {

                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_43(Master_ID, Dev_Adres);
                    }
                });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            Prim_value = true;
            Prim_value_set.ChangeCanExecute();
        }
        async void _Preamb_Leng_set()
        {
            Preamb_Leng = false;
            Preamb_Leng_set.ChangeCanExecute();
            Preamb_Leng_get.ChangeCanExecute();
            try
            {
                int temp = Convert.ToInt32(Preamb_Leng_str);
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_59(Master_ID, Dev_Adres, temp);
                    }
                });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            Preamb_Leng = true;
            Preamb_Leng_set.ChangeCanExecute();
            Preamb_Leng_get.ChangeCanExecute();
        }
        async void _Preamb_Leng_get()
        {
            Preamb_Leng = false;
            Preamb_Leng_set.ChangeCanExecute();
            Preamb_Leng_get.ChangeCanExecute();
            try
            { 
                string[] temp = { };
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_0(Master_ID, Dev_Adres,ref temp);
                    }
                });
                Preamb_Leng_str = temp[2];
                
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            Preamb_Leng = true;
            Preamb_Leng_set.ChangeCanExecute();
            Preamb_Leng_get.ChangeCanExecute();
        }
        async void _Serial_Num_set()
        {
           
            Serial_Num = false;
            Serial_Num_set.ChangeCanExecute();
            Serial_Num_get.ChangeCanExecute();
            try
            {
                byte[] temp = _Convert.GetBytes(Serial_Num_str.Replace("-", ""));
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_49(Master_ID, Dev_Adres, temp);
                    }
                });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            Serial_Num = true;
            Serial_Num_set.ChangeCanExecute();
            Serial_Num_get.ChangeCanExecute();
        }
        async void _Serial_Num_get()
        {
            Serial_Num = false;
            Serial_Num_set.ChangeCanExecute();
            Serial_Num_get.ChangeCanExecute();
            try
            {
                string temp = "";
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_14(Master_ID, Dev_Adres,ref temp);
                    }
                });
                Serial_Num_str = temp;
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            Serial_Num = true;
            Serial_Num_set.ChangeCanExecute();
            Serial_Num_get.ChangeCanExecute();
        }
        async void _Reset_flag()
        {
            Reset_flag = false;
            Reset_flag_set.ChangeCanExecute();
            try
            {
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_38(Master_ID, Dev_Adres);
                    }
                });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            Reset_flag = true;
            Reset_flag_set.ChangeCanExecute();
        }
        async void _Reset()
        {
            Reset = false;
            Reset_set.ChangeCanExecute();
            try
            {
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_42(Master_ID, Dev_Adres);
                    }
                });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            Reset = true;
            Reset_set.ChangeCanExecute();
        }
        async void _Burn_EEPROM()
        {
            EEPROM = false;
            Burn_EEPROM_set.ChangeCanExecute();
            Restor_EEPROM_set.ChangeCanExecute();
            try
            {
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_39(Master_ID, Dev_Adres,0);
                    }
                });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            EEPROM = true;
            Burn_EEPROM_set.ChangeCanExecute();
            Restor_EEPROM_set.ChangeCanExecute();
        }
        
        async void _Restor_EEPROM()
        {
            EEPROM = false;
            Restor_EEPROM_set.ChangeCanExecute();
            Burn_EEPROM_set.ChangeCanExecute();
            try
            {
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_39(Master_ID, Dev_Adres, 1);
                    }
                });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            EEPROM = true;
            Restor_EEPROM_set.ChangeCanExecute();
            Burn_EEPROM_set.ChangeCanExecute();
        }
    }
}
