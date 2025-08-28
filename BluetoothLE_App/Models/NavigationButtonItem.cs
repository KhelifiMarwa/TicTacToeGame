using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.Models
{
    public class NavigationButtonItem
    {
        public NavigationButtonItem()
        {
        }

        [SetsRequiredMembers]
        public NavigationButtonItem(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public required string Name { get; init; }

        public required string Path { get; init; }

    }
}
