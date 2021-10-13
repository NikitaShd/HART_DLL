using Andriod_Hart.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Class_HART;
using System.Threading;

namespace Andriod_Hart.ViewModels
{
    public class BaseInfoModel : BaseViewModel
    {
      
        string[] Values = new string[10] {"NAN", "NAN", "NAN", "NAN", "NAN", "NAN", "NAN", "NAN", "NAN", "NAN"};
        public string L_Manufacturer_Code  { get { return Values[0]; }  set { SetProperty(ref Values[0], value); } }
        public string L_Device_Type_Code   { get { return Values[1]; } set { SetProperty(ref Values[1], value); } }
        public string L_Preambul_leng      { get { return Values[2]; } set { SetProperty(ref Values[2], value); } }
        public string L_Universal_commands { get { return Values[3]; } set { SetProperty(ref Values[3], value); } }
        public string L_Specific_commands  { get { return Values[4]; } set { SetProperty(ref Values[4], value); } }
        public string L_Software_version   { get { return Values[5]; } set { SetProperty(ref Values[5], value); } }
        public string L_Hardware_version   { get { return Values[6]; } set { SetProperty(ref Values[6], value); } }
        public string L_Device_function    { get { return Values[8]; } set { SetProperty(ref Values[8], value); } }

        public List<string> S_Alarm_cods { get; set; } = new List<string> { };
        string selected_S_Alarm_cods;
        public string Selected_S_Alarm_cods { get { return selected_S_Alarm_cods; } set { SetProperty(ref selected_S_Alarm_cods, value); } }
        public List<string> S_Unit_cods { get; set; } = new List<string> { };
        string selected_S_Unit_cods;
        public string Selected_S_Unit_cods { get { return selected_S_Unit_cods; } set { SetProperty(ref selected_S_Unit_cods, value); } }
        public List<string> S_Protect_cods { get; set; } = new List<string> { };
        string selected_Protect_cods;
        public string Selected_Protect_cods { get { return selected_Protect_cods; } set { SetProperty(ref selected_Protect_cods, value); } }
        public List<string> S_Transfer_cods { get; set; } = new List<string> { };
        string selected_S_Transfer_cods;
        public string Selected_S_Transfer_cods { get { return selected_S_Transfer_cods; } set { SetProperty(ref selected_S_Transfer_cods, value); } }

        string t_dev_mes;
        public string T_dev_mes { get { return t_dev_mes; } set { SetProperty(ref t_dev_mes, value); } }

        string t_s_teg;
        public string T_s_teg { get { return t_s_teg; } set { SetProperty(ref t_s_teg, value); } }

        string t_discriptor;
        public string T_discriptor { get { return t_discriptor; } set { SetProperty(ref t_discriptor, value); } }

        string t_L_teg;
        public string T_L_teg { get { return t_L_teg; } set { SetProperty(ref t_L_teg, value); } }

        private DateTime? t_data;

        public DateTime? T_data
        {
            get => t_data;

            set
            {
                t_data = value;
                OnPropertyChanged();
            }
        }

        string t_U_renge;
        public string T_U_renge { get { return t_U_renge; } set { SetProperty(ref t_U_renge, value); } }

        string t_L_renge;
        public string T_L_renge { get { return t_L_renge; } set { SetProperty(ref t_L_renge, value); } }

        string t_Demp;
        public string T_Demp { get { return t_Demp; } set { SetProperty(ref t_Demp, value); } }

        string t_Manufacturer;
        public string T_Manufacturer { get { return t_Manufacturer; } set { SetProperty(ref t_Manufacturer, value); } }

        bool B_staite_0 = true;
        bool b_info_staite = true;
        public bool B_info_staite
        {
            get => b_info_staite;
            set
            {
                b_info_staite = value;
                OnPropertyChanged();
            }
        }
        bool b_info_extend = true;
        public bool B_info_extend
        {
            get => b_info_extend;
            set
            {
                b_info_extend = value;
                OnPropertyChanged();
            }
        }
        public Command Command_0 { get; }
        public Command Command_info_read { get; }
        public Command Command_info_write { get; }
        public Command Command_extendet_read { get; }
        public Command Command_extendet_write { get; }
        public BaseInfoModel()
        {
            this.T_data = new DateTime();
            Command_0 = new Command(_Comand_0, canExecute: () => B_staite_0);
            Command_info_read = new Command(_Read_Info, canExecute: () => B_info_staite);
            Command_info_write = new Command(_Write_Info, canExecute: () => B_info_staite);
            Command_extendet_read = new Command(_Read_Extented, canExecute: () => B_info_extend);
            Command_extendet_write = new Command(_Write_Extented, canExecute: () => B_info_extend);
            Title = "Base Info";
            S_Alarm_cods.AddRange(_Tables.Alarm_Cods_arr());
            S_Protect_cods.AddRange(_Tables.Protect_Cods_arr());
            S_Transfer_cods.AddRange(_Tables.Transfer_Cods_arr());
            S_Unit_cods.AddRange(_Tables.unit_arr());
        }

        private async void _Comand_0()
        {

            B_staite_0 = false;
            string[] temp = { };
            Command_0.ChangeCanExecute();
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.Comand_0(Master_ID, Dev_Adres, ref temp);

                    }
                });
                L_Manufacturer_Code = temp[0];
                L_Device_Type_Code = temp[1];
                L_Preambul_leng = temp[2];
                L_Universal_commands = temp[3];
                L_Specific_commands = temp[4];
                L_Software_version = temp[5];
                L_Hardware_version = temp[6];
                L_Device_function = temp[8];
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Fixed Current Value :", ex.Message, "Cancel");
            }
            B_staite_0 = true;
            Command_0.ChangeCanExecute();
        }
        private async void _Read_Info()
        {
            B_info_staite = false;
            Command_info_read.ChangeCanExecute();
            Command_info_write.ChangeCanExecute();
            try { 
            string mes = "";
            string[] teg_discriptor_data = { };
            string long_teg = "";
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    mes = Hart_conection.Comand_12(Master_ID, Dev_Adres);
                }
            });
            T_dev_mes = mes;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    teg_discriptor_data = Hart_conection.Comand_13(Master_ID, Dev_Adres);
                }
            });
            T_s_teg = teg_discriptor_data[0];
            T_discriptor = teg_discriptor_data[1];
            T_data = DateTime.Parse(teg_discriptor_data[2]);
            
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    Hart_conection.Comand_20(Master_ID, Dev_Adres, ref long_teg);
                }
            });
            T_L_teg = long_teg;
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Fixed Current Value :", ex.Message, "Cancel");
            }
            B_info_staite = true;
            Command_info_read.ChangeCanExecute();
            Command_info_write.ChangeCanExecute();
        }
        private async void _Write_Info()
        {
            B_info_staite = false;
            Command_info_read.ChangeCanExecute();
            Command_info_write.ChangeCanExecute();
            try{ 
            string mes = T_dev_mes;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                   if(!Hart_conection.Comand_17(Master_ID, Dev_Adres, mes.ToUpper()))
                   Task.Delay(1000);
                }
            });
            string s_teg = T_s_teg;
            string discriptor = T_discriptor;
            string data = T_data.Value.ToShortDateString();
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    if (!Hart_conection.Comand_18(Master_ID, Dev_Adres, s_teg.ToUpper(), discriptor.ToUpper(), data))
                    Task.Delay(1000);
                }
            });
            string l_teg = T_L_teg;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    //if(!Hart_conection.Comand_22(Master_ID, Dev_Adres, l_teg))
                    // Task.Delay(1000);
                }
            });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Fixed Current Value :", ex.Message, "Cancel");
            }
            B_info_staite = true;
            Command_info_read.ChangeCanExecute();
            Command_info_write.ChangeCanExecute();
        }
        private async void _Read_Extented()
        {
            B_info_extend = false;
            Command_extendet_read.ChangeCanExecute();
            Command_extendet_write.ChangeCanExecute();
            try { 
            int Alarm = 0;
            int Transfer = 0;
            int Unit = 0;
            float U_renge = 0;
            float L_renge = 0;
            float Damfing = 0;
            int Protect = 0;
            int Manufacturer = 0;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    Hart_conection.Comand_15(Master_ID, Dev_Adres, ref Alarm, ref Transfer, ref Unit, ref U_renge, ref L_renge, ref Damfing, ref Protect, ref Manufacturer);
                }
            });
            /*
            ComboBox_Signal.SelectedItem = _Tables.Alarm_Cods(Alarm);
            ComboBox_Function.SelectedItem = _Tables.Transfer_Cods(Transfer);
            ComboBox_Protect.SelectedItem = _Tables.Protect_Cods(Protect);
            ComboBox_Units.SelectedItem = _Tables.Encod_unit(Unit);*/
            Selected_S_Alarm_cods = _Tables.Alarm_Cods(Alarm);
            Selected_Protect_cods = _Tables.Protect_Cods(Protect);
            Selected_S_Transfer_cods= _Tables.Transfer_Cods(Transfer);
            Selected_S_Unit_cods = _Tables.Encod_unit(Unit);
            T_U_renge = U_renge.ToString();
            T_L_renge = L_renge.ToString();
            T_Demp = Damfing.ToString();
            T_Manufacturer = Manufacturer.ToString();
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Fixed Current Value :", ex.Message, "Cancel");
            }
            B_info_extend = true;
            Command_extendet_read.ChangeCanExecute();
            Command_extendet_write.ChangeCanExecute();
        }
        private async void _Write_Extented()
        {
            B_info_extend = false;
            Command_extendet_read.ChangeCanExecute();
            Command_extendet_write.ChangeCanExecute();
            try { 
            int Alarm = _Tables.Alarm_Cods(Selected_S_Alarm_cods);
            int Transfer = _Tables.Transfer_Cods(Selected_S_Transfer_cods);
            int Unit = _Tables.Encod_unit(Selected_S_Unit_cods);
            float U_renge = Convert.ToSingle(T_U_renge);
            float L_renge = Convert.ToSingle(T_L_renge);
            float Damfing = Convert.ToSingle(T_Demp);
            int Protect = _Tables.Protect_Cods(Selected_Protect_cods);
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    Hart_conection.Comand_35(Master_ID, Dev_Adres, Unit, U_renge, L_renge);
                }
            });
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    Hart_conection.Comand_34(Master_ID, Dev_Adres, Damfing);
                }
            });
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    Hart_conection.Comand_47(Master_ID, Dev_Adres, Transfer);
                }
            });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Fixed Current Value :", ex.Message, "Cancel");
            }
            B_info_extend = true;
            Command_extendet_read.ChangeCanExecute();
            Command_extendet_write.ChangeCanExecute();
        }
    }
}
