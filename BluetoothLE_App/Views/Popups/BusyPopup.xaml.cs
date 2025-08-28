using BluetoothLE_App.Abstractions;

namespace BluetoothLE_App.Views.Popups;

public partial class BusyPopup 
{
	public BusyPopup()
	{
		InitializeComponent();
    }
    public async Task Hide()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        await HideAsync();

        IsBusy = false;
    }
}