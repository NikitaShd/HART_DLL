using Andriod_Hart.Models;
using Andriod_Hart.Views;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Andriod_Hart.ViewModels
{
    public class ItemsViewModel :  BaseViewModel
    {
        private Item _selectedItem;
    

        public ObservableCollection<Item> Items { get; }
        public ObservableCollection<Item> Items_fond { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Item> ItemTapped { get; }

        public bool item_blok { get; set; } = true;
        public ItemsViewModel()
        {
            Title = "Bluetooth";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            Items_fond = new ObservableCollection<Item>();

            IsBusy = true;
            ItemTapped = new Command<Item>(execute: (Item dEVISE) =>
            {
                OnItemSelected(dEVISE);
                ItemTapped.ChangeCanExecute();
            },
            canExecute: (Item dEVISE) => item_blok);
           
            AddItemCommand = new Command(OnAddItem); 
            
        }

        async Task ExecuteLoadItemsCommand()
        {
            Visable = true;
           
          
            try
            {
                Items.Clear();
                Items_fond.Clear();

                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
                List<Item> items_ = (List<Item>)await DataStore.GetItemsFingAsync(true);
                foreach (var item in items_)
                {
                    Items_fond.Add(item);
                }
            }
            catch (Exception ex)
            {
              //  Debug.WriteLine(ex);
            }
            finally
            {
                Visable = false;
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
           //   IsBusy = true;
          //  SelectedItem = null;
        }

        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private async void OnAddItem(object obj)
        {
            Visable = true;
            IsBusy = true;
            //await Shell.Current.GoToAsync(nameof(NewItemPage));
        }
      
        async void OnItemSelected(Item item)
        {
            if (item == null)
                return;
            bool action = await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Conect ?", item.Text , "Yes", "No");
            if (action)
            {
                BluetoothDevice device = bluetoothAdapter.GetRemoteDevice(item.Description);
                btSocket = device.CreateRfcommSocketToServiceRecord(MY_UUID);
                item_blok = false;
                ItemTapped.ChangeCanExecute();
                try
                {

                    btSocket.Connect();
                   
                    if (btSocket.IsConnected == false) btSocket.Connect();
                    //  await Task.Delay(10);
                }
                catch { Title = "Socket Error"; }
                finally
                {
                    if (btSocket.IsConnected)
                    {
                        Title = item.Text;
                    }
                    else
                    {
                        Title = "Not Conected";
                    }

                    Hart_conection = new _Conect(btSocket);
                }
                item_blok = true;
                ItemTapped.ChangeCanExecute();
            }
            // This will push the ItemDetailPage onto the navigation stack
            // await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }
    }
}