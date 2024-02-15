using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DeviceManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BT05 : ContentPage
    {

        private readonly IDevice connectedDevice;
        private ICharacteristic sendCharacteristic;
        private ICharacteristic receiveCharacteristic;
        public BT05(IDevice _connectedDevice)
        {
            InitializeComponent();
            connectedDevice = _connectedDevice;
            Title = "Подключено " + connectedDevice.Name;
            GattConnect();
        }

        private async void GattConnect()
        {
            try
            {
                var service = await connectedDevice.GetServiceAsync(GattIdentifiers.UartGattServiceId);
                if (service != null)
                {
                    sendCharacteristic = await service.GetCharacteristicAsync(GattIdentifiers.UartGattCharacteristicSendId);
                    receiveCharacteristic = await service.GetCharacteristicAsync(GattIdentifiers.UartGattCharacteristicReceiveId);
                    if (receiveCharacteristic != null)
                    {
                        receiveCharacteristic.ValueUpdated += (o, args) =>
                        {
                            var receivedBytes = args.Characteristic.Value;
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                Output.Text += Encoding.UTF8.GetString(receivedBytes, 0, receivedBytes.Length);
                            });
                        };
                        await receiveCharacteristic.StartUpdatesAsync();
                    }
                }
            }
            catch
            {
            }
        }

        /*
        private async void SendButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (sendCharacteristic != null)
                {
                    var bytes = await sendCharacteristic.WriteAsync(Encoding.ASCII.GetBytes($"{SendEntry.Text}"));
                    SendEntry.Text = "";
                }
            }
            catch
            {
                
            }
        }
        */

        private async void DiodOnButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (sendCharacteristic != null)
                {
                    var bytes = await sendCharacteristic.WriteAsync(Encoding.ASCII.GetBytes("1"));
                }
            }
            catch
            {

            }
        }

        private async void DiodOffButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (sendCharacteristic != null)
                {
                    var bytes = await sendCharacteristic.WriteAsync(Encoding.ASCII.GetBytes("0"));
                }
            }
            catch
            {

            }
        }
    }
}