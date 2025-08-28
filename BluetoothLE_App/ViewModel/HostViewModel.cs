using BluetoothLE_App.Enums;
using BluetoothLE_App.Helpers;
using BluetoothLE_App.Models;
using BluetoothLE_App.Views;
using Shiny.BluetoothLE.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BluetoothLE_App.Helpers.Constants.Texts;
namespace BluetoothLE_App.ViewModel
{
    internal class HostViewModel : BaseHostViewModel
    {
        private bool _isConnected;
        private string _characteristicUuid;
        private string _newUuid;
        private int _count;
        private int Read => _characteristicUuid.Length;

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        public string ConnectedDevice
        {
            set
            {
                IsConnected = true;
                var screenParam = value;
                var str = PayloadsHelper.GetStringFromPayload(new ConnectedDevicePayload
                {
                    DeviceIdentifier = screenParam,
                    State = TicTacToeState.Cross,
                    CharacteristicUuid = _characteristicUuid
                });

                MainThread.InvokeOnMainThreadAsync(() =>
                    Shell.Current.GoToAsync($"{nameof(GamePage)}?Payload={str}"));
            }
        }

        public HostViewModel()
        {
            _characteristicUuid = string.Empty;
            IsConnected = false;
            _newUuid = Guid.NewGuid().ToString();
            Task.Run(() => StartServer(ConnectionServiceUuid));
        }


        protected override void BuildService(IGattServiceBuilder serviceBuilder)
        {
            serviceBuilder.AddCharacteristic(
                _newUuid,
                cb =>
                {
                    cb.SetWrite(request =>
                    {
                        if (_count != 0)
                        {
                            _characteristicUuid += Encoding.UTF8.GetString(request.Data, 0, request.Data.Length);
                            if (Read == _count)
                            {
#if IOS
                            ConnectedDevice = $"{request.Peripheral.Uuid}";
#elif ANDROID
                                ConnectedDevice = $"{((Peripheral)request.Peripheral).Native.Name}";
#endif
                            }
                        }
                        else
                        {
                            _count = int.Parse(Encoding.UTF8.GetString(request.Data, 0, request.Data.Length));
                        }

                        return Task.FromResult(GattState.Success);
                    });
                    cb.SetRead(request => Task.FromResult(GattResult.Success(Encoding.UTF8.GetBytes(Read.ToString()))));
                }
            );
        }
    }
}