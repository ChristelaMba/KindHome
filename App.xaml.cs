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
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}