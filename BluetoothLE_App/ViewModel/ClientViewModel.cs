using BluetoothLE_App.Enums;
using BluetoothLE_App.Helpers;
using BluetoothLE_App.Models;
using BluetoothLE_App.Views;
using BluetoothLE_App.Views.Popups;
using CommunityToolkit.Mvvm.Input;
using Shiny.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static BluetoothLE_App.Helpers.Constants.Texts;
namespace BluetoothLE_App.ViewModel
{
    internal class ClientViewModel : BaseClientViewModel
    {
        private List<IPeripheral> _peripherals;

        public ObservableCollection<string> Peripherals { get; } = new();
        public ICommand ScanToggle { get; }

        public ICommand SelectCommand { get; }

        private bool _isScan;

        public bool IsScanning
        {
            get => _isScan;
            private set
            {
                _isScan = value;
                OnPropertyChanged();
            }
        }

        public ClientViewModel()
        {
            IsScanning = BleManager?.IsScanning ?? false;

            _peripherals = new List<IPeripheral>();

            ScanToggle = new Command(() => Task.Run(Scan));
            SelectCommand = new AsyncRelayCommand<string>(Select);
        }

        private async Task Scan()
        {
            if (BleManager == null)
            {
                await Shell.Current.CurrentPage.DisplayAlert("error", "Not supported", "ok");
                return;
            }

            if (IsScanning)
            {
                StopScan();
            }
            else
            {
                Peripherals.Clear();
                IsScanning = true;

                Scan(results =>
                {
                    foreach (var result in results)
                    {
                        if (!string.IsNullOrWhiteSpace(result.Peripheral.Name) &&
                            !Peripherals.Contains(result.Peripheral.Name))
                        {
                            _peripherals.Add(result.Peripheral);
                        }
                    }

                    if (_peripherals.Any())
                    {
                        foreach (var item in _peripherals)
                            if (!string.IsNullOrWhiteSpace(item.Name) &&
                                !Peripherals.Contains(item.Name))
                                Peripherals.Add(item.Name);
                    }
                });
            }
        }

        private async Task Select(string? name)
        {
            if (name == null) return;
            var popup = new BusyPopup();
            try
            {
                MainThread.InvokeOnMainThreadAsync(() => popup.ShowAsync());
                StopScan();

                var peripheral = _peripherals.FirstOrDefault(x => x.Name == name);
                var characteristic = await BleHelper.GetFirstCharacteristic(peripheral, ConnectionServiceUuid);
                var characteristicUuid = Guid.NewGuid().ToString();
                var data = Encoding.UTF8.GetBytes(characteristicUuid.Length.ToString());
                await peripheral.WriteCharacteristicAsync(characteristic, data);
                var count = 0;
                var parts = characteristicUuid.Split("-");
                var i = 0;
                while (count < characteristicUuid.Length)
                {
                    data = i < parts.Length - 1 ? Encoding.UTF8.GetBytes(parts[i] + '-') : Encoding.UTF8.GetBytes(parts[i]);

                    await peripheral.WriteCharacteristicAsync(characteristic, data);
                    var res = await peripheral.ReadCharacteristicAsync(characteristic);
                    count = int.Parse(Encoding.UTF8.GetString(res.Data));
                    i++;
                }

                var str = PayloadsHelper.GetStringFromPayload(new ConnectedDevicePayload
                {
#if IOS
                DeviceIdentifier = peripheral.Uuid,
#elif ANDROID
                    DeviceIdentifier = name,
#endif
                    State = TicTacToeState.Zero,
                    CharacteristicUuid = characteristicUuid
                });
                peripheral.CancelConnection();
                await MainThread.InvokeOnMainThreadAsync(() => popup.Hide());
                await Shell.Current.GoToAsync($"{nameof(GamePage)}?Payload={str}");
            }
            catch (Exception e)
            {
                await MainThread.InvokeOnMainThreadAsync(() => popup.Hide());
                await Shell.Current.CurrentPage.DisplayAlert("Error", e.Message, "ok");
            }
        }

        public override void StopScan()
        {
            base.StopScan();
            IsScanning = false;
        }
    }
}