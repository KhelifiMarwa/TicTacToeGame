using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.Enums
{
    internal enum WinnerPositionState
    {
        None = 0,
        FirstRow,
        SecondRow,
        ThirdRow,
        FirstColumn,
        SecondColumn,
        ThirdColumn,
        UpToDownDiagonal,
        DownToUpDiagonal
    }
}
