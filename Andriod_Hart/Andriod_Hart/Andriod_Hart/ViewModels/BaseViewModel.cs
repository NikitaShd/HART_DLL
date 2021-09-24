using Andriod_Hart.Models;
using Andriod_Hart.Services;
using Android.Bluetooth;
using Android.Content;
using Java.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Class_HART;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Forms.Platform.Android;

namespace Andriod_Hart.ViewModels
{
  
    public class BaseViewModel : INotifyPropertyChanged
    {
       

        public class _Conect : Class_HART.Conect
        {
              BluetoothSocket input;
             public _Conect(BluetoothSocket temp)
             {
                input = temp;
             }
            static int T = 0;
            static Byte[] temp2 = { };
            static bool tread = false;
            public override Byte[][] Write(Byte[] a)
            {
                 try
                 {
                    input.InputStream.Flush();
                 }
                 catch
                 {
                     Byte[][] read2 = { };

                     return read2;
                 }
                 //port.OutputStream.Flush();
                 Task.Delay(150);
                input.OutputStream.Write(a, 0, a.Length);


                int i = 0;
                Array.Resize(ref temp2,0);
                Byte[] temp = { };
                Byte[][] read = { };
                 T = (int)write_taim;
                 int FF_Start = 0;
                 bool F = false;
                 bool F_ = true;
                if (tread == false)
                {
                    Task.Run(() =>
                    {
                        tread = true;
                        try
                        {
                            while (true)
                            {

                                Byte b = (byte)input.InputStream.ReadByte();
                                Array.Resize(ref temp2, temp2.Length + 1);
                                temp2[temp2.Length - 1] = b;
                                T += (int)(write_taimout);

                            }
                        }
                        catch
                        {
                            tread = false;
                        }
                    });
                }

                 Task test = Task.Factory.StartNew(async () =>
                 {
                     int l = 0;
                     while (l < T)
                     {
                         l++;
                         await Task.Delay(1);
                     }
                     // port.InputStream.Close();
                     F_ = false;
                   
                     
                     T = 1;
                     
                 });

                 while (F_)
                 {

                 }
                 temp = temp2;

                 int tmp = 0;
                 int size = 0;
                 for (int j = 1; j < temp.Length; j++) // я уже и сам не нимаю что тут происходит 
                 {
                     // try { if ((temp[j + 1] == 0xFF) && (temp[j] == 0xFF) && (temp[j - 1] != 0xFF)) { F = false; } }
                     // catch { }
                     try
                     {
                         if (((temp[j] == 0x06) || (temp[j] == 0x86)) && (temp[j - 1] == 0xFF) && (temp[j - 2] == 0xFF))
                         {
                             FF_Start++;
                             F = true;
                             tmp = 0;
                             Array.Resize(ref read, read.Length + 1);
                             read[read.Length - 1] = new Byte[] { };

                         }
                     }
                     catch { }
                     if (F == true)
                     {
                         Array.Resize(ref read[read.Length - 1], read[read.Length - 1].Length + 1);
                         read[read.Length - 1][read[read.Length - 1].Length - 1] = temp[j];

                         if ((temp[j] == 0x06) && (size == 0))
                         {
                             size = 4;
                         }
                         else if ((temp[j] == 0x86) && (size == 0))
                         {
                             size = 8;
                         }

                         if (((tmp == 3) && (size == 4)) || ((tmp == 7) && (size == 8)))
                         {
                             size += temp[j];
                         }

                         if (size == tmp)
                         {
                             F = false; tmp = 1;//// не трогать ! 
                             size = 0;

                         }
                         tmp++;


                     }
                 }


                 return read;


             }
         }
      
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        bool isBusy = false;
       
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
        bool visable = false;
        public bool Visable
        {
            get { return visable; }
            set { SetProperty(ref visable, value); }
        }
        bool visable_Bluet = true;
        public bool Visable_Bluet
        {
            get { return visable_Bluet; }
            set { SetProperty(ref visable_Bluet, value); }
        }
        bool visable_DEV = false;
        public bool Visable_DEV
        {
            get { return visable_Bluet; }
            set { SetProperty(ref visable_Bluet, value); }
        }
        public bool Visable_DEV_rev
        {
            get { return !visable_Bluet; } 
        }
        public BluetoothAdapter bluetoothAdapter { get; } = BluetoothAdapter.DefaultAdapter;
        public static UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        public static int Master_ID = 0;
        public static BluetoothSocket btSocket = null;
        public static _Conect Hart_conection ;
       
        public object balanceLock { get; } = new object();
        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
