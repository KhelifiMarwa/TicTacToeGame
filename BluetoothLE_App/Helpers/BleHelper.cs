using Shiny.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.Helpers
{
    internal static class BleHelper
    {
        public static async Task<BleCharacteristicInfo?> GetCharacteristic(IPeripheral peripheral, string serviceUuid,
            string characteristicUuid)
        {
            var services = await peripheral
                .WithConnectIf()
                .Select(x => x.GetServices())
                .Switch();

            var result = await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var service = services.ToList().Find(x => x.Uuid == serviceUuid);
                if (service?.Uuid != null)
                {
                    var characteristic = (await peripheral.GetCharacteristicsAsync(service.Uuid)).ToList()
                        .Find(x => x.Uuid == characteristicUuid);
                    return characteristic;
                }

                return null;
            });

            return result;
        }

        public static async Task<BleCharacteristicInfo?> GetFirstCharacteristic(IPeripheral peripheral, string serviceUuid)
        {
            var services = await peripheral
                .WithConnectIf()
                .Select(x => x.GetServices())
                .Switch();

            var result = await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var service = services.ToList().Find(x => x.Uuid == serviceUuid);
                if (service?.Uuid != null)
                {
                    var characteristic = (await peripheral.GetCharacteristicsAsync(service.Uuid)).ToList()
                        .First();
                    return characteristic;
                }

                return null;
            });

            return result;
        }
    }
}