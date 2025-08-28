using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.Helpers
{
    public static class Constants
    {
        public static class Texts
        {
            public const string ConnectionServiceUuid = "6e41cfd8-4c08-4197-a93e-1c84b1a847de";
            public const string DataTransferServiceUuid = "fa242add-b5ea-41be-aa9a-cc3e2fb9b8f3";
            public const string WriteClientNameCharacteristicUuid = "e8a2539e-16fe-452a-91dd-a986235bd4cc";

            public const string Draw = "Draw - nobody wins, everybody loses";
            public const string ServerMessage = "Is connected device:";
            public const string OpponentLeft = "Your opponent left";
            public const string GameOverPopupTitle = "Game Over";
            public const string HighScoreLabel = "Result: ";
            public const string Await = "Await connection";
            public const string ServerTitle = "Server";
            public const string ScanTitle = "Scan";
            public const string MainMenu = "Exit";
            public const string Retry = "Retry";
            public const string Move = " move";
            public const string Turn = "Turn:";
        }
    }
}