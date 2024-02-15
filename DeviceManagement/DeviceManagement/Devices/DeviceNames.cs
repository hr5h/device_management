using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DeviceManagement.Devices
{
    public class DeviceNames
    {
        //public static List<string> DeviceNamesList = new List<string>() { "BT05", "IPS_1" }; // Известные устройства
        public static Dictionary<string, string> DeviceNamesList = new Dictionary<string, string>()
        {
            ["BT05"] = "BT05",
            ["IPS_1"] = "ИПС-1"
        };
        public static List<DeviceInfo> DeviceInfoList = new List<DeviceInfo>() // Информация об устройствах
        { 
            new DeviceInfo() { Image = "npodiod.png", Name = "НПО ДиОД", Url = "https://npo-diod.com/" },
            new DeviceInfo() { Image = "IPS_11.png", Name = "ИПС-1", Url = "https://npo-diod.com/product/izmeritel-poverkhnostnogo-soprotivleniya-ips-1/#desc" }, 
            new DeviceInfo() { Image = "BT051.png", Name = "BT05", Url = "https://microkontroller.ru/esp32-projects/ispolzovanie-bluetooth-v-module-esp32/" } 
        };

        public class DeviceInfo
        {
            public string Image { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
            public ICommand DeviceLinkCommand => new Command(DeviceLink);

            private async void DeviceLink()
            {
                string url = Url;
                await Browser.OpenAsync(new Uri(url));
            }
        }

        public class IPS_1Data
        {
            public DateTime Date { get; set; }
            public TimeSpan Time { get; set; }
            public string Name { get; set; }
            public int Voltage { get; set; }
            public int Temperature { get; set; }
            public int Humidity { get; set; }
            public int Resistor { get; set; }
        }

    }
}
