using BluetoothLE_App.Abstractions;

namespace BluetoothLE_App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new AppShell());
        //}
        protected override void OnResume()
        {
            try
            {
                //Workaround: call OnAppearing of the page after hide/resume app
                if (Shell.Current.Navigation?.NavigationStack?.Last() is BasePage lastPage)
                    lastPage.InvokeAppearing();
            }
            catch (Exception)
            {
                //ignore
            }
        }

        protected override void OnSleep()
        {
            try
            {
                //Workaround: call OnDisappearing of the page after hide/resume app
                if (Shell.Current.Navigation?.NavigationStack?.Last() is BasePage lastPage)
                    lastPage.InvokeDisappearing();
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }
}
