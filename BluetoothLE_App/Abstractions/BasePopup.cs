using Mopups.Pages;
using Mopups.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.Abstractions
{
    public abstract class BasePopup<TReturnValue> : PopupPage
    {
        protected BasePopup()
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 128);
        }

        public virtual async Task<TReturnValue?> ShowAsync(bool animate = true)
        {
            try
            {
                await MopupService.Instance.PushAsync(this, animate);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return default;
        }

        public virtual async Task HideAsync(bool animate = true)
        {
            try
            {
                await MopupService.Instance.RemovePageAsync(this, animate);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}