using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceManagement
{
    public class GattIdentifiers
    {
        public static Guid UartGattServiceId = Guid.Parse("0000FFE0-0000-1000-8000-00805F9B34FB");
        public static Guid UartGattCharacteristicSendId = Guid.Parse("0000FFE1-0000-1000-8000-00805F9B34FB");
        public static Guid UartGattCharacteristicReceiveId = Guid.Parse("0000FFE1-0000-1000-8000-00805F9B34FB");

        public static Guid SpecialNotificationDescriptorId = Guid.Parse("00002902-0000-1000-8000-00805f9b34fb");
    }
}
