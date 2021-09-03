using System;
using System.Collections.Generic;
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

namespace Wpf_Hart.Devise
{
    /// <summary>
    /// Логика взаимодействия для P_MTM701_7.xaml
    /// </summary>
    public partial class P_MTM701_7 : Page
    {
        MainWindow window;
        public P_MTM701_7()
        {
            InitializeComponent();
            window = (MainWindow)Application.Current.MainWindow;
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
    }
}
