using BluetoothLE_App.ViewModel;
using Shiny.BluetoothLE;

namespace BluetoothLE_App.Views;

public partial class ClientPage : ContentPage
{
    private readonly ClientViewModel _viewModel;

    public ClientPage()
	{
		InitializeComponent();
        var manager = Shiny.Hosting.Host.GetService<IBleManager>();
        manager?.RequestAccess();
        BindingContext = _viewModel = new ClientViewModel();
    }
    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        _viewModel.StopScan();
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}