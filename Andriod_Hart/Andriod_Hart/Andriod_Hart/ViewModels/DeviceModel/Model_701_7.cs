using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using Class_HART;

namespace Andriod_Hart.ViewModels.DeviceModel
{
    public class Calib_tem
    {
        public int ID { get; set; }
        public float Temperature { get; set; }
        public string Kod_ACP_Temperature_Sensor { get; set; }
        public string Kod_ACP_Voltage_Sensor { get; set; }
        public string Kod_ACP_Сurrent_Sensor { get; set; }
        public ObservableCollection<Calib_dav> _Davs { get; set; }
        public Calib_tem(int i,float T,string K1,string K2,string K3)
        {
            ID = i;
            Temperature = T;
            Kod_ACP_Temperature_Sensor = K1;
            Kod_ACP_Voltage_Sensor = K2;
            Kod_ACP_Сurrent_Sensor = K3;
            _Davs = new ObservableCollection<Calib_dav>();
        }
        public Calib_tem() { }
    }
    public class Calib_dav
    {
        public int ID { get; set; }
        public float Pressure { get; set; }
        public string Kod_ACP_1 { get; set; }
        public string Kod_ACP_2 { get; set; }
        public string Kod_ACP_3 { get; set; }
        public string Kod_ACP_4 { get; set; }
        public Calib_dav(int i,float P,string K1, string K2, string K3, string K4)
        {
            ID = i;
            Pressure = P;
            Kod_ACP_1 = K1;
            Kod_ACP_2 = K2;
            Kod_ACP_3 = K3;
            Kod_ACP_4 = K4;
        }
        public Calib_dav() { }
    }
    public class Model_701_7 : BaseViewModel
    {
       
        public ObservableCollection<Calib_tem> Calib { get; set; } = new ObservableCollection<Calib_tem> { };
        public Command Calibration_Table_set { get; }
        public Command Calibration_Table_get { get; }
        bool calibration_Table = true;
        public bool Calibration_Table { get => calibration_Table; set { calibration_Table = value; OnPropertyChanged();} }
        public bool Calibration_Table_invers { get => !(Calibration_Table); set { Calibration_Table = !(value); OnPropertyChanged(); } }
    
        string proges = "Calibration Table";
        public string Progres { get => proges; set { proges = value; OnPropertyChanged(); } }

        public Command ADC_Cods_get { get; }
        bool adc_cods = true;
        public bool ADC_Cods { get => adc_cods; set { adc_cods = value; OnPropertyChanged(); } }

        string p_setsor = "";
        public string P_Sensor { get => p_setsor; set { p_setsor = value; OnPropertyChanged(); } }
        string t_setsor = "";
        public string T_Sensor { get => t_setsor; set { t_setsor = value; OnPropertyChanged(); } }
        string v_setsor = "";
        public string V_Sensor { get => v_setsor; set { v_setsor = value; OnPropertyChanged(); } }
        string c_setsor = "";
        public string C_Sensor { get => c_setsor; set { c_setsor = value; OnPropertyChanged(); } }
        string oc_setsor = "";
        public string Oc_Sensor { get => oc_setsor; set { oc_setsor = value; OnPropertyChanged(); } }
        public Command Dev_Param_get { get; }
        public Command Dev_Param_set { get; }
        bool dev_param = true;
        public bool Dev_Param { get => dev_param; set { dev_param = value; OnPropertyChanged(); } }
        int contrast = 10;
        public int Contrast { get => contrast; set { contrast = value; OnPropertyChanged(); } }
        public List<string> MTM_Temperature_sensor { get; set; } = new List<string> { };
        public List<string> MTM_Presure_sensor { get; set; } = new List<string> { };
        public List<string> MTM_tupe_ys { get; set; } = new List<string> { };
        public List<string> MTM_priv_ys { get; set; } = new List<string> { };
        int s_sensor_tupe_t = 0;
        public int S_sensor_tupe_t { get => s_sensor_tupe_t; set { s_sensor_tupe_t = value; OnPropertyChanged(); } }
        int s_sensor_tupe_p = 0;
        public int S_sensor_tupe_p { get => s_sensor_tupe_p; set { s_sensor_tupe_p = value; OnPropertyChanged(); } }
        int s_l_1 = 0;
        public int S_l_1 { get => s_l_1; set { s_l_1 = value; OnPropertyChanged(); } }
        int s_p_1 = 0;
        public int S_p_1 { get => s_p_1; set { s_p_1 = value; OnPropertyChanged(); } }
        int s_l_2 = 0;
        public int S_l_2 { get => s_l_2; set { s_l_2 = value; OnPropertyChanged(); } }
        int s_p_2 = 0;
        public int S_p_2 { get => s_p_2; set { s_p_2 = value; OnPropertyChanged(); } }
        string s_lo_res_1 = "";
        public string S_lo_res_1 { get => s_lo_res_1; set { s_lo_res_1 = value; OnPropertyChanged(); } }
        string s_up_res_1 = "";
        public string S_up_res_1 { get => s_up_res_1; set { s_up_res_1 = value; OnPropertyChanged(); } }
        string s_Hyst_1 = "";
        public string S_Hyst_1 { get => s_Hyst_1; set { s_Hyst_1 = value; OnPropertyChanged(); } }
        string s_lo_res_2 = "";
        public string S_lo_res_2 { get => s_lo_res_2; set { s_lo_res_2 = value; OnPropertyChanged(); } }
        string s_up_res_2 = "";
        public string S_up_res_2 { get => s_up_res_2; set { s_up_res_2 = value; OnPropertyChanged(); } }
        string s_Hyst_2 = "";
        public string S_Hyst_2 { get => s_Hyst_2; set { s_Hyst_2 = value; OnPropertyChanged(); } }
        public Command Calibration_Current_set { get; }
        public Command Calibration_Current_get { get; }
        bool calibration_Current = true;
        public bool Calibration_Current { get => calibration_Current; set { calibration_Current = value; OnPropertyChanged(); } }
        string min_i = "";
        public string Min_i { get => min_i; set { min_i = value; OnPropertyChanged(); } }
        string min_DAC = "";
        public string Min_DAC { get => min_DAC; set { min_DAC = value; OnPropertyChanged(); } }
        string min_ADC = "";
        public string Min_ADC { get => min_ADC; set { min_ADC = value; OnPropertyChanged(); } }
        string max_i = "";
        public string Max_i { get => max_i; set { max_i = value; OnPropertyChanged(); } }
        string max_DAC = "";
        public string Max_DAC { get => max_DAC; set { max_DAC = value; OnPropertyChanged(); } }
        string max_ADC = "";
        public string Max_ADC { get => max_ADC; set { max_ADC = value; OnPropertyChanged(); } }
        public Model_701_7()
        {
            MTM_Temperature_sensor.AddRange(Conect.MTM701_Cods_TempSignal);
            MTM_Presure_sensor.AddRange(Conect.MTM701_Cods_Pressure);
            MTM_tupe_ys.AddRange(Conect.MTM701_Cods_Type);
            MTM_priv_ys.AddRange(Conect.MTM701_Cods_Priv);
            Calibration_Table_get = new Command(_Calibration_Table, canExecute: () => Calibration_Table);
            Calibration_Table_set = new Command(_Calibration_Table_write, canExecute: () => Calibration_Table);
            ADC_Cods_get = new Command(_ADC_Cods, canExecute: () => ADC_Cods);
            Dev_Param_get = new Command(_Dev_Param_get, canExecute: () => Dev_Param);
            Dev_Param_set = new Command(_Dev_Param_set, canExecute: () => Dev_Param);
            Calibration_Current_set = new Command(_Calibration_Current_set, canExecute: () => Calibration_Current);
            Calibration_Current_get = new Command(_Calibration_Current_get, canExecute: () => Calibration_Current);
            for (int i = 0; i <= 1; i++)
            {
                ObservableCollection<Calib_dav> temp = new ObservableCollection<Calib_dav>();
                for (int j = 0; j <= 9; j++)
                {
                    Calib_dav temp2 = new Calib_dav();
                    temp2.ID = j;
                    temp2.Pressure = 0;
                    temp2.Kod_ACP_1 = "0x0000";
                    temp2.Kod_ACP_2 = "0x0000";
                    temp2.Kod_ACP_3 = "0x0000";
                    temp2.Kod_ACP_4 = "0x0000";
                    temp.Add(temp2);
                }
                Calib_tem temp3 = new Calib_tem();
                temp3.ID = i;
                temp3._Davs = temp;
                temp3.Temperature = 0;
                temp3.Kod_ACP_Temperature_Sensor = "0x0000";
                temp3.Kod_ACP_Voltage_Sensor = "0x0000";
                temp3.Kod_ACP_Сurrent_Sensor = "0x0000";
                Calib.Add(temp3);
            }
            Title = "MTM701.7";
           
        }
        async void _Calibration_Table()
        {
            Calibration_Table = false;
            Calibration_Table_get.ChangeCanExecute();
            Calibration_Table_set.ChangeCanExecute();
            Calib.Clear();
            try
            {
                int Erors = 0;
                for (int i = 0; i <= 4; i++)
                {
                    if (Erors < 30)
                    {
                        float temp = 0;
                        string kod1 = "0";
                        string kod2 = "0";
                        string kod3 = "0";
                        Progres = "Read Table Point : " + i.ToString() + " / -";
                        await Task.Run(() =>
                        {
                            lock (balanceLock)
                            {
                                int p = 0;
                                while ((p <= 2))
                                {
                                    Hart_conection.MTM701_Comand_134(Master_ID, Dev_Adres, 0, i,
                                    ref temp,
                                    ref kod1,
                                    ref kod2,
                                    ref kod3);
                                    if (kod1 != "0") return;
                                    p++;
                                    Erors++;
                                }

                            }
                        });
                        Calib.Add(new Calib_tem(i, temp, kod1, kod2, kod3));


                        for (int j = 0; j <= 9; j++)
                        {
                            if (Erors < 30)
                            {
                                float pres = 0;
                                string kod_1 = "0", kod_2 = "0", kod_3 = "0", kod_4 = "0";
                                Progres = "Read Table Point : " + i.ToString() + " / " + j.ToString();
                                await Task.Run(() =>
                                {
                                    lock (balanceLock)
                                    {
                                        int p = 0;
                                        while ((p <= 2))
                                        {
                                            Hart_conection.MTM701_Comand_135(Master_ID, Dev_Adres, 0, i, j,
                                            ref pres,
                                            ref kod_1,
                                            ref kod_2,
                                            ref kod_3,
                                            ref kod_4);
                                            if (kod_1 != "0") return;
                                            p++;
                                            Erors++;
                                        }
                                    }
                                });
                                Calib[i]._Davs.Add(new Calib_dav(j, pres, kod_1, kod_2, kod_3, kod_4));
                            }
                        }
                    }
                }
                if (Erors >= 30) await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Table Eror :", "The number of query errors exceeded the allowed value", "Cancel");
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            Calibration_Table = true;
            Calibration_Table_get.ChangeCanExecute();
            Calibration_Table_set.ChangeCanExecute();
            
            Progres = "Calibration Table";
        }
        async void _Calibration_Table_write()
        {
            Calibration_Table = false;
            Calibration_Table_get.ChangeCanExecute();
            Calibration_Table_set.ChangeCanExecute();
            for (int i = 0; i <= 4; i++)
            {
                float temp = Calib[i].Temperature;
                string kod1 = Calib[i].Kod_ACP_Temperature_Sensor;
                string kod2 = Calib[i].Kod_ACP_Voltage_Sensor;
                string kod3 = Calib[i].Kod_ACP_Сurrent_Sensor;
                Progres = "Write Table Point : " + i.ToString() + " / -";
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.MTM701_Comand_134(Master_ID, Dev_Adres, 1, i,
                        ref temp,
                        ref kod1,
                        ref kod2,
                        ref kod3);
                    }
                });
                for (int j = 0; j <= 9; j++)
                {
                    float pres = Calib[i]._Davs[j].Pressure;
                    string kod_1 = Calib[i]._Davs[j].Kod_ACP_1,
                           kod_2 = Calib[i]._Davs[j].Kod_ACP_2,
                           kod_3 = Calib[i]._Davs[j].Kod_ACP_3,
                           kod_4 = Calib[i]._Davs[j].Kod_ACP_4;
                    Progres = "Write Table Point : " + i.ToString() + " / " + j.ToString();
                    await Task.Run(() =>
                    {
                        lock (balanceLock)
                        {
                            Hart_conection.MTM701_Comand_135(Master_ID, Dev_Adres, 1, i, j,
                            ref pres,
                            ref kod_1,
                            ref kod_2,
                            ref kod_3,
                            ref kod_4);
                        }
                    });
                   
                }
            }
            Calibration_Table = true;
            Calibration_Table_get.ChangeCanExecute();
            Calibration_Table_set.ChangeCanExecute();
            Progres = "Calibration Table";
        }
        async void _ADC_Cods()
        {
            ADC_Cods = false;
            ADC_Cods_get.ChangeCanExecute();
            try
            {
                string[] s_res = { };
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.MTM701_Comand_130(Master_ID, Dev_Adres,
                     ref s_res);
                    }
                });
                P_Sensor = s_res[0];
                T_Sensor = s_res[1];
                V_Sensor = s_res[2];
                C_Sensor = s_res[3];
                Oc_Sensor = s_res[4];
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            ADC_Cods = true;
            ADC_Cods_get.ChangeCanExecute();
        }
        async void _Dev_Param_get()
        {
            Dev_Param = false;
            Dev_Param_get.ChangeCanExecute();
            Dev_Param_set.ChangeCanExecute();
            try
            {
                int display = 0;
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.MTM701_Comand_131(Master_ID, Dev_Adres, 0,
                            ref display);
                    }
                });

                Contrast = display;
                int cod_dt = 0;
                int cod_iz = 0;
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.MTM701_Comand_133(Master_ID, Dev_Adres, 0,
                            ref cod_dt, ref cod_iz);
                    }
                });

                S_sensor_tupe_t = cod_dt;
                S_sensor_tupe_p = cod_iz;
                int tupe = 0;
                int tupe2 = 0;
                float lou = 0;
                float upp = 0;
                float dum = 0;
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.MTM701_Comand_132(Master_ID, Dev_Adres, 0, 0,
                            ref tupe, ref tupe2, ref lou, ref upp, ref dum);
                    }
                });

                S_l_1 = tupe;
                S_p_1 = tupe2;
                S_lo_res_1 = lou.ToString();
                S_up_res_1 = upp.ToString();
                S_Hyst_1 = dum.ToString();
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.MTM701_Comand_132(Master_ID, Dev_Adres, 0, 1,
                            ref tupe, ref tupe2, ref lou, ref upp, ref dum);
                    }
                });

                S_l_2 = tupe;
                S_p_2 = tupe2;
                S_lo_res_2 = lou.ToString();
                S_up_res_2 = upp.ToString();
                S_Hyst_2 = dum.ToString();
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            Dev_Param = true;
            Dev_Param_get.ChangeCanExecute();
            Dev_Param_set.ChangeCanExecute();
        }
        async void _Dev_Param_set()
        {
            Dev_Param = false;
            Dev_Param_get.ChangeCanExecute();
            Dev_Param_set.ChangeCanExecute();
            try
            {
                int display = (int)Contrast;
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.MTM701_Comand_131(Master_ID, Dev_Adres, 1,
                            ref display);
                    }
                });

                int cod_dt = S_sensor_tupe_t;
                int cod_iz = S_sensor_tupe_p;
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.MTM701_Comand_133(Master_ID, Dev_Adres, 1,
                            ref cod_dt, ref cod_iz);
                    }
                });

                int tupe = S_l_1;
                int tupe2 = S_p_1;
                float lou = Convert.ToSingle(S_lo_res_1);
                float upp = Convert.ToSingle(S_up_res_1);
                float dum = Convert.ToSingle(S_Hyst_1);
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.MTM701_Comand_132(Master_ID, Dev_Adres, 1, 0,
                            ref tupe, ref tupe2, ref lou, ref upp, ref dum);
                    }
                });

                tupe = S_l_2;
                tupe2 = S_p_2;
                lou = Convert.ToSingle(S_lo_res_2);
                upp = Convert.ToSingle(S_up_res_2);
                dum = Convert.ToSingle(S_Hyst_2);
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.MTM701_Comand_132(Master_ID, Dev_Adres, 1, 1,
                            ref tupe, ref tupe2, ref lou, ref upp, ref dum);
                    }
                });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            Dev_Param = true;
            Dev_Param_get.ChangeCanExecute();
            Dev_Param_set.ChangeCanExecute();
        }
        async void _Calibration_Current_get()
        {
            Calibration_Current = false;
            Calibration_Current_get.ChangeCanExecute();
            Calibration_Current_set.ChangeCanExecute();
            try
            {
                float min = 0;
                string min_a = "";
                string min_c = "";
                float max = 0;
                string max_a = "";
                string max_c = "";
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.MTM701_Comand_136(Master_ID, Dev_Adres, 0,
                            ref min,
                            ref max,
                            ref min_c,
                            ref min_a,
                            ref max_c,
                            ref max_a);
                    }
                });
                Max_i = max.ToString();
                Min_i = min.ToString();
                Max_ADC = max_a;
                Max_DAC = max_c;
                Min_ADC = min_a;
                Min_DAC = min_c;

            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            Calibration_Current = true;
            Calibration_Current_get.ChangeCanExecute();
            Calibration_Current_set.ChangeCanExecute();
        }
        async void _Calibration_Current_set()
        {
            Calibration_Current = false;
            Calibration_Current_get.ChangeCanExecute();
            Calibration_Current_set.ChangeCanExecute();
            try
            {
                float min = Convert.ToSingle(Min_i);
                string min_a = Min_ADC;
                string min_c = Min_DAC;
                float max = Convert.ToSingle(Max_i);
                string max_a = Max_ADC;
                string max_c = Max_DAC;
                await Task.Run(() =>
                {
                    lock (balanceLock)
                    {
                        Hart_conection.MTM701_Comand_136(Master_ID, Dev_Adres, 1,
                            ref min,
                            ref max,
                            ref min_c,
                            ref min_a,
                            ref max_c,
                            ref max_a);
                    }
                });
            }
            catch (Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Eror :", ex.Message, "Cancel");
            }
            Calibration_Current = true;
            Calibration_Current_get.ChangeCanExecute();
            Calibration_Current_set.ChangeCanExecute();
        }
    }
}
