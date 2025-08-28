using BluetoothLE_App.ViewModel;
using Shiny.BluetoothLE.Hosting;


namespace BluetoothLE_App.Views;

public partial class HostPage : ContentPage
{
    private readonly IBleHostingManager? _manager;

    public HostPage()
    {
        InitializeComponent();
        BindingContext = new HostViewModel();
        _manager = Shiny.Hosting.Host.GetService<IBleHostingManager>();
        _manager?.RequestAccess();
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        _manager?.RemoveService(Helpers.Constants.Texts.ConnectionServiceUuid);
    }
}