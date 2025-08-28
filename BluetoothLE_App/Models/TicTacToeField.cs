using BluetoothLE_App.Enums;
using SkiaSharp.Views.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.Models
{
    public class TicTacToeField
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TicTacToeState State { get; set; }
        public SKCanvasView? Canvas { get; set; }
    }
}
