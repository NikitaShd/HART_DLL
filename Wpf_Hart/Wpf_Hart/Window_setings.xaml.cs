using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

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
         = new ObservableCollection<string> {"200 ms" ,"400 ms","700 ms","1000 ms","1500 ms","2000 ms","3000 ms","4000 ms" };
        public ObservableCollection<string> taumautadd_s { get; set; }
         = new ObservableCollection<string> { "1 ms", "2 ms", "5 ms", "10 ms" };

        public ObservableCollection<int> Preambula { get; set; }
         = new ObservableCollection<int> { 5,6,7,8,9,10,11,12,13,14,15};
        public Window_setings()
        {
            this.DataContext = this;
            InitializeComponent();
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

       
    }
}
