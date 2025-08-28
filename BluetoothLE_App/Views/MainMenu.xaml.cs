using BluetoothLE_App.ViewModel;

namespace BluetoothLE_App.Views;

public partial class MainMenu : ContentPage
{
	public MainMenu()
	{
		InitializeComponent();
        BindingContext = new NavigationViewModel();
    }
}