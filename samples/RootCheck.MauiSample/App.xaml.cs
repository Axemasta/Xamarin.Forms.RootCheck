using RootCheck.Core;
using RootCheck.MauiSample.Views;

namespace RootCheck.MauiSample;

public partial class App : Application
{
    public App(IRootChecker rootChecker)
    {
        InitializeComponent();

        if (rootChecker.IsDeviceRooted())
        {
            MainPage = new RootedPage();
            return;
        }

        MainPage = new NavigationPage(new MainPage());
    }
}
