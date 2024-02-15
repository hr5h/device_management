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
using DeviceManagement.Devices;
using Xamarin.Forms.Internals;
using static DeviceManagement.IPS_1;
using System.Threading;
using System.Windows.Input;

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
            CarouselView.ItemsSource = DeviceNames.DeviceInfoList;
            bluetooth = CrossBluetoothLE.Current;
            bluetoothAdapter = bluetooth.Adapter;
            bluetoothAdapter.ScanTimeout = 5000;
            bluetoothAdapter.DeviceDiscovered += (s, a) =>
            {
                if (a.Device != null && !string.IsNullOrEmpty(a.Device.Name))
                {
                    deviceList.Add(a.Device);
                }
            };
            RepeatFunction();
            Console.WriteLine("--------");
            Console.WriteLine(DeviceDisplay.MainDisplayInfo);
            Console.WriteLine(DeviceDisplay.MainDisplayInfo.Width);
            Console.WriteLine(DeviceDisplay.MainDisplayInfo.Height);
            Console.WriteLine("--------");
        }

        private async void RepeatFunction()
        {
            while (true)
            {
                int currentPosition = CarouselView.Position;
                currentPosition++;
                if (currentPosition == 3) { currentPosition = 0; }
                CarouselView.ScrollTo(currentPosition);
                await Task.Delay(4000);
            }
        }

        private async Task<bool> PermissionsGrantedAsync()
        {
            var locationPermissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (locationPermissionStatus != PermissionStatus.Granted)
            {
                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                return status == PermissionStatus.Granted;
            }
            return true;
        }

        private async void ScanButton_Clicked(object sender, EventArgs e)
        {
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanButton.IsEnabled = false);
            HistoryButton.IsEnabled = false;
            //ScanCheckBox.IsEnabled = false;

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
            Grid.Children.Clear();
            Grid.RowDefinitions.Clear();
            Grid.ColumnDefinitions.Clear();
            Frame.IsVisible = false;

            foreach (var device in bluetoothAdapter.ConnectedDevices)
                deviceList.Add(device);

            await bluetoothAdapter.StartScanningForDevicesAsync();

            GridCarouselView.IsVisible = false;

            Frame.IsVisible = true;
            //CreateGrid(ScanCheckBox.IsChecked);
            CreateGrid(true);

            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanButton.IsEnabled = true);
            HistoryButton.IsEnabled = true;
            //ScanCheckBox.IsEnabled = true;
        }

        private List<IDevice> ListDevices(bool condition)
        {
            if (!condition) return deviceList;
            else return deviceFilterList;
        }

        private void CreateGrid(bool checkBox)
        {
            int rowIndex = -1;
            int columnIndex = 1;

            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 172.5 });
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 172.5 });

            if (checkBox)
            {
                foreach (var d in deviceList.ToList())
                {
                    if (DeviceNames.DeviceNamesList.ContainsKey(d.Name))
                    {
                        deviceFilterList.Add(d);
                    }
                }
            }

            foreach (var device in ListDevices(checkBox))
            {
                if (columnIndex == 0)
                {
                    columnIndex++;
                }
                else
                {
                    Grid.RowDefinitions.Add(new RowDefinition() { Height = 230 });
                    columnIndex--;
                    rowIndex++;
                }
                Frame frame = new Frame()
                {
                    WidthRequest = 172.5,
                    HeightRequest = 230,
                    //BorderColor = Color.FromRgb(0, 127, 255),
                    CornerRadius = 30,
                    HasShadow = false,
                };
                Image image = new Image();
                image.HeightRequest = 160;
                if (DeviceNames.DeviceNamesList.ContainsKey(device.Name))
                {
                    image.Source = device.Name + "1.png";
                }
                else
                {
                    image.Source = "noimg2.png";
                }
                //image.Source = "IPS_1.png";
                Label label = new Label()
                {
                    Text = DeviceNames.DeviceNamesList[device.Name],
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontSize = 14,
                    TextColor = Color.Black
                };
                StackLayout stackLayout = new StackLayout();
                stackLayout.Children.Add(image);
                stackLayout.Children.Add(label);
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, ev) => {
                    DevicesListView_ItemTapped(s, ev, device);
                };
                frame.GestureRecognizers.Add(tapGestureRecognizer);
                frame.Content = stackLayout;
                Grid.Children.Add(frame, columnIndex, rowIndex);
            }

            if (Grid.Children.Count() == 0)
            {
                Grid.RowDefinitions.Clear();
                Grid.ColumnDefinitions.Clear();
                Label label = new Label()
                {
                    Text = "Устройства не найдены",
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontSize = 26,
                    TextColor = Color.Black
                };
                Grid.Children.Add(label);
            }
        }

        private async void DevicesListView_ItemTapped(object sender, EventArgs e, IDevice device)
        {
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanButton.IsEnabled = false);
            //ScanCheckBox.IsEnabled = false;
            Frame frame = (sender as Frame);
            frame.BackgroundColor = Color.FromRgb(204, 213, 244);

            IDevice selectedItem = device;

            if (selectedItem.State == DeviceState.Connected)
            {
                NavigationPage(selectedItem);
                //frame.BackgroundColor = Color.White;
            }
            else
            {
                try
                {
                    var connectParameters = new ConnectParameters(false, true);
                    await bluetoothAdapter.ConnectToDeviceAsync(selectedItem, connectParameters);
                    NavigationPage(selectedItem);
                    //frame.BackgroundColor = Color.White;
                }
                catch
                {
                    await DisplayAlert("Ошибка подключения", $"Ошибка подключения к: {selectedItem.Name ?? "N/A"}", "ОК");
                }
            }
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanButton.IsEnabled = true);
            //ScanCheckBox.IsEnabled = true;
        }

        private async void NavigationPage(IDevice selectedItem)
        {
            switch (selectedItem.Name)
            {
                case "BT05":
                    await Navigation.PushAsync(new BT05(selectedItem));
                    break;
                case "IPS_1":
                    await Navigation.PushAsync(new IPS_1(selectedItem));
                    break;
                default:
                    await Navigation.PushAsync(new BluetoothPage(selectedItem));
                    break;
            }
        }

        private async void TestButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TestPage());
        }

        private void ThemeSwitch_Toggled(object sender, ToggledEventArgs e)
        {

            /*
            if (ThemeSwitch.IsToggled)
            {
                BackgroundColor = Color.Black;
                HeaderLabel.TextColor = Color.White;
                //IndicatorView.SelectedIndicatorColor = Color.White;
                ThemeSwitch.ThumbColor = Color.White;
            }
            else
            {
                //BackgroundColor = Color.FromRgb(245, 245, 245);
                BackgroundColor = Color.White;
                HeaderLabel.TextColor = Color.Black;
                //IndicatorView.SelectedIndicatorColor = Color.Black;
                ThemeSwitch.ThumbColor = Color.Black;
            }
            */
        }

        private async void HistoryButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HistoryPage());
        }
    }
}
