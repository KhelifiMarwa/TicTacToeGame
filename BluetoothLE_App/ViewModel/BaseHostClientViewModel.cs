using Shiny.BluetoothLE.Hosting;
using Shiny.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace BluetoothLE_App.ViewModel
{
    internal abstract class BaseHostClientViewModel : BaseViewModel
    {
        protected IDisposable? ScanSub;
        protected readonly IBleManager? BleManager;
        protected readonly IBleHostingManager? Manager;

        protected BaseHostClientViewModel()
        {
            BleManager = Shiny.Hosting.Host.GetService<IBleManager>();
            Manager = Shiny.Hosting.Host.GetService<IBleHostingManager>();

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


        protected async Task StartServer(string serviceUuid)
        {
            if (Manager != null)
            {
                await Manager.AddService(
                    serviceUuid,
                    true,
                    BuildService
                );

                await MainThread.InvokeOnMainThreadAsync(() => Manager.StartAdvertising(new AdvertisementOptions()));
            }
        }

        protected virtual void BuildService(IGattServiceBuilder serviceBuilder)
        {
        }
    }
}