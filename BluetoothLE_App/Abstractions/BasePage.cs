using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE_App.Abstractions
{
    public abstract class BasePage : ContentPage
    {
        public void InvokeAppearing()
        {
            OnAppearing();
        }

        public void InvokeDisappearing()
        {
            OnDisappearing();
        }
    }
}