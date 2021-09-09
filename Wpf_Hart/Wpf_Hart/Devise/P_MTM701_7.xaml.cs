using Class_HART;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf_Hart.Devise
{

    /// <summary>
    /// Логика взаимодействия для P_MTM701_7.xaml
    /// </summary>


    public partial class P_MTM701_7 : Page
    {
       
        public class Calib_tem
        {
            public int ID { get; set; }
            public float Temperature { get; set; }
            public string Kod_ACP_Temperature_Sensor { get; set; }
            public string Kod_ACP_Voltage_Sensor { get; set; }
            public string Kod_ACP_Сurrent_Sensor { get; set; }
            public ObservableCollection<Calib_dav> _Davs { get; set; }
          
        }
       public class Calib_dav
        {
            public int ID { get; set; }
            public float Pressure { get; set; }
            public string Kod_ACP_1 { get; set; }
            public string Kod_ACP_2 { get; set; }
            public string Kod_ACP_3 { get; set; }
            public string Kod_ACP_4 { get; set; }
        }
       
        public ObservableCollection<Calib_tem> Calib { get; set; } = new ObservableCollection<Calib_tem> {};

         MainWindow window;
        public List<string> MTM_Temperature_sensor { get; set; } = new List<string> { };
        public List<string> MTM_Presure_sensor { get; set; } = new List<string> {};

        public List<string> MTM_tupe_ys { get; set; } = new List<string> {};
        public List<string> MTM_priv_ys { get; set; } = new List<string> {};

        public P_MTM701_7()
        {
            
            MTM_Temperature_sensor.AddRange(Conect.MTM701_Cods_TempSignal);
            MTM_Presure_sensor.AddRange(Conect.MTM701_Cods_Pressure);
            MTM_tupe_ys.AddRange(Conect.MTM701_Cods_Type);
            MTM_priv_ys.AddRange(Conect.MTM701_Cods_Priv);
            for (int i = 0; i <= 4; i++)
            {
                ObservableCollection<Calib_dav> temp = new ObservableCollection<Calib_dav>();
                for (int j = 0; j <= 9; j++)
                {
                    Calib_dav temp2 = new Calib_dav();
                    temp2.ID = j;
                    temp2.Pressure = 0;
                    temp2.Kod_ACP_1 = "0x0000 ";
                    temp2.Kod_ACP_2 = "0x0000 ";
                    temp2.Kod_ACP_3 = "0x0000 ";
                    temp2.Kod_ACP_4 = "0x0000 ";
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

            this.DataContext = this;
            InitializeComponent();

           
            window = (MainWindow)Application.Current.MainWindow;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void hexValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9A-Fa-f-x]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        void Saive()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            saveFileDialog.FileName = "MTM701_7(" + DateTime.Now.ToString() + ")";
            if (saveFileDialog.ShowDialog() == true)
            {
                List<string> temp = new List<string>();
                
                for (int i = 0; i <= 4; i++)
                {
                    temp.Add(Calib[i].Temperature.ToString() + ":" +
                             Calib[i].Kod_ACP_Temperature_Sensor.ToString() + ":" +
                             Calib[i].Kod_ACP_Voltage_Sensor.ToString() + ":" +
                             Calib[i].Kod_ACP_Сurrent_Sensor.ToString()
                        );

                    for (int j = 0; j <= 9; j++)
                    {
                        temp.Add(Calib[i]._Davs[j].Pressure.ToString() + ":" +
                            Calib[i]._Davs[j].Kod_ACP_1.ToString() + ":" +
                            Calib[i]._Davs[j].Kod_ACP_2.ToString() + ":" +
                            Calib[i]._Davs[j].Kod_ACP_3.ToString() + ":" +
                            Calib[i]._Davs[j].Kod_ACP_4.ToString()
                       );
                    }
                }
                temp.Add("[Param]");
                temp.Add(slider.Value.ToString());
                temp.Add(C_Temp_sensor.SelectedIndex.ToString());
                temp.Add(C_Pres_sensor.SelectedIndex.ToString());
                temp.Add(C_ys1_log.SelectedIndex.ToString());
                temp.Add(C_ys1_tec.SelectedIndex.ToString());
                temp.Add(T_ys1_Lower.Text);
                temp.Add(T_ys1_Upper.Text);
                temp.Add(T_ys1_Hyster.Text);
                temp.Add(C_ys2_log.SelectedIndex.ToString());
                temp.Add(C_ys2_tec.SelectedIndex.ToString());
                temp.Add(T_ys2_Lower.Text);
                temp.Add(T_ys2_Upper.Text);
                temp.Add(T_ys2_Hyster.Text);
                temp.Add("[ACP]");
                temp.Add(T_ACP_1.Text);
                temp.Add(T_ACP_2.Text);
                temp.Add(T_ACP_3.Text);
                temp.Add(T_ACP_4.Text);
                temp.Add(T_ACP_5.Text);
                temp.Add("[Current]");
                temp.Add(T_max.Text);
                temp.Add(T_max_D.Text);
                temp.Add(T_max_A.Text);
                temp.Add(T_min.Text);
                temp.Add(T_min_D.Text);
                temp.Add(T_min_A.Text);

                File.WriteAllLines(saveFileDialog.FileName, temp.ToArray());
            }
        }
        void Load()
        {
            List<string> temp = new List<string>();
            string[] subs;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                int x = -1;
                int y = 0;
                temp = File.ReadAllLines(openFileDialog.FileName).ToList();
                for (int i = 0; i <= 54; i++)
                {
                    
                    if ((i % 11) == 0)
                    {
                        x++;
                        y = 0;
                        subs = temp[i].Split(':');
                        Calib[x].Temperature = Convert.ToSingle(subs[0]);
                        Calib[x].Kod_ACP_Temperature_Sensor = subs[1];
                        Calib[x].Kod_ACP_Voltage_Sensor = subs[2];
                        Calib[x].Kod_ACP_Сurrent_Sensor = subs[3];
                       
                    }
                    else
                    {
                        subs = temp[i].Split(':');
                        Calib[x]._Davs[y].Pressure = Convert.ToSingle(subs[0]);
                        Calib[x]._Davs[y].Kod_ACP_1 = subs[1];
                        Calib[x]._Davs[y].Kod_ACP_2 = subs[2];
                        Calib[x]._Davs[y].Kod_ACP_3 = subs[3];
                        Calib[x]._Davs[y].Kod_ACP_4 = subs[4];
                        y++;
                    }

                }
                slider.Value = Convert.ToDouble(temp[56]);
                C_Temp_sensor.SelectedIndex = Convert.ToInt32(temp[57]);
                C_Pres_sensor.SelectedIndex = Convert.ToInt32(temp[58]);
                C_ys1_log.SelectedIndex = Convert.ToInt32(temp[59]);
                C_ys1_tec.SelectedIndex = Convert.ToInt32(temp[60]);
                T_ys1_Lower.Text = temp[61];
                T_ys1_Upper.Text = temp[62];
                T_ys1_Hyster.Text = temp[63];
                C_ys2_log.SelectedIndex = Convert.ToInt32(temp[64]);
                C_ys2_tec.SelectedIndex = Convert.ToInt32(temp[65]);
                T_ys2_Lower.Text = temp[66];
                T_ys2_Upper.Text = temp[67];
                T_ys2_Hyster.Text = temp[68];

                 T_ACP_1.Text = temp[70];
                 T_ACP_2.Text = temp[71];
                 T_ACP_3.Text = temp[72];
                 T_ACP_4.Text = temp[73];
                 T_ACP_5.Text = temp[74];

                 T_max.Text = temp[76];
                 T_max_D.Text = temp[77];
                 T_max_A.Text = temp[78];
                 T_min.Text = temp[79];
                 T_min_D.Text = temp[80];
                 T_min_A.Text = temp[81];
            }
            test1.Items.Refresh();
        }
        private void exit_Click(object sender, RoutedEventArgs e)
        {
         window.Frame_close();
            
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            T_contrast.Text = Convert.ToInt32(slider.Value).ToString();
        }

        private async void B_dev_Contrast_read_Click(object sender, RoutedEventArgs e)
        {
            P_Ystavki.IsEnabled = false;
            Skan_progres.Maximum += 4;
            int display = 0;
          
            await Task.Run(() =>
            {
                lock (window.balanceLock)
                {
                    window.HART_conection.MTM701_Comand_131(Properties.Settings.Default.Master, window.Devise_long_adres, 0,
                        ref display);
                }
            });
            Skan_progres.Value += 1;
            slider.Value = display;
            int cod_dt = 0;
            int cod_iz = 0;
            await Task.Run(() =>
            {
                lock (window.balanceLock)
                {
                    window.HART_conection.MTM701_Comand_133(Properties.Settings.Default.Master, window.Devise_long_adres, 0,
                        ref cod_dt,ref cod_iz);
                }
            });
            Skan_progres.Value += 1;
            C_Temp_sensor.SelectedIndex = cod_dt;
            C_Pres_sensor.SelectedIndex = cod_iz;
            int tupe = 0;
            int tupe2 = 0;
            float lou = 0;
            float upp = 0;
            float dum = 0;
            await Task.Run(() =>
            {
                lock (window.balanceLock)
                {
                    window.HART_conection.MTM701_Comand_132(Properties.Settings.Default.Master, window.Devise_long_adres, 0,0,
                        ref tupe,ref tupe2,ref lou ,ref upp,ref dum);
                }
            });
            Skan_progres.Value += 1;
            C_ys1_log.SelectedIndex = tupe;
            C_ys1_tec.SelectedIndex = tupe2;
            T_ys1_Lower.Text = lou.ToString();
            T_ys1_Upper.Text = upp.ToString();
            T_ys1_Hyster.Text = dum.ToString();
            await Task.Run(() =>
            {
                lock (window.balanceLock)
                {
                    window.HART_conection.MTM701_Comand_132(Properties.Settings.Default.Master, window.Devise_long_adres, 0, 1,
                        ref tupe, ref tupe2, ref lou, ref upp, ref dum);
                }
            });
            Skan_progres.Value += 1;
            C_ys2_log.SelectedIndex = tupe;
            C_ys2_tec.SelectedIndex = tupe2;
            T_ys2_Lower.Text = lou.ToString();
            T_ys2_Upper.Text = upp.ToString();
            T_ys2_Hyster.Text = dum.ToString();
            Skan_progres.Value -= 4;
            Skan_progres.Maximum -= 4;
            P_Ystavki.IsEnabled = true;
        }

        private async void B_dev_Contrast_write_Click(object sender, RoutedEventArgs e)
        {
            P_Ystavki.IsEnabled = false;
            Skan_progres.Maximum += 4;
            int display = (int)slider.Value;

            await Task.Run(() =>
            {
                lock (window.balanceLock)
                {
                    window.HART_conection.MTM701_Comand_131(Properties.Settings.Default.Master, window.Devise_long_adres, 1,
                        ref display);
                }
            });
            Skan_progres.Value += 1;
           
            int cod_dt = C_Temp_sensor.SelectedIndex;
            int cod_iz = C_Pres_sensor.SelectedIndex;
            await Task.Run(() =>
            {
                lock (window.balanceLock)
                {
                    window.HART_conection.MTM701_Comand_133(Properties.Settings.Default.Master, window.Devise_long_adres, 1,
                        ref cod_dt, ref cod_iz);
                }
            });
            Skan_progres.Value += 1;
          
            int tupe = C_ys1_log.SelectedIndex;
            int tupe2 = C_ys1_tec.SelectedIndex;
            float lou = Convert.ToSingle(T_ys1_Lower.Text);
            float upp = Convert.ToSingle(T_ys1_Upper.Text);
            float dum = Convert.ToSingle(T_ys1_Hyster.Text);
            await Task.Run(() =>
            {
                lock (window.balanceLock)
                {
                    window.HART_conection.MTM701_Comand_132(Properties.Settings.Default.Master, window.Devise_long_adres, 1, 0,
                        ref tupe, ref tupe2, ref lou, ref upp, ref dum);
                }
            });
            Skan_progres.Value += 1;
             tupe = C_ys2_log.SelectedIndex;
             tupe2 = C_ys2_tec.SelectedIndex;
             lou = Convert.ToSingle(T_ys2_Lower.Text);
             upp = Convert.ToSingle(T_ys2_Upper.Text);
             dum = Convert.ToSingle(T_ys2_Hyster.Text);
            await Task.Run(() =>
            {
                lock (window.balanceLock)
                {
                    window.HART_conection.MTM701_Comand_132(Properties.Settings.Default.Master, window.Devise_long_adres, 1, 1,
                        ref tupe, ref tupe2, ref lou, ref upp, ref dum);
                }
            });
            Skan_progres.Value += 1;
         
            Skan_progres.Value -= 4;
            Skan_progres.Maximum -= 4;
            P_Ystavki.IsEnabled = true;
        }

        private async void B_dev_Table_read_Click(object sender, RoutedEventArgs e)
        {
            Skan_progres.Maximum += 36;
            
            test1.IsEnabled = false;
            B_dev_Table_read.IsEnabled = false;
            B_dev_Table_write.IsEnabled = false;
            for (int i = 0; i <= 4; i++)
            {
                float temp = 0;
                string kod1 = "";
                string kod2 = "";
                string kod3 = "";
                await Task.Run(() =>
                {
                  lock (window.balanceLock)
                  {
                    window.HART_conection.MTM701_Comand_134(Properties.Settings.Default.Master, window.Devise_long_adres, 0, i,
                        ref temp,
                        ref kod1,
                        ref kod2,
                        ref kod3);
                  }
                });
                Calib[i].ID = i;
                Calib[i].Temperature = temp;
                Calib[i].Kod_ACP_Temperature_Sensor = kod1;
                Calib[i].Kod_ACP_Voltage_Sensor = kod2;
                Calib[i].Kod_ACP_Сurrent_Sensor = kod3;
                test1.Items.Refresh();
                for (int j = 0; j <= 9; j++)
                {
                    float pres = 0;
                    string kod_1 = "", kod_2 = "", kod_3 = "", kod_4 = "";

                    await Task.Run(() =>
                    {
                        lock (window.balanceLock)
                        {
                            window.HART_conection.MTM701_Comand_135(Properties.Settings.Default.Master, window.Devise_long_adres, 0, i,j, 
                            ref pres, 
                            ref kod_1,
                            ref kod_2, 
                            ref kod_3,
                            ref kod_4);
                        }
                    });
                    Calib[i]._Davs[j].ID = j;
                    Calib[i]._Davs[j].Pressure = pres;
                    Calib[i]._Davs[j].Kod_ACP_1 = kod_1;
                    Calib[i]._Davs[j].Kod_ACP_2 = kod_2;
                    Calib[i]._Davs[j].Kod_ACP_3 = kod_3;
                    Calib[i]._Davs[j].Kod_ACP_4 = kod_4;
                    Skan_progres.Value = Skan_progres.Value + 1;
                    test1.Items.Refresh();
                }
                
            }
            Skan_progres.Value -= 36;
            Skan_progres.Maximum -= 36;
            test1.IsEnabled = true;
            B_dev_Table_read.IsEnabled = true;
            B_dev_Table_write.IsEnabled = true;
        }

        private async void B_dev_Table_write_Click(object sender, RoutedEventArgs e)
        {
            Skan_progres.Maximum += 36;
            test1.IsEnabled = false;
            B_dev_Table_read.IsEnabled = false;
            B_dev_Table_write.IsEnabled = false;
            for (int i = 0; i <= 4; i++)
            {
                float temp = Calib[i].Temperature;
                string kod1 = Calib[i].Kod_ACP_Temperature_Sensor;
                string kod2 = Calib[i].Kod_ACP_Voltage_Sensor;
                string kod3 = Calib[i].Kod_ACP_Сurrent_Sensor;
                await Task.Run(() =>
                {
                    lock (window.balanceLock)
                    {
                        window.HART_conection.MTM701_Comand_134(Properties.Settings.Default.Master, window.Devise_long_adres, 1, i,
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

                    await Task.Run(() =>
                    {
                        lock (window.balanceLock)
                        {
                            window.HART_conection.MTM701_Comand_135(Properties.Settings.Default.Master, window.Devise_long_adres, 1, i, j,
                            ref pres,
                            ref kod_1,
                            ref kod_2,
                            ref kod_3,
                            ref kod_4);
                        }
                    });
                    
                  
                    Skan_progres.Value = Skan_progres.Value + 1;
                    
                }

            }


            Skan_progres.Value -= 36;
            Skan_progres.Maximum -= 36;
            test1.IsEnabled = true;
            B_dev_Table_read.IsEnabled = true;
            B_dev_Table_write.IsEnabled = true;
        }

        

        private async void B_dev_Curent_read_Click(object sender, RoutedEventArgs e)
        {
            P_Curent.IsEnabled = false;
            float min = 0;
            string min_a = "";
            string min_c = "";
            float max = 0;
            string max_a = "";
            string max_c = "";
            await Task.Run(() =>
            {
                lock (window.balanceLock)
                {
                    window.HART_conection.MTM701_Comand_136(Properties.Settings.Default.Master, window.Devise_long_adres, 0,
                        ref min,
                        ref max,
                        ref min_c,
                        ref min_a,
                        ref max_c,
                        ref max_a);
                }
            });
            T_max.Text = max.ToString();
            T_min.Text = min.ToString();
            T_max_A.Text = max_a;
            T_max_D.Text = max_c;
            T_min_A.Text = min_a;
            T_min_D.Text = min_c;
            P_Curent.IsEnabled = true;
        }

        private async void B_dev_Curent_write_Click(object sender, RoutedEventArgs e)
        {
            P_Curent.IsEnabled = false;
            float min = Convert.ToSingle(T_min.Text);
            string min_a = T_min_A.Text;
            string min_c = T_min_D.Text;
            float max = Convert.ToSingle(T_max.Text);
            string max_a = T_max_A.Text;
            string max_c = T_max_D.Text;
            await Task.Run(() =>
            {
                lock (window.balanceLock)
                {
                    window.HART_conection.MTM701_Comand_136(Properties.Settings.Default.Master, window.Devise_long_adres, 1,
                        ref min,
                        ref max,
                        ref min_c,
                        ref min_a,
                        ref max_c,
                        ref max_a);
                }
            });
           
            P_Curent.IsEnabled = true;
        }

        private async void B_dev_Acp_read_Click(object sender, RoutedEventArgs e)
        {
            P_ACP.IsEnabled = false;

            string[] s_res = { };

            await Task.Run(() =>
            {
                lock (window.balanceLock)
                {
                    window.HART_conection.MTM701_Comand_130(Properties.Settings.Default.Master, window.Devise_long_adres,
                        ref s_res);
                }
            });

            T_ACP_1.Text = s_res[0];
            T_ACP_2.Text = s_res[1];
            T_ACP_3.Text = s_res[2];
            T_ACP_4.Text = s_res[3];
            T_ACP_5.Text = s_res[4];
            P_ACP.IsEnabled = true;
        }

        private void B_Saive_Click(object sender, RoutedEventArgs e)
        {
            Saive();
        }

        private void B_Load_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }
    }
}
