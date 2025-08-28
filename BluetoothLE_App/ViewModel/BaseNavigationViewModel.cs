using BluetoothLE_App.Models;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.ViewModel
{
    internal abstract class BaseNavigationViewModel : BaseViewModel
    {
        public List<NavigationButtonItem> Items { get; set; }


        public IAsyncRelayCommand<string?> GoToCommand { get; }

        protected BaseNavigationViewModel()
        {
            Items = new List<NavigationButtonItem>();
            GoToCommand = new AsyncRelayCommand<string?>(GoToPageAsync);
        }

        private static async Task GoToPageAsync(string? path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                await Shell.Current.GoToAsync($"{path}");
            }
        }
    }
}