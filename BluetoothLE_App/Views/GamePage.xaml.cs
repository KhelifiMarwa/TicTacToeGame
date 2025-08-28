using BluetoothLE_App.Helpers;
using BluetoothLE_App.Models;
using BluetoothLE_App.ViewModel;
using Shiny.BluetoothLE.Hosting;


namespace BluetoothLE_App.Views;
[QueryProperty(nameof(Payload), nameof(Payload))]
public partial class GamePage : ContentPage
{
    private GameViewModel? _viewModel;
    private readonly IBleHostingManager? _manager;

    public string Payload
    {
        set
        {
            var payload = PayloadsHelper.GetPayloadFromString<ConnectedDevicePayload>(value);
            if (payload == null) return;
            BindingContext = _viewModel =
                new GameViewModel(payload.DeviceIdentifier, payload.State, payload.CharacteristicUuid);
            WinnerLine.PaintSurface += _viewModel.WinnerLineCanvasPaintSurface;
            _viewModel.WinnerLineCanvas = WinnerLine;
        }
    }

    public GamePage()
    {
        InitializeComponent();
        _manager = Shiny.Hosting.Host.GetService<IBleHostingManager>();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        if (_viewModel != null) _viewModel.Height = width / 3;
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        _manager?.RemoveService(Helpers.Constants.Texts.DataTransferServiceUuid);
        _viewModel?.StopScan();
        _viewModel?.Peripheral?.CancelConnection();
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
#if ANDROID
        if (_viewModel != null) _viewModel.NotRedraw = true;
#endif
    }
}