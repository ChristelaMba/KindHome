using KindHome.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KindHome
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Theme clair uniquement, comme la maquette
            UserAppTheme = AppTheme.Light;

            // Charge les besoins depuis la base avant l'ouverture des pages
            Task.Run(() => NeedService.LoadAsync()).GetAwaiter().GetResult();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}