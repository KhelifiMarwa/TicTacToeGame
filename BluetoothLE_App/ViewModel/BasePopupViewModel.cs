using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.ViewModel
{
    public abstract class BasePopupViewModel<TValue> : BaseViewModel
    {
        private readonly TaskCompletionSource<TValue> _taskCompletionSource = new();

        public Task<TValue> ReturnValueAsync()
        {
            return _taskCompletionSource.Task;
        }

        public void SetReturnValue(TValue status)
        {
            _taskCompletionSource.TrySetResult(status);
        }
    }
}