using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceManagement
{
    public class GattIdentifiers
    {
        //ESP32
        public static Guid UartGattServiceId = Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b");
        public static Guid UartGattCharacteristicSendId = Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8");
        public static Guid UartGattCharacteristicReceiveId = Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8");
        //HM-10
        /*
        public static Guid UartGattServiceId = Guid.Parse("0000FFE0-0000-1000-8000-00805F9B34FB");
        public static Guid UartGattCharacteristicSendId = Guid.Parse("0000FFE1-0000-1000-8000-00805F9B34FB");
        public static Guid UartGattCharacteristicReceiveId = Guid.Parse("0000FFE1-0000-1000-8000-00805F9B34FB");
        */
        public static Guid SpecialNotificationDescriptorId = Guid.Parse("00002902-0000-1000-8000-00805f9b34fb");
    }
}
