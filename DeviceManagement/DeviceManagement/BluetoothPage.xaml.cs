using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Essentials;
using Plugin.BLE;
using System.Windows.Input;

namespace DeviceManagement
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothPage : ContentPage
    {
        private readonly IDevice connectedDevice;
        //private ICharacteristic sendCharacteristic;
        //private ICharacteristic receiveCharacteristic;

        public BluetoothPage(IDevice _connectedDevice)
        {
            InitializeComponent();
            connectedDevice = _connectedDevice;
            Title = "Подключено " + connectedDevice.Name;
        }
    }
}