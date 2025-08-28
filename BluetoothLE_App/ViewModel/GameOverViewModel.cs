using BluetoothLE_App.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.ViewModel
{
    internal class GameOverViewModel : BasePopupViewModel<DialogReturnValue>
    {
        private bool _isEnableRetry;

        public bool IsEnableRetry
        {
            get => _isEnableRetry;
            set
            {
                _isEnableRetry = value;
                OnPropertyChanged();
            }
        }

        private string? _winner;

        public string? Winner
        {
            get => _winner;
            set
            {
                _winner = value;
                OnPropertyChanged();
            }
        }

        public GameOverViewModel(string winner)
        {
            Winner = winner;
            IsEnableRetry = true;
        }
    }
}