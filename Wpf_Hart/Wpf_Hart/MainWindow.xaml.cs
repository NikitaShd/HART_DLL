using Class_HART;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Wpf_Hart
{
    public class MarginConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Thickness(System.Convert.ToDouble(value), 40, 0, 0);
        }
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    public class ReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((SeriesCollection)value).Reverse();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class OpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible
                ? 1d
                : .2d;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Conect HART_conection = new Conect(); // основной клас для взаимодействия с устройством
        // == списки предопредёлёніх значений 
        public List<string> S_Alarm_cods { get; set; } = new List<string> { };
        public List<string> S_Unit_cods { get; set; } = new List<string> { };
        public List<string> S_Protect_cods { get; set; } = new List<string> { };
        public List<string> S_Transfer_cods { get; set; } = new List<string> { };
        public List<string> S_timer_s { get; set; } = new List<string> { "10s", "20s", "40s", "1m", "5m", "10m", "20m", "30m", "1h" };
        public List<string> S_timer2_s { get; set; } = new List<string> { "5s", "10s", "15s", "20s", "40s", "1m", "5m", "10m", "30m" };
        // ====================================
        string this_usb = ""; // адрес текущего выбраного порта 
        public ObservableCollection<string> usb { get; set; } = new ObservableCollection<string> { }; // список виртуальных портов

        public readonly object balanceLock = new object(); // блокиратор потоков для предотвращения одновременого выполнения нескольких команд
        public Byte[] Devise_long_adres = { }; // адрес текущего выбраного устройства

        DispatcherTimer timer_Grapics = new DispatcherTimer(); // таймер для опроса устройства и записи в вкладке Grapics
        DispatcherTimer timer_Sensors = new DispatcherTimer();// таймер для опроса устройства и записи в вкладке Sensors 
        public struct devaise // структурв даных для хранения списка найденых устройств 
        {
            public string Short_Address { get; set; }
            public string Long_Address { get; set; }
            public string Device_Version { get; set; }
            public string Software_Version { get; set; }
            public string Assembly_Number { get; set; }
        }
        public ObservableCollection<devaise> HART_dev_list { get; set; } = new ObservableCollection<devaise> { }; //список найденых устройств

        private const int WM_DEVICECHANGE = 0x0219;  // int = 537
        private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00000004;

        // проверка коректност ввода для числового формата
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        // проверка коректност ввода для Hex формата
        private void HexValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9A-Fa-f-x]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DEVICECHANGE)
            {
                ReadDongleHeader();
            }
            return IntPtr.Zero;
        }
        public string[] GetSerialPort() //поиск доступных портов
        {
            string[] ports = SerialPort.GetPortNames();
            try
            {
                foreach (string mo in ports)
                {
                    SerialPort temp = new SerialPort(mo);
                    bool t = temp.IsOpen;
                    temp.Dispose();
                }
            }
            catch { }
            ports = SerialPort.GetPortNames();
            return ports;
        }
        private void ReadDongleHeader() // срабатывает при подключении\отключении usb
        {
            string[] ports = GetSerialPort();
            usb.Clear();
            foreach (string port in ports)
            {
                usb.Add(port);//обновляем список портов
            }
            if (usb.Contains(this_usb))
            {
                ComboBox_UsbDevaise.SelectedIndex = usb.IndexOf(this_usb);
                this_usb = "";
            }
            else if (usb.Count > 0)
            {
                HART_conection.close();
                ComboBox_UsbDevaise.SelectedIndex = 0;
            }
        }
        public SeriesCollection Series { get; set; } // даные для отображения на первом графике
        public SeriesCollection Series2 { get; set; } // даные для отображения на втором графике
        public List<string> Labels { get; set; } = new List<string>(); // даные для отображения на подписи на обоих графиках
        private void ListBox_OnPreviewMouseDown(object sender, MouseButtonEventArgs e) // скрывает либо показует ненужный график по клику на его имя
        {
            var item = ItemsControl.ContainerFromElement(ListBox1, (DependencyObject)e.OriginalSource) as ListBoxItem;
            if (item == null) return;

            var series = (LineSeries)item.Content;
            series.Visibility = series.Visibility == Visibility.Visible
                ? Visibility.Hidden
                : Visibility.Visible;
        }
        private void ListBox_OnPreviewMouseDown2(object sender, MouseButtonEventArgs e)// скрывает либо показует ненужный график по клику на его имя на втором графике
        {
            var item = ItemsControl.ContainerFromElement(ListBox2, (DependencyObject)e.OriginalSource) as ListBoxItem;
            if (item == null) return;

            var series = (LineSeries)item.Content;
            series.Visibility = series.Visibility == Visibility.Visible
                ? Visibility.Hidden
                : Visibility.Visible;
        }
        public MainWindow()
        {
            //== настраеваем таймеры ===================
            timer_Grapics.Interval = TimeSpan.FromMilliseconds(10000);
            timer_Grapics.Tick += timer_Grapics_Tick;
            timer_Sensors.Interval = TimeSpan.FromMilliseconds(10000);
            timer_Sensors.Tick += timer_Sensors_Tick;
            //===============================================
            // == установливаем стандартное отображение для графиков ==
            Series = new SeriesCollection 
            {
                new LineSeries
                {
                    Values = new ChartValues<float> {20, 30, 35, 45, 20, 35},
                    Title = "Param-1",
                    Fill = Brushes.Transparent
                },
                new LineSeries
                {
                    Values = new ChartValues<float> {10, 50, 10, 24, 22, 63},
                    Title = "Param-2",
                    Fill = Brushes.Transparent,
                },
                new LineSeries
                {
                    Values = new ChartValues<float> {5, 3, 45, 35, 35, 25},
                    Title = "Param-3",
                    Fill = Brushes.Transparent
                },
                new LineSeries
                {
                    Values = new ChartValues<float> {50, 10, 24, 22, 63, 20},
                    Title = "Param-4",
                    Fill = Brushes.Transparent

                }
            };
            Series2 = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<float> {20, 30, 35, 45, 65, 85},
                    Title = ".ma",
                    Fill = Brushes.Transparent
                },
                new LineSeries
                {
                    Values = new ChartValues<float> {10, 50, 10, 24, 22, 63},
                    Title = ".%",
                    Fill = Brushes.Transparent
                }
            };
            Labels.Add("00:00:00");
            Labels.Add("01:01:01");
            Labels.Add("02:02:02");
            Labels.Add("03:03:03");
            Labels.Add("04:04:04");
            Labels.Add("05:05:05");
            //=============================================
            //== получаем список кодов из класа  =========
            S_Alarm_cods.AddRange(_Tables.Alarm_Cods_arr());
            S_Protect_cods.AddRange(_Tables.Protect_Cods_arr());
            S_Transfer_cods.AddRange(_Tables.Transfer_Cods_arr());
            S_Unit_cods.AddRange(_Tables.unit_arr());
            this.DataContext = this;
            //== подгружаем тему приложения при запуске =====
            ResourceDictionary dictionary = new ResourceDictionary();
            if (!Properties.Settings.Default.Darkmode)
            {
                var uri = new Uri("Dictionary_Lite.xaml", UriKind.Relative);
                dictionary = Application.LoadComponent(uri) as ResourceDictionary;
                Application.Current.Resources.Clear();
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(dictionary);
            }
            else
            {
                var uri = new Uri("Dictionary_dark.xaml", UriKind.Relative);
                dictionary = Application.LoadComponent(uri) as ResourceDictionary;
                Application.Current.Resources.Clear();
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(dictionary);
            }
            //== подгружаем языковой пакет ===== 
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(Properties.Settings.Default.Langue);
           
            InitializeComponent(); // иницыализацыя окна 

            List_menu.SelectedIndex = 0; // устанавливаем по умолчанию выбраный первый элемент меню
            Tab_control_main.SelectedIndex = 0;// устанавливаем по умолчанию первую панель 
          
            Load_propertis();// подгружаем настойки 
            if (!Properties.Settings.Default.Staite)
            {
                ButtonCloseMenu.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // Востанавливаем состояние боковой панели из настроек
            }
            if (Properties.Settings.Default.Maximaize == true) this.WindowState = WindowState.Maximized;// Востанавливаем состояние окна из настроек
          
            ReadDongleHeader();//получаем список доступных портов

            //== нужно чтобы в редакторе вкладки отображались а в програме нет ==================
            Style s = new Style();
            s.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Collapsed));//убираем полосу вкладок в стиле
            Tab_control_main.ItemContainerStyle = s;// присваеваем стил нашей панельке 
            //===========================================================================================
        }


        //изменение размена окна 
        private void stateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                Properties.Settings.Default.Maximaize = false;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                Properties.Settings.Default.Maximaize = true;
            }
        }

        //меняем местами кнопки выдвижного меню
        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
            Properties.Settings.Default.Staite = false;
        }

        //меняем местами кнопки выдвижного меню
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            Properties.Settings.Default.Staite = true;
        }

        //Срабатывает при закрытии преложение 
        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized) // сохраняем размер окна 
            {
                Properties.Settings.Default.Width = Application.Current.MainWindow.Width;
                Properties.Settings.Default.Height = Application.Current.MainWindow.Height;
            }
            Properties.Settings.Default.Save(); // сохраняем все настройки 

            base.OnClosing(e);
        }

        // кнопка закрыть окно
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //кнопка свернуть окно
        private void Minimaize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        int item = 0;//хранит индекс выбраного в боковой панелиэлемента 
        private void List_menu_MouseDown(object sender, MouseButtonEventArgs e) // переключение панелей при клике на боковвую панел
        {
            if ((List_menu.SelectedIndex != List_menu.Items.Count - 2) && (List_menu.SelectedIndex != List_menu.Items.Count - 1))
            {
                item = List_menu.SelectedIndex;
                if (item != -1) Tab_control_main.SelectedIndex = item; // преключаем таб контрол в зависимости от выбраного пункта 
            }
            else
            {
                if (List_menu.SelectedIndex == List_menu.Items.Count - 2) { System.Diagnostics.Process.Start("explorer.exe", "http://github.com/TviZet/HART_DLL/wiki"); }
                if (List_menu.SelectedIndex == List_menu.Items.Count - 1) { System.Diagnostics.Process.Start("explorer.exe", "http://github.com/TviZet/HART_DLL"); }
                List_menu.SelectedIndex = item;
            }
        }
        //перетаскивание формы
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        // происходит при выборе порта 
        private void ComboBox_UsbDevaise_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ComboBox_UsbDevaise.SelectedItem != null))
            {
                if (this_usb != ComboBox_UsbDevaise.SelectedItem.ToString()) // проверка не выбрали ли мы наш порт ещо раз 
                {
                    this_usb = ComboBox_UsbDevaise.SelectedItem.ToString();// обновляем текуше выбраный порт
                    if ((Properties.Settings.Default.AvtoCOM))
                    {
                        HART_conection.close(); // закрывем предыдущий порт 
                        Load_propertis(); // подгружаем настройки 
                        string conect_usb_staite = HART_conection.init(this_usb);//иницеализируем наш порт
                        if (conect_usb_staite == "True")// проверяем статус 
                        {
                            usb_stats.Content = "Conect";
                        }
                        else
                        {
                            usb_stats.Content = "Disconect";
                        }
                    }
                }
            }
        }
        // кнопка поиск устройств 
        private void B_findDevaise_Click(object sender, RoutedEventArgs e)
        {
            if (HART_conection.scan == false) // проверяем не начат ли уже поиск устройств 
            {
                Thread thread = new Thread(UpdateDevise);//поток для обновления графики 
                thread.Start();

                HART_conection.Scan_start(0);//запускаем сканирывание 
            }

        }
        private void UpdateDevise() // обновляем список устройств в отдельном потоке 
        {
            this.Dispatcher.BeginInvoke(new Action(() => //блокируем кнопки 
            {
                listBox_dev.IsEnabled = false;
                ComboBox_UsbDevaise.IsEnabled = false;
                B_findDevaise.IsEnabled = false;
                B_findDevaiseStop.IsEnabled = true;
            }));
            Thread.Sleep(1000);
            while (HART_conection.scan)// цыкл будет выполнятся до тех пор пока идёт сканирывание 
            {
                Thread.Sleep(500); // задержка 
                this.Dispatcher.BeginInvoke(new Action(() => 
                {
                    HART_dev_list.Clear();
                    this.Skan_progres.Value = HART_conection.scan_step; // обновляем прогрес бар
                    foreach (Conect.Read_Fraim item in HART_conection.Net_config()) //получаем список найденых устройств 
                    {
                        devaise temp = new devaise();
                        temp.Long_Address =
                        item.DT[1].ToString("X2") + '-' +
                        item.DT[2].ToString("X2") + '-' +
                        item.DT[9].ToString("X2") + '-' +
                        item.DT[10].ToString("X2") + '-' +
                        item.DT[11].ToString("X2");

                        temp.Assembly_Number = BitConverter.ToString(item.DT, 9);
                        temp.Software_Version = item.DT[6].ToString("X2");
                        temp.Device_Version = item.DT[7].ToString("X2");
                        temp.Short_Address = item.AD_Short.ToString();
                        HART_dev_list.Add(temp);//добавляем устройства в список устройств 
                    }
                }));
            }
            this.Dispatcher.BeginInvoke(new Action(() => //разблакируем кнопки кнопки
            {
                this.Skan_progres.Value = 0;
                listBox_dev.IsEnabled = true;
                ComboBox_UsbDevaise.IsEnabled = true;
                B_findDevaise.IsEnabled = true;
                B_findDevaiseStop.IsEnabled = false;
            }));
        }
        //кнопка завершения поиска 
        private void B_findDevaiseStop_Click(object sender, RoutedEventArgs e) 
        {
            B_findDevaiseStop.IsEnabled = false;
            HART_conection.Scan_stop();// посылаем кооманду на коректное завершения поиска 
        }

        private void Setings_Click(object sender, RoutedEventArgs e) // вызов диалогового окна с настройками 
        {
            Window_setings SetingsWindow = new Window_setings();
            if (SetingsWindow.ShowDialog() == true)
            {
                HART_conection.close();
                Load_propertis();
                string conect_usb_staite = HART_conection.init(this_usb);
                if (conect_usb_staite == "True")
                {
                    usb_stats.Content = "Conect";
                }
                else
                {
                    usb_stats.Content = "Disconect";
                }
            }
        }
        private void Load_propertis()//загружает настройки в переменые из Properties
        {
            HART_conection.Master = Properties.Settings.Default.Master;
            HART_conection.Spide = Properties.Settings.Default.Spide;
            HART_conection.preambula_leng = Properties.Settings.Default.preambula_leng;
            HART_conection.write_taimout = Properties.Settings.Default.write_taim;
            HART_conection.write_taim = Properties.Settings.Default.write_taimout;
            Application.Current.MainWindow.Width = Properties.Settings.Default.Width;
            Application.Current.MainWindow.Height = Properties.Settings.Default.Height;
        }
        //кнопка смены темы приложения
        private void Darkmode_Click(object sender, RoutedEventArgs e)
        {
            Info info_ = new Info();
            if (info_.ShowDialog() == true)
            {
                Set_them();
            }
           
        }
        void Set_them() // функцыя смены темы приложения 
        {
            if (Properties.Settings.Default.Darkmode)
            {
                Properties.Settings.Default.Darkmode = false;
            }
            else
            {
                Properties.Settings.Default.Darkmode = true;
            }
            Properties.Settings.Default.Save();
            var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(currentExecutablePath);
            Application.Current.Shutdown();
        } 
        // возникает при выборе устройства 
        private void listBox_dev_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox_dev.SelectedItem != null)
            {
                devaise a = (devaise)listBox_dev.SelectedItem;
                Devise_long_adres = _Convert.GetBytes(a.Long_Address);// записуем выбраное устройство 
                Label_DevLongAdres.Text = "HART Device [ " + BitConverter.ToString(Devise_long_adres) + " ]";
                listBox_dev.SelectedIndex = -1;
            }
        }
        // считывание базовых параметров об устройств 
        private async void B_Comand_0_Click(object sender, RoutedEventArgs e)
        {
            string[] temp = { };
            P_basic_param.IsEnabled = false;
            await Task.Factory.StartNew(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_0(Properties.Settings.Default.Master, Devise_long_adres, ref temp);
                }
            });
            P_basic_param.IsEnabled = true;
            L_Manufacturer_Code.Content = Properties.Resource.R_Manufacturer_Code + temp[0];
            L_Device_Type_Code.Content = Properties.Resource.R_Device_Type_Code + temp[1];
            L_Preambul_leng.Content = Properties.Resource.R_Preambul_leng + temp[2];
            L_Universal_commands.Content = Properties.Resource.R_Universal_commands + temp[3];
            L_Specific_commands.Content = Properties.Resource.R_Specific_commands + temp[4];
            L_Software_version.Content = Properties.Resource.R_Software_version + temp[5];
            L_Hardware_version.Content = Properties.Resource.R_Hardware_version + temp[6];
            L_Device_function.Content = Properties.Resource.R_Device_function + temp[8];
        }
        // считывание продвинутой информации по устройству 
        private async void B_dev_info_read_Click(object sender, RoutedEventArgs e)
        {
            P_info.IsEnabled = false;
            string mes = "";
            string[] teg_discriptor_data = { };
            string long_teg = "";
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    mes = HART_conection.Comand_12(Properties.Settings.Default.Master, Devise_long_adres);
                }
            });
            T_dev_mes.Text = mes;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    teg_discriptor_data = HART_conection.Comand_13(Properties.Settings.Default.Master, Devise_long_adres);
                }
            });
            T_s_teg.Text = teg_discriptor_data[0];
            T_discriptor.Text = teg_discriptor_data[1];
            T_data.Text = teg_discriptor_data[2];
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_20(Properties.Settings.Default.Master, Devise_long_adres, ref long_teg);
                }
            });
            T_L_teg.Text = long_teg;
            P_info.IsEnabled = true;
        }
        // запись продвинутой информации о устройстве 
        private async void B_dev_info_write_Click(object sender, RoutedEventArgs e)
        {
            P_info.IsEnabled = false;
            string mes = T_dev_mes.Text;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_17(Properties.Settings.Default.Master, Devise_long_adres, mes.ToUpper());
                    // Thread.Sleep(500);
                }
            });
            string s_teg = T_s_teg.Text;
            string discriptor = T_discriptor.Text;
            string data = T_data.Text;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_18(Properties.Settings.Default.Master, Devise_long_adres, s_teg.ToUpper(), discriptor.ToUpper(), data);
                    //Thread.Sleep(500);
                }
            });
            string l_teg = T_L_teg.Text;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_22(Properties.Settings.Default.Master, Devise_long_adres, l_teg);
                    // Thread.Sleep(500);
                }
            });
            P_info.IsEnabled = true;
        }
        private async void B_dev_Extended_Info_read_Click(object sender, RoutedEventArgs e)
        {
            P_Extended_Info.IsEnabled = false;
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
                    HART_conection.Comand_15(Properties.Settings.Default.Master, Devise_long_adres, ref Alarm, ref Transfer, ref Unit, ref U_renge, ref L_renge, ref Damfing, ref Protect, ref Manufacturer);
                }
            });
            ComboBox_Signal.SelectedItem = _Tables.Alarm_Cods(Alarm);
            ComboBox_Function.SelectedItem = _Tables.Transfer_Cods(Transfer);
            ComboBox_Protect.SelectedItem = _Tables.Protect_Cods(Protect);
            ComboBox_Units.SelectedItem = _Tables.Encod_unit(Unit);
            T_U_renge.Text = U_renge.ToString();
            T_L_renge.Text = L_renge.ToString();
            T_Demp.Text = Damfing.ToString();
            T_Manufacturer.Text = Manufacturer.ToString();
            P_Extended_Info.IsEnabled = true;
        }
        private async void B_dev_Extended_Info_write_Click(object sender, RoutedEventArgs e)
        {
            P_Extended_Info.IsEnabled = false;
            int Alarm = _Tables.Alarm_Cods(ComboBox_Signal.SelectedItem.ToString());
            int Transfer = _Tables.Transfer_Cods(ComboBox_Function.SelectedItem.ToString());
            int Unit = _Tables.Encod_unit(ComboBox_Units.SelectedItem.ToString());
            float U_renge = Convert.ToSingle(T_U_renge.Text);
            float L_renge = Convert.ToSingle(T_L_renge.Text);
            float Damfing = Convert.ToSingle(T_Demp.Text);
            int Protect = _Tables.Protect_Cods(ComboBox_Protect.SelectedItem.ToString());
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_35(Properties.Settings.Default.Master, Devise_long_adres, Unit, U_renge, L_renge);
                }
            });
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_34(Properties.Settings.Default.Master, Devise_long_adres, Damfing);
                }
            });
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_47(Properties.Settings.Default.Master, Devise_long_adres, Transfer);
                }
            });

            P_Extended_Info.IsEnabled = true;
        }
        private async void B_dev_fixed_current_set_Click(object sender, RoutedEventArgs e)
        {
            P_Fixed_Current.IsEnabled = false;
            float temp = Convert.ToSingle(T_fixed_current.Text);
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_40(Properties.Settings.Default.Master, Devise_long_adres, temp);
                }
            });
            P_Fixed_Current.IsEnabled = true;
        }
        private async void B_dev_fixed_current_exit_Click(object sender, RoutedEventArgs e)
        {
            P_Fixed_Current.IsEnabled = false;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_40(Properties.Settings.Default.Master, Devise_long_adres, 0);
                }
            });
            P_Fixed_Current.IsEnabled = true;
        }
        private async void B_dev_zero_set_Click(object sender, RoutedEventArgs e)
        {
            P_Zero.IsEnabled = false;
            float temp = Convert.ToSingle(T_Zero.Text);
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_45(Properties.Settings.Default.Master, Devise_long_adres, temp);
                }
            });
            P_Zero.IsEnabled = true;
        }
        private async void B_dev_coficent_set_Click(object sender, RoutedEventArgs e)
        {
            P_Coficent.IsEnabled = false;
            float temp = Convert.ToSingle(T_Coficent.Text);
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_46(Properties.Settings.Default.Master, Devise_long_adres, temp);
                }
            });
            P_Coficent.IsEnabled = true;
        }
        private async void B_dev_U_range_value_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_36(Properties.Settings.Default.Master, Devise_long_adres);
                }
            });
            ((Button)sender).IsEnabled = true;
        }
        private async void B_dev_L_range_value_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_37(Properties.Settings.Default.Master, Devise_long_adres);
                }
            });
            ((Button)sender).IsEnabled = true;
        }
        private async void B_dev_Prim_value_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_43(Properties.Settings.Default.Master, Devise_long_adres);
                }
            });
            ((Button)sender).IsEnabled = true;
        }
        private async void B_dev_eeprom_burn_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_39(Properties.Settings.Default.Master, Devise_long_adres, 0);
                }
            });
            ((Button)sender).IsEnabled = true;
        }
        private async void B_dev_eeprom_restor_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_39(Properties.Settings.Default.Master, Devise_long_adres, 1);
                }
            });
            ((Button)sender).IsEnabled = true;
        }
        private async void B_dev_reset_flad_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_38(Properties.Settings.Default.Master, Devise_long_adres);
                }
            });
            ((Button)sender).IsEnabled = true;
        }
        private async void B_dev_reset_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_42(Properties.Settings.Default.Master, Devise_long_adres);
                }
            });
            ((Button)sender).IsEnabled = true;
        }

        private async void B_dev_Preambul_read_Click(object sender, RoutedEventArgs e)
        {
            P_preambula.IsEnabled = false;
            string[] temp = { };
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_0(Properties.Settings.Default.Master, Devise_long_adres, ref temp);
                }
            });
            T_Preambul_l.Text = temp[2];
            P_preambula.IsEnabled = true;
        }
        private async void B_dev_Preambul_write_Click(object sender, RoutedEventArgs e)
        {
            P_preambula.IsEnabled = false;
            int temp = Convert.ToInt32(T_Preambul_l.Text);
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_59(Properties.Settings.Default.Master, Devise_long_adres, temp);
                }
            });

            P_preambula.IsEnabled = true;
        }
        private async void B_dev_Short_ad_write_Click(object sender, RoutedEventArgs e)
        {
            P_short_ad.IsEnabled = false;
            int temp = Convert.ToInt32(T_Short_ad.Text);
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_6(Properties.Settings.Default.Master, Devise_long_adres, temp);
                }
            });

            P_short_ad.IsEnabled = true;
        }
        private async void B_dev_Serial_num_read_Click(object sender, RoutedEventArgs e)
        {
            P_serial_num.IsEnabled = false;
            string temp = "";
            string kod = "";
            float Min = 0;
            float Max = 0;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_14(Properties.Settings.Default.Master, Devise_long_adres, ref temp, ref kod, ref Min, ref Max);
                }
            });
            T_Serial_num.Text = temp;
            P_serial_num.IsEnabled = true;
        }
        private async void B_dev_Serial_num_write_Click(object sender, RoutedEventArgs e)
        {
            P_serial_num.IsEnabled = false;
            string temp = T_Serial_num.Text;

            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_49(Properties.Settings.Default.Master, Devise_long_adres, _Convert.GetBytes(temp.Replace("-", "")));
                }
            });

            P_serial_num.IsEnabled = true;
        }

        bool isferst = true; // переманая хранит был ли это первое выполнениие даной функции 
        private async void Dat_chart_ubdate()// записует новые даные на график 
        {
            if (isferst) // если это первое выполнение функции то очищяем график 
            {
                C_unit_value.Series[0].Values.Clear();
                C_unit_value.Series[1].Values.Clear();
                C_unit_value.Series[2].Values.Clear();
                C_unit_value.Series[3].Values.Clear();
                C_unit_value2.Series[0].Values.Clear();
                C_unit_value2.Series[1].Values.Clear();
                Labels.Clear();
                isferst = false;
            }
            float tok = 0; float proc = 0;
            string kod_1 = ""; float par_1 = 0;
            string kod_2 = ""; float par_2 = 0;
            string kod_3 = ""; float par_3 = 0;
            string kod_4 = ""; float par_4 = 0;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_3(Properties.Settings.Default.Master, Devise_long_adres, ref tok, ref kod_1, ref par_1, ref kod_2, ref par_2, ref kod_3, ref par_3, ref kod_4, ref par_4);
                }
            });
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_2(Properties.Settings.Default.Master, Devise_long_adres, ref tok, ref proc);
                }
            });
            //==================
            if (par_1 != -1) C_unit_value.Series[0].Values.Add(par_1); else C_unit_value.Series[0].Values.Add(float.NaN);
            if (par_2 != -1) C_unit_value.Series[1].Values.Add(par_2); else C_unit_value.Series[1].Values.Add(float.NaN);
            if (par_3 != -1) C_unit_value.Series[2].Values.Add(par_3); else C_unit_value.Series[2].Values.Add(float.NaN);
            if (par_4 != -1) C_unit_value.Series[3].Values.Add(par_4); else C_unit_value.Series[3].Values.Add(float.NaN);
            //==================
            T_par_1.Text = par_1.ToString() + "(" + kod_1 + ")  ";
            T_par_2.Text = par_2.ToString() + "(" + kod_2 + ")  ";
            T_par_3.Text = par_3.ToString() + "(" + kod_3 + ")  ";
            T_par_4.Text = par_4.ToString() + "(" + kod_4 + ")  ";
            //==================
            if (tok != -1) C_unit_value2.Series[0].Values.Add(tok); else C_unit_value2.Series[0].Values.Add(float.NaN);
            if (proc != -1) C_unit_value2.Series[1].Values.Add(proc); else C_unit_value2.Series[1].Values.Add(float.NaN);
            //==================
            T_par_tok.Text = tok.ToString() + "(.ma)  ";
            T_par_proc.Text = proc.ToString() + "(.%)  ";
            //==================
            Labels.Add(DateTime.Now.ToString());
        }
        // нажатие на кнопку прочитать даные на вкладке с графиками 
        private void B_charts_add_Click(object sender, RoutedEventArgs e)
        {
          Dat_chart_ubdate();
        }

        // очистка графика 
        private void B_Clear_charts_Click(object sender, RoutedEventArgs e)
        {
            C_unit_value.Series[0].Values.Clear();
            C_unit_value.Series[1].Values.Clear();
            C_unit_value.Series[2].Values.Clear();
            C_unit_value.Series[3].Values.Clear();
            C_unit_value2.Series[0].Values.Clear();
            C_unit_value2.Series[1].Values.Clear();
            Labels.Clear();
        }
        // событие таймера для опроса прибора и составления графика с опредёлёним интермалом 
        void timer_Grapics_Tick(object sender, EventArgs e)
        {
            Dat_chart_ubdate();
        }
        // Запуск таймера для заполнение графиков 
        private void B_Start_timer_Click(object sender, RoutedEventArgs e)
        {
            B_Start_timer.IsEnabled = false;
            B_Stop_timer.IsEnabled = true;
            Dat_chart_ubdate();
            timer_Grapics.Start();
        }
        // остановка таймера для заполнения графиков
        private void B_Stop_timer_Click(object sender, RoutedEventArgs e)
        {
            B_Start_timer.IsEnabled = true;
            B_Stop_timer.IsEnabled = false;
            timer_Grapics.Stop();
        }

        // кнопка выбора команд для конкретного устройства 
        private void B_Select_Click(object sender, RoutedEventArgs e)
        {
            P_dev_list.Visibility = Visibility.Collapsed;
            Frame_devise.Visibility = Visibility.Visible;

            Frame_devise.Navigate(new Uri("/Devise/P_MTM701_7.xaml", UriKind.RelativeOrAbsolute));// подгружаем панель 
        }
        // закрытие панели 
        public void Frame_close()
        {
            P_dev_list.Visibility = Visibility.Visible;
            Frame_devise.Visibility = Visibility.Collapsed;
        }
        //изменяет интервал таймера в зависимости от выбраного значения 
        private void ComboBox_timer_s_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_timer_s.SelectedItem.ToString().Contains('s'))
            {
                timer_Grapics.Interval = TimeSpan.FromSeconds(Convert.ToDouble(ComboBox_timer_s.SelectedItem.ToString().Replace("s", "")));
            }
            if (ComboBox_timer_s.SelectedItem.ToString().Contains('m'))
            {
                timer_Grapics.Interval = TimeSpan.FromMinutes(Convert.ToDouble(ComboBox_timer_s.SelectedItem.ToString().Replace("m", "")));
            }
            if (ComboBox_timer_s.SelectedItem.ToString().Contains('h'))
            {
                timer_Grapics.Interval = TimeSpan.FromHours(Convert.ToDouble(ComboBox_timer_s.SelectedItem.ToString().Replace("h", "")));
            }
        }
        // считывает даные переменых и запись на вкладку Sensors 
        private async void SensorsRead()
        {
            B_Sensors_add.IsEnabled = false;
            float tok = 0; float proc = 0;
            string kod_1 = ""; float par_1 = 0;
            string kod_2 = ""; float par_2 = 0;
            string kod_3 = ""; float par_3 = 0;
            string kod_4 = ""; float par_4 = 0;
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_3(Properties.Settings.Default.Master, Devise_long_adres, ref tok, ref kod_1, ref par_1, ref kod_2, ref par_2, ref kod_3, ref par_3, ref kod_4, ref par_4);
                }
            });
            await Task.Run(() =>
            {
                lock (balanceLock)
                {
                    HART_conection.Comand_2(Properties.Settings.Default.Master, Devise_long_adres, ref tok, ref proc);
                }
            });
            //==============
            G_test.Value = Math.Round(par_1, 2);
            G_test2.Value = Math.Round(par_2, 2);
            G_test3.Value = Math.Round(par_3, 2);
            G_test4.Value = Math.Round(par_4, 2);
            G_test5.Value = Math.Round(tok, 2);
            G_test6.Value = Math.Round(proc, 2);
            //==============
            L_test.Content = kod_1;
            L_test2.Content = kod_2;
            L_test3.Content = kod_3;
            L_test4.Content = kod_4;
            //==============
            B_Sensors_add.IsEnabled = true;
        }
        // кнопка считівания параметров 
        private void B_Sensors_add_Click(object sender, RoutedEventArgs e)
        {
            SensorsRead();
        }
        //считівание параметров по таймеру 
        void timer_Sensors_Tick(object sender, EventArgs e)
        {
            SensorsRead();
        }
        //запуск таймера 
        private void B_Start_sensortimer_Click(object sender, RoutedEventArgs e)
        {
            B_Start_sensortimer.IsEnabled = false;
            B_Stop_sensortimer.IsEnabled = true;
            SensorsRead();
            timer_Sensors.Start();
        }
        //остановка таймера 
        private void B_Stop_sensortimer_Click(object sender, RoutedEventArgs e)
        {
            B_Start_sensortimer.IsEnabled = true;
            B_Stop_sensortimer.IsEnabled = false;
            timer_Sensors.Stop();
        }
        // изменения интервала таймера в зависимости от выбрагого значения
        private void ComboBox_sensortimer_s_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_sensortimer_s.SelectedItem.ToString().Contains('s'))
            {
                timer_Sensors.Interval = TimeSpan.FromSeconds(Convert.ToDouble(ComboBox_sensortimer_s.SelectedItem.ToString().Replace("s", "")));
            }
            if (ComboBox_sensortimer_s.SelectedItem.ToString().Contains('m'))
            {
                timer_Sensors.Interval = TimeSpan.FromMinutes(Convert.ToDouble(ComboBox_sensortimer_s.SelectedItem.ToString().Replace("m", "")));
            }
            if (ComboBox_sensortimer_s.SelectedItem.ToString().Contains('h'))
            {
                timer_Sensors.Interval = TimeSpan.FromHours(Convert.ToDouble(ComboBox_sensortimer_s.SelectedItem.ToString().Replace("h", "")));
            }
        }
        // чтение графика из файла 
        private void B_read_file_Click(object sender, RoutedEventArgs e)
        {
            List<string> temp = new List<string>();
            string[] subs;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Data (*.dat)|*.dat";
            if (openFileDialog.ShowDialog() == true)
            {
                C_unit_value.Series[0].Values.Clear();
                C_unit_value.Series[1].Values.Clear();
                C_unit_value.Series[2].Values.Clear();
                C_unit_value.Series[3].Values.Clear();
                C_unit_value2.Series[0].Values.Clear();
                C_unit_value2.Series[1].Values.Clear();
                Labels.Clear();

                temp = File.ReadAllLines(openFileDialog.FileName).ToList();
                subs = temp[0].Split('#');
                T_par_1.Text = subs[0];
                T_par_2.Text = subs[1];
                T_par_3.Text = subs[2];
                T_par_4.Text = subs[3];
                T_par_tok.Text = subs[4];
                T_par_proc.Text = subs[5];
                for (int i = 1; i < temp.Count; i++)
                {
                    subs = temp[i].Split('#');
                    C_unit_value.Series[0].Values.Add(Convert.ToSingle( subs[0]));
                    C_unit_value.Series[1].Values.Add(Convert.ToSingle(subs[1]));
                    C_unit_value.Series[2].Values.Add(Convert.ToSingle(subs[2]));
                    C_unit_value.Series[3].Values.Add(Convert.ToSingle(subs[3]));
                    C_unit_value2.Series[0].Values.Add(Convert.ToSingle(subs[4]));
                    C_unit_value2.Series[1].Values.Add(Convert.ToSingle(subs[5]));
                    Labels.Add(subs[6]);
                }
            }
        }
        // запись графика в фаил 
        private void B_write_file_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Data (*.dat)|*.dat";
            saveFileDialog.FileName = "Graphic(" + DateTime.Now.ToShortDateString() + ")";
            if (saveFileDialog.ShowDialog() == true)
            {
                List<string> temp = new List<string>();
                temp.Add(T_par_1.Text + "#" +
                         T_par_2.Text + "#" +
                         T_par_3.Text + "#" +
                         T_par_4.Text + "#"+
                         T_par_tok.Text + "#"+
                         T_par_proc.Text);
                for (int i = 0; i < C_unit_value.Series[0].Values.Count; i++)
                {
                    temp.Add(C_unit_value.Series[0].Values[i].ToString() + "#" +
                             C_unit_value.Series[1].Values[i].ToString() + "#" +
                             C_unit_value.Series[2].Values[i].ToString() + "#" +
                             C_unit_value.Series[3].Values[i].ToString() + "#" +
                             C_unit_value2.Series[0].Values[i].ToString() + "#" +
                             C_unit_value2.Series[1].Values[i].ToString() + "#" +
                             Labels[i]);
                }
                File.WriteAllLines(saveFileDialog.FileName, temp.ToArray());
            }
        }

        private void B_ClosePort_Click(object sender, RoutedEventArgs e)
        {
            HART_conection.close(); // закрывем порт 
            usb_stats.Content = "Disconect";
        }
        private void B_OpenPort_Click(object sender, RoutedEventArgs e)
        {
            this_usb = ComboBox_UsbDevaise.SelectedItem.ToString();// обновляем текуше выбраный порт
            HART_conection.close(); // закрывем предыдущий порт 
            Load_propertis(); // подгружаем настройки 
            string conect_usb_staite = HART_conection.init(this_usb);//иницеализируем наш порт
            if (conect_usb_staite == "True")// проверяем статус 
            {
                usb_stats.Content = "Conect";
            }
            else
            {
                usb_stats.Content = "Disconect";
            }
        }
    }
}
