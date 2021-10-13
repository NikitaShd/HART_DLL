using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Andriod_Hart.ViewModels
{
    public class SetingsModel : BaseViewModel
    {
        
        public ObservableCollection<string> Tamaut { get; set; } = new ObservableCollection<string> { "500", "700", "1000", "1200", "1800", "2200", "2800", "3600", "4100", "5000" };
        public ObservableCollection<string> Master { get; set; } = new ObservableCollection<string> { "0", "1" };
        public ObservableCollection<string> Leng { get; set; } = new ObservableCollection<string> {};
        int tamaut = 0;
        public int tamaut_int { get => tamaut; set { tamaut = value; OnPropertyChanged(); Preferences.Set(Name_Tameout, Tamaut[tamaut_int]); _set_change(); } }
        int master = 0;
        public int master_int { get => master; set { master = value; OnPropertyChanged(); Preferences.Set(Name_Master, Master[master_int]); _set_change(); } }
        int leng = 0;
        public int leng_int { get => leng; set { leng = value; OnPropertyChanged(); Preferences.Set(Name_PrLeng, Leng[leng_int]); _set_change(); } }
       public SetingsModel()
       {
            for (int i = 3; i <= 15; i++)
            {
                Leng.Add(i.ToString());
            }
            Title = "Setings";
            // int tmp = Preferences.Get(Name_Tameout, 700);
            //  tamaut = Tamaut.IndexOf(tmp.ToString());
            // leng = Leng.IndexOf(Preferences.Get(Name_PrLeng, 7).ToString());
            // master = Master.IndexOf(Preferences.Get(Name_Master, 0).ToString());
          
        }
        public void OnAppearing()
        {

            string temp = Preferences.Get(Name_Tameout, "700");
            tamaut_int = Tamaut.IndexOf(temp);
            temp = Preferences.Get(Name_PrLeng, "7");
            leng_int = Leng.IndexOf(temp);
            temp = Preferences.Get(Name_Master, "0");
            master_int = Master.IndexOf(temp);
        }
       void _set_change()
       {

            if (Hart_conection != null)
            {
                Hart_conection.write_taim = Convert.ToInt32(Preferences.Get(Name_Tameout, "700"));
                Hart_conection.preambula_leng = Convert.ToInt32(Preferences.Get(Name_PrLeng, "7"));
            }
            Master_ID = Convert.ToInt32(Preferences.Get(Name_Master, "0"));
        }
    }
}
