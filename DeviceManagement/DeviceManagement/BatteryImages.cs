using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceManagement
{
    public class BatteryImages
    {
        public enum BatteryCharge
        {
            power_low, //0-1
            power, // 1-2
            power_full // 2-3
            /*
            empty_battery, //0-1
            low_battery, //1-2
            half_battery, //2-3
            battery, //3-4
            full_battery //4-5
            */
        };
    }
}
