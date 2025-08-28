using BluetoothLE_App.Models;
using BluetoothLE_App.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.ViewModel
{
    internal class NavigationViewModel : BaseNavigationViewModel
    {
        public NavigationViewModel()
        {
            Items = new List<NavigationButtonItem>()
        {
            new("Server",nameof(HostPage)),
            new("Client",nameof(ClientPage))
        };
        }
    }
}