using Andriod_Hart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Java.Lang;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;
namespace Andriod_Hart.Services
{
    
public class MockDataStore : IDataStore<Item>
    {
        readonly List<Item> items = new List<Item>();
        IBluetoothLE ble;
        IAdapter adapter;

        public MockDataStore()
        {
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            adapter.ScanMode = ScanMode.Balanced;
            adapter.DeviceDiscovered += async (s, a) => {
                var serv = await a.Device.GetServicesAsync();
                Item temp = new Item();
                temp.Id = Guid.NewGuid().ToString();
                if(a.Device.Name == null) { temp.Text = a.Device.NativeDevice.ToString(); } else { temp.Text = a.Device.Name; }
               
                temp.Description = a.Device.Id.ToString();
                items.Add(temp);
            };
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);
            
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);
            
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            items.Clear();
            await adapter.StartScanningForDevicesAsync();
            return await Task.FromResult(items);
        }
    }
}