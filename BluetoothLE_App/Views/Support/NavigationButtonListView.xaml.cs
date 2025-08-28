using BluetoothLE_App.Models;
using System.Windows.Input;

namespace BluetoothLE_App.Views.Support;

public partial class NavigationButtonListView : ContentView
{
    #region Bindable Properties

    #region ItemsSource Property

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable<NavigationButtonItem>),
        typeof(NavigationButtonListView));

    public IEnumerable<NavigationButtonItem>? ItemsSource
    {
        get => (IEnumerable<NavigationButtonItem>?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    #endregion ItemsSource Property

    #region Command Property

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command),
        typeof(ICommand),
        typeof(NavigationButtonListView));

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    #endregion Command Property

    #endregion Bindable Properties

    public NavigationButtonListView()
    {
        InitializeComponent();
    }
}