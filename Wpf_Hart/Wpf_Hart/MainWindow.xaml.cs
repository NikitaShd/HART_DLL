using Class_HART;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;

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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Conect HART_conection = new Conect();
        string this_usb = "";
        Byte[] Devise_long_adres = { };
        public ObservableCollection<string> usb { get; set; } = new ObservableCollection<string> { };
        
        public struct devaise
        {

            public string Short_Address { get; set; }
            public string Long_Address { get; set; }
            public string Device_Version { get; set; }
            public string Software_Version { get; set; }
            public string Assembly_Number { get; set; }

        }



        public ObservableCollection<devaise> HART_dev_list { get; set; } = new ObservableCollection<devaise> { };
        private const int WM_DEVICECHANGE = 0x0219;  // int = 537
        private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00000004;

        
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



        public string[] GetSerialPort()
        {
            string[] ports = SerialPort.GetPortNames();
            try
            {
                // ManagementClass mc = new ManagementClass("WIN32_SerialPort");
                //ManagementObjectCollection moc = mc.GetInstances();
                // foreach (ManagementObject mo in moc)
                //если порт был открыт а мы вытянули usb то функцыя GetPortNames дцблирует название потра 
                // поэтому мы получаем спсок всех портов и проверяем их зате снова вызываем GetPortNames
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
                usb.Add(port);
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

        public MainWindow()
        {
           
            this.DataContext = this;
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
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(Properties.Settings.Default.Langue);
            InitializeComponent();
            List_menu.SelectedIndex = 0; // устанавливаем по умолчанию выбраный первый элемент меню
            Tab_control_main.SelectedIndex = 0;// устанавливаем по умолчанию первую панель 
            ///  Properties.Settings.Default.Master;


            Load_propertis();
            ReadDongleHeader();



            // ========== нужно чтобы в редакторе вкладки отображались а в програме нет ==================
            Style s = new Style();
            s.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Collapsed));//убираем полосу вкладок в стиле
            Tab_control_main.ItemContainerStyle = s;// присваеваем стил нашей панельке 
            // ===========================================================================================
        }


        //изменение размена окна 
        private void stateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        //меняем местами кнопки выдвижного меню
        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }

        //меняем местами кнопки выдвижного меню
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }

        //диалог на закрытиие
        protected override void OnClosing(CancelEventArgs e)
        {
            var response = MessageBox.Show("Do you really want to exit?", "Exiting...",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (response == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                Application.Current.Shutdown();
            }

            base.OnClosing(e);
        }

        // закрыть окно
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //свернуть окно
        private void Minimaize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void List_menu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int item = List_menu.SelectedIndex;
            // List_menu.SelectedIndex = item;
            if (item != -1) Tab_control_main.SelectedIndex = item;

        }

        //перетаскивание формы
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ComboBox_UsbDevaise_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_UsbDevaise.SelectedItem != null)
            {
                if (this_usb != ComboBox_UsbDevaise.SelectedItem.ToString())
                {
                    this_usb = ComboBox_UsbDevaise.SelectedItem.ToString();
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
        }

        private void B_findDevaise_Click(object sender, RoutedEventArgs e)
        {


            if (HART_conection.scan == false)
            {
                Thread thread = new Thread(UpdateDevise);
                thread.Start();

                HART_conection.Scan_start(0);
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

            while (HART_conection.scan)
            {
                Thread.Sleep(500);

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    HART_dev_list.Clear();
                    this.Skan_progres.Value = HART_conection.scan_step;
                    foreach (Conect.Read_Fraim item in HART_conection.Net_config())
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
                        HART_dev_list.Add(temp);
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

        private void B_findDevaiseStop_Click(object sender, RoutedEventArgs e)
        {
            HART_conection.Scan_stop();
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

        private void Load_propertis()
        {
            HART_conection.Master = Properties.Settings.Default.Master;
            HART_conection.Spide = Properties.Settings.Default.Spide;
            HART_conection.preambula_leng = Properties.Settings.Default.preambula_leng;
            HART_conection.write_taimout = Properties.Settings.Default.write_taim;
            HART_conection.write_taim = Properties.Settings.Default.write_taimout;
        }
        private void Darkmode_Click(object sender, RoutedEventArgs e)
        {

            Set_them();

        }
        void Set_them()
        {
            ResourceDictionary dictionary = new ResourceDictionary();


            // Динамически меняем коллекцию MergedDictionaries
            // Application.Current.Resources.MergedDictionaries. = dictionary;
            if (Properties.Settings.Default.Darkmode)
            {
                var uri = new Uri("Dictionary_Lite.xaml", UriKind.Relative);
                dictionary = Application.LoadComponent(uri) as ResourceDictionary;
                Application.Current.Resources.Clear();
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(dictionary);
                Properties.Settings.Default.Darkmode = false;

            }
            else
            {
                var uri = new Uri("Dictionary_dark.xaml", UriKind.Relative);
                dictionary = Application.LoadComponent(uri) as ResourceDictionary;
                Application.Current.Resources.Clear();
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(dictionary);
                Properties.Settings.Default.Darkmode = true;
            }
            Properties.Settings.Default.Save();
        } // функцыя смены темы приложения 

        private void listBox_dev_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox_dev.SelectedItem != null)
            {
                devaise a = (devaise)listBox_dev.SelectedItem;
                Devise_long_adres = _Convert.GetBytes(a.Long_Address);
                Label_DevLongAdres.Text = "HART Device [ " + BitConverter.ToString(Devise_long_adres) +" ]";
            }
        }

        private async void B_Comand_0_Click(object sender, RoutedEventArgs e)
        {
            string[] temp = { };
            ((Button)sender).IsEnabled = false;
            await Task.Factory.StartNew(() =>
            {
                HART_conection.Comand_0(Properties.Settings.Default.Master,Devise_long_adres,ref temp);
            });
            ((Button)sender).IsEnabled = true;
            L_Manufacturer_Code.Content = Properties.Resource.R_Manufacturer_Code + temp[0];
            L_Device_Type_Code.Content = Properties.Resource.R_Device_Type_Code + temp[1];
            L_Preambul_leng.Content = Properties.Resource.R_Preambul_leng + temp[2];
            L_Universal_commands.Content = Properties.Resource.R_Universal_commands + temp[3];
            L_Specific_commands.Content = Properties.Resource.R_Specific_commands+ temp[4];
            L_Software_version.Content = Properties.Resource.R_Software_version + temp[5];
            L_Hardware_version.Content = Properties.Resource.R_Hardware_version+ temp[6];
            L_Device_function.Content = Properties.Resource.R_Device_function +  temp[8];
           
        }

        
    }
}
