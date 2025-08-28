using BluetoothLE_App.Abstractions;
using BluetoothLE_App.Enums;
using BluetoothLE_App.Helpers;
using BluetoothLE_App.ViewModel;

namespace BluetoothLE_App.Views.Popups;

public partial class GameOverPopup 
{
    private readonly GameOverViewModel _viewModel;
    public GameOverPopup(string winner)
    {
        InitializeComponent();
        BindingContext = _viewModel = new GameOverViewModel(winner);
    }

    public void SetMessage(string message)
    {
        _viewModel.Winner = message;
    }

    public void SetIsEnableRetry(bool isEnableRetry)
    {
        _viewModel.IsEnableRetry = isEnableRetry;
    }

    private async Task ExecuteButtonClickAsync(DialogReturnStatuses status)
    {
        if (IsBusy)
        {
            return;
        }
        IsBusy = true;

        await HideAsync();

        _viewModel.SetReturnValue(new DialogReturnValue(status));

        IsBusy = false;
    }

    public override async Task<DialogReturnValue?> ShowAsync(bool animate = true)
    {
        await base.ShowAsync(animate);

        return await _viewModel.ReturnValueAsync();
    }

    private void OnPositiveButtonClicked(object sender, EventArgs e)
    {
        _ = ExecuteButtonClickAsync(DialogReturnStatuses.Positive);
    }

    private void OnNegativeButtonClicked(object sender, EventArgs e)
    {
        _ = ExecuteButtonClickAsync(DialogReturnStatuses.Negative);
    }
}