using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public class View
        {
            public string Name { get; set; }
            public string Quantity { get; set; }
            public ObservableCollection<Item> items { get; set; }
        }

        public class Item
        {
            public string ItemName { get; set; }
            public double ItemQuantity { get; set; }
        }
        public struct Calib_tem{
            public float Temperature { get; set; }
            public string Kod_ACP_Temperature_Sensor { get; set; }
            public string Kod_ACP_Voltage_Sensor { get; set; }
            public string Kod_ACP_Сurrent_Sensor { get; set; }
            public Calib_dav[] _Davs { get; set; }
        }
       public struct Calib_dav
        {
            public float Pressure { get; set; }
            public string Kod_ACP_1 { get; set; }
            public string Kod_ACP_2 { get; set; }
            public string Kod_ACP_3 { get; set; }
            public string Kod_ACP_4 { get; set; }
        }
       
        public ObservableCollection<Calib_tem> Calib { get; set; } = new ObservableCollection<Calib_tem> { };
        MainWindow window;
        public ObservableCollection<View> myViews { get; set; } = new ObservableCollection<View> { };
        public P_MTM701_7()
        {
            for (int i = 0; i < 4; i++)
            {
                ObservableCollection<Item> temp = new ObservableCollection<Item>();
                for (int j = 0; j < 9; j++)
                {
                    Item temp2 = new Item();
                    temp2.ItemQuantity = j;
                    temp2.ItemName = "p1 " + i.ToString()+ " : "+ j.ToString();
                  
                    temp.Add(temp2);
                }
                View temp3 = new View();
                temp3.items = temp;
               
                temp3.Name = "t1  " + i.ToString();
                temp3.Quantity = "t2  " + i.ToString();
               
                myViews.Add(temp3);
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int kod = 0;
            float paran = 0;
            
            window.HART_conection.Comand_1(Properties.Settings.Default.Master, window.Devise_long_adres, ref kod, ref paran);
           // test.Text = kod.ToString() + " : " + paran.ToString();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            window.Frame_close();
            
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            T_contrast.Text = Convert.ToInt32(slider.Value).ToString();
        }

        private void B_dev_Contrast_read_Click(object sender, RoutedEventArgs e)
        {

        }

        private void B_dev_Contrast_write_Click(object sender, RoutedEventArgs e)
        {

        }

        private void B_dev_Table_read_Click(object sender, RoutedEventArgs e)
        {

        }

        private void B_dev_Table_write_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
