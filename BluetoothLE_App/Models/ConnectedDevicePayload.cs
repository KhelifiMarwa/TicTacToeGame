using BluetoothLE_App.Abstractions;
using BluetoothLE_App.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.Models
{
    internal class ConnectedDevicePayload : BasePagePayload
    {
        public string DeviceIdentifier { get; set; } = string.Empty;

        public TicTacToeState State { get; set; }

        public string CharacteristicUuid { get; set; }
    }
}
