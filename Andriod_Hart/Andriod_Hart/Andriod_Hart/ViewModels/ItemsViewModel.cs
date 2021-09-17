using Andriod_Hart.Models;
using Andriod_Hart.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Andriod_Hart.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private Item _selectedItem;
    

        public ObservableCollection<Item> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Item> ItemTapped { get; }

        public ItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Item>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem); 
            
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;
          
            try
            {
                Items.Clear();
                
                
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
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
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        async void OnItemSelected(Item item)
        {
            if (item == null)
                return;

            var connectedDevice = await adapter.ConnectToKnownDeviceAsync(Guid.Parse(item.Description));
            var service = await connectedDevice.GetServiceAsync(Guid.Parse("ffe0ecd2-3d16-4f8d-90de-e89e7fc396a5"));
            var characteristic = await service.GetCharacteristicAsync(Guid.Parse("d8de624e-140f-4a22-8594-e2216b84a5f2"));
            await characteristic.WriteAsync(new byte[] { 0xFF, 0xFF, 0xFF});
            // This will push the ItemDetailPage onto the navigation stack
            // await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }
    }
}