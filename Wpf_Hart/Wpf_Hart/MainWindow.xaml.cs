﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
using System.Windows.Interop;
using System.Collections.ObjectModel;
using System.IO.Ports;
using Class_HART;
using System.Management;
using System.Threading;


namespace Wpf_Hart
{
  
    public class MarginConverter : IValueConverter
    {

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Thickness(System.Convert.ToDouble(value),40, 0, 0);
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
        public ObservableCollection<string> usb { get; set; } = new ObservableCollection<string> { };
        private const int WM_DEVICECHANGE = 0x0219;  // int = 537
        private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00000004;

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
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
            }catch{ }

            ports = SerialPort.GetPortNames();
            return ports;
            
          

        }
        private void ReadDongleHeader() // срабатывает при подключении\отключении usb
        {




            //Thread.Sleep(1000);

            // string[] ports = SerialPort.GetPortNames();
           string[] ports = GetSerialPort();
          //  usb = GetCOMPortsInfo();
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
            InitializeComponent();
            List_menu.SelectedIndex = 0; // устанавливаем по умолчанию выбраный первый элемент меню
            Tab_control_main.SelectedIndex = 0;// устанавливаем по умолчанию первую панель 

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
                    string conect_usb_staite = HART_conection.init(this_usb);
                    if (conect_usb_staite == "True")
                    {
                        usb_stats.Content = "Conect " + this_usb;
                    }
                    else
                    {
                        usb_stats.Content = "false";
                    }
                }
            }
        }
    }
}
