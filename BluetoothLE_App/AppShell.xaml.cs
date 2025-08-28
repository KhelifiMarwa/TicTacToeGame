using BluetoothLE_App.Views;

namespace BluetoothLE_App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(HostPage), typeof(HostPage));
            Routing.RegisterRoute(nameof(ClientPage), typeof(ClientPage));
            Routing.RegisterRoute(nameof(GamePage), typeof(GamePage));
        }
    }
}
