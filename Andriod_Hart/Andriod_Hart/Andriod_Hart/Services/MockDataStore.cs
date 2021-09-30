using Andriod_Hart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.OS;

using Android.Bluetooth;
using Android.Content;
using Android.Support.V4.Content;

namespace Andriod_Hart.Services
{
    
public class MockDataStore : IDataStore<Item>
    {
        readonly static List<Item> items = new List<Item>();
        readonly static List<Item> items_find = new List<Item>();

        BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
       
        public MockDataStore()
        {
            
            if (bluetoothAdapter == null)
            {
                // Device does not support Bluetooth
            }
            else
            {
                if (bluetoothAdapter.IsEnabled)
                {

                    IntentFilter filter = new IntentFilter(BluetoothDevice.ActionFound);
                    // наш ранее описанный ресивер, для поиска по указанному фильтру
                    BluetoothDeviceReceiver receiver = new BluetoothDeviceReceiver();
                    // зарегистрируем трансляцию(ресивер) и фильтр для обнаружения устройств
                   // LocalBroadcastManager.GetInstance(Android.App.Application.Context).RegisterReceiver(receiver, filter);
                    Android.App.Application.Context.RegisterReceiver(receiver, filter);
                   
                }
            }
        }
        class BluetoothDeviceReceiver : BroadcastReceiver
        {
            // перегрузим метод `OnReceive`
            public bool contain(List<Item> items,Item temp)
            {
                foreach (Item item in items)
                {
                    if (item.Description == temp.Description) return true;
                }
                return false;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                // это действие, которое будем выполнять (для нас пока будет только поиск)
                String action = intent.Action;

                if (action == BluetoothDevice.ActionFound)
                {
                    // Получить устройство
                    BluetoothDevice newDevice = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);

                    // если устройство не сопряжено (его еще не было в BondedDevices)
                    if ((newDevice.BondState != Bond.Bonded))
                    {
                        Item temp = new Item();
                        if (newDevice.Name != null) temp.Text = newDevice.Name; else temp.Text = "{null}";
                        temp.Description = newDevice.Address;
                        temp.IsConected = false;
                        if (!contain(items_find,temp)) { 
                            items_find.Add(temp); 
                        } // добавление имени

                    }
                }
             
                    // далее можно описать и другое действие, к примеру остановка поиска
                    // это будет action = BluetoothAdapter.ActionDiscoveryFinished
            }
        }
        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);
            
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Description == item.Description).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);
            
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Description == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Description == id));
        }
        public async Task<IEnumerable<Item>> GetItemsFingAsync(bool forceRefresh = false)
        {
            items_find.Clear();
            bluetoothAdapter.StartDiscovery();
            await Task.Delay(10000);
            bluetoothAdapter.CancelDiscovery();
            return await Task.FromResult(items_find);
        }
        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            items.Clear();
            ICollection<BluetoothDevice> devices = bluetoothAdapter.BondedDevices;

            // пройдем по списку устройств и будем заполнять список имен
            foreach (var device in devices)
            {
                // device - сопряженное устройство из BondedDevices
                Item temp = new Item();
                temp.Text = device.Name;
                temp.Description = device.Address;
                temp.IsConected = false;
                items.Add(temp); // добавление имени
            }
            
         
            return await Task.FromResult(items);
        }
    }
}