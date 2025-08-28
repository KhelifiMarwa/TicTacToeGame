using Shiny.BluetoothLE.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.ViewModel
{
    internal abstract class BaseHostViewModel : BaseViewModel
    {
        protected readonly IBleHostingManager? Manager;

        protected BaseHostViewModel()
        {
            Manager = Shiny.Hosting.Host.GetService<IBleHostingManager>();
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


        protected virtual void BuildService(IGattServiceBuilder serviceBuilder) { }
    }
}