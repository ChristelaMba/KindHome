using Microsoft.UI.Xaml;

namespace KindHome.WinUI
{
    // Application WinUI : demarre l'application MAUI
    public partial class App : MauiWinUIApplication
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

}
