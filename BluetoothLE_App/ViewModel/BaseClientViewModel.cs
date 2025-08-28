using Shiny.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.ViewModel
{
    internal abstract class BaseClientViewModel : BaseViewModel
    {
        protected IDisposable? ScanSub;
        protected readonly IBleManager? BleManager;

        protected BaseClientViewModel()
        {
            BleManager = Shiny.Hosting.Host.GetService<IBleManager>();
        }

        protected void Scan(Action<IList<ScanResult>> func)
        {
            ScanSub = BleManager?.Scan()
                .Buffer(TimeSpan.FromSeconds(1))
                .Where(x => x.Any())
                .Subscribe(func,
                    ex => { Shell.Current.CurrentPage.DisplayAlert("error", ex.Message, "ok"); });
        }

        public virtual void StopScan()
        {
            ScanSub?.Dispose();
            ScanSub = null;
        }
    }
}