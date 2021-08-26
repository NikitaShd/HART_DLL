using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Wpf_Hart
{
    /// <summary>
    /// Логика взаимодействия для Window_setings.xaml
    /// </summary>
    public partial class Window_setings : Window
    {
        public ObservableCollection<int> port_spid { get; set; }
         = new ObservableCollection<int> { 2400, 4800, 9600, 19200, 38400, 57600, 115200 };
        public ObservableCollection<string> taumaut_s { get; set; }
         = new ObservableCollection<string> { "200 ms", "400 ms", "700 ms", "1000 ms", "1500 ms", "2000 ms", "3000 ms", "4000 ms" };
        public ObservableCollection<string> taumautadd_s { get; set; }
         = new ObservableCollection<string> { "1 ms", "2 ms", "5 ms", "10 ms" };

        public ObservableCollection<int> Preambula { get; set; }
         = new ObservableCollection<int> { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        public Window_setings()
        {
            this.DataContext = this;
            InitializeComponent();
            ComboBox_PortSpid.SelectedItem = Properties.Settings.Default.Spide;
            ComboBox_write_taimout.SelectedItem = Properties.Settings.Default.write_taimout + " ms";
            ComboBox_write_taim.SelectedItem = Properties.Settings.Default.write_taim + " ms";
            ComboBox_preambula_leng.SelectedItem = Properties.Settings.Default.preambula_leng;
            if( Properties.Settings.Default.Master == 0)
            {
                R_master1.IsChecked = true;
            }
            else
            {
                R_master2.IsChecked = true;
            }
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Spide = (int)ComboBox_PortSpid.SelectedItem;
            Properties.Settings.Default.write_taimout = Convert.ToInt32(ComboBox_write_taimout.SelectedItem.ToString().Replace(" ms", ""));
            Properties.Settings.Default.write_taim    = Convert.ToInt32(ComboBox_write_taim.SelectedItem.ToString().Replace(" ms", ""));
            Properties.Settings.Default.preambula_leng = (int)ComboBox_preambula_leng.SelectedItem;
            if (R_master1.IsChecked == true)
            {
                Properties.Settings.Default.Master = 0; 
            }
            else
            {
                Properties.Settings.Default.Master = 1;
            }
            Properties.Settings.Default.Save();
            this.DialogResult = true;
        }


    }
}
