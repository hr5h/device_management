using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Essentials;
using Plugin.BLE.Abstractions;

namespace DeviceManagement
{
    public partial class MainPage : ContentPage
    {
        private IBluetoothLE bluetooth;
        private IAdapter bluetoothAdapter;
        private List<IDevice> deviceList = new List<IDevice>();
        private List<IDevice> deviceFilterList = new List<IDevice>();

        public MainPage()
        {
            InitializeComponent();
            bluetooth = CrossBluetoothLE.Current;
            bluetoothAdapter = bluetooth.Adapter;
            bluetoothAdapter.DeviceDiscovered += (s, a) =>
            {
                if (a.Device != null && !string.IsNullOrEmpty(a.Device.Name))
                {
                    deviceList.Add(a.Device);
                }
            };
        }

        private async Task<bool> PermissionsGrantedAsync()
        {
            var locationPermissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (locationPermissionStatus != PermissionStatus.Granted)
            {
                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                Console.WriteLine("---");
                Console.WriteLine(status);
                Console.WriteLine("---");
                return status == PermissionStatus.Granted;
            }
            return true;
        }

        private async void ScanButton_Clicked(object sender, EventArgs e)
        {
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanButton.IsEnabled = false);
            ScanCheckBox.IsEnabled = false;
            DevicesListView.ItemsSource = null;

            var state = bluetooth.State;

            if (state == BluetoothState.Off)
            {
                IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanButton.IsEnabled = true);
                await DisplayAlert("Выключен Bluetooth", "Приложению требуется включенный Bluetooth", "OK");
                return;
            }

            if (!await PermissionsGrantedAsync())
            {
                IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanButton.IsEnabled = true);
                await DisplayAlert("Требуется разрешение", "Приложению требуется разрешение на определение местоположения", "OK");
                return;
            }

            deviceList.Clear();
            deviceFilterList.Clear();

            foreach (var device in bluetoothAdapter.ConnectedDevices)
                deviceList.Add(device);

            await bluetoothAdapter.StartScanningForDevicesAsync();

            if (ScanCheckBox.IsChecked)
            {
                foreach (var d in deviceList.ToList())
                {
                    if (DeviceNames.DeviceNamesList.Contains(d.Name))
                    {
                        deviceFilterList.Add(d);
                    }
                }
                DevicesListView.ItemsSource = deviceFilterList;
            }
            else
            {
                DevicesListView.ItemsSource = deviceList;
            }

            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanButton.IsEnabled = true);
            ScanCheckBox.IsEnabled = true;
        }

        private async void DevicesListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanButton.IsEnabled = false);
            IDevice selectedItem = e.Item as IDevice;

            if (selectedItem.State == DeviceState.Connected)
            {
                await Navigation.PushAsync(new BluetoothPage(selectedItem));
            }
            else
            {
                try
                {
                    var connectParameters = new ConnectParameters(false, true);
                    await bluetoothAdapter.ConnectToDeviceAsync(selectedItem, connectParameters);
                    await Navigation.PushAsync(new BluetoothPage(selectedItem));
                }
                catch
                {
                    await DisplayAlert("Error connecting", $"Error connecting to BLE device: {selectedItem.Name ?? "N/A"}", "Retry");
                }
            }
            DevicesListView.SelectedItem = null;
            //IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanButton.IsEnabled = true);
        }
    }
}
