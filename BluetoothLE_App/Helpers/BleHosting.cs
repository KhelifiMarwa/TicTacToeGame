using Shiny.BluetoothLE.Hosting.Managed;
using Shiny.BluetoothLE.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.Helpers
{
    [BleGattCharacteristic("9cc82e82-d889-4f29-bdba-124d5ee2b0d0", "9cc82e82-d889-4f29-bdba-124d5ee2b0d1")]
    public class MyManagedCharacteristics : BleGattCharacteristic;


    [BleGattCharacteristic("9cc82e82-d889-4f29-bdba-124d5ee2b0d0", "9cc82e82-d889-4f29-bdba-124d5ee2b0d1")]
    public class MyManagedRequestCharacteristic : BleGattCharacteristic
    {
        public override Task<GattResult> Request(WriteRequest request)
        {
            return Task.FromResult(GattResult.Success(new byte[] { 0x01 }));
        }
    }
}
