using BluetoothLE_App.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.Helpers
{
    public class DialogReturnValue
    {
        public readonly DialogReturnStatuses Status;

        public DialogReturnValue(DialogReturnStatuses status)
        {
            this.Status = status;
        }
    }
}
