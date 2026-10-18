using KindHome.Data;
using KindHome.Views;

namespace KindHome
{
    public partial class AppShell : Shell
    {
        // Pages ouvertes par-dessus la connexion
        static readonly string[] PushedFromLogin =
        {
            nameof(RegisterPage), nameof(ForgotPasswordPage)
        };

        // Pages ouvertes par-dessus les onglets de l'organisation
        static readonly string[] PushedFromOrgTabs =
        {
            nameof(PublishNeedPage)
        };

        // Pages ouvertes par-dessus les onglets
        static readonly string[] PushedFromTabs =
        {
            nameof(OrphanageDetailPage), nameof(NeedDetailPage), nameof(DonationPage),
            nameof(DonationConfirmationPage), nameof(VolunteerPage), nameof(VolunteerConfirmationPage),
            nameof(ContactPage), nameof(SettingsPage), nameof(HistoryPage), nameof(NotificationsPage)
        };

        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(ForgotPasswordPage), typeof(ForgotPasswordPage));
            Routing.RegisterRoute(nameof(OrphanageDetailPage), typeof(OrphanageDetailPage));
            Routing.RegisterRoute(nameof(NeedDetailPage), typeof(NeedDetailPage));
            Routing.RegisterRoute(nameof(DonationPage), typeof(DonationPage));
            Routing.RegisterRoute(nameof(DonationConfirmationPage), typeof(DonationConfirmationPage));
            Routing.RegisterRoute(nameof(VolunteerPage), typeof(VolunteerPage));
            Routing.RegisterRoute(nameof(VolunteerConfirmationPage), typeof(VolunteerConfirmationPage));
            Routing.RegisterRoute(nameof(ContactPage), typeof(ContactPage));
            Routing.RegisterRoute(nameof(PublishNeedPage), typeof(PublishNeedPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(HistoryPage), typeof(HistoryPage));
            Routing.RegisterRoute(nameof(NotificationsPage), typeof(NotificationsPage));

            Loaded += OnShellLoaded;
        }

        // Affiche la page choisie dans DebugFlags.StartRoute
        async void OnShellLoaded(object? sender, EventArgs e)
        {
            Loaded -= OnShellLoaded;
            string start = DebugFlags.StartRoute;
            if (start == nameof(OnboardingPage))
                return;

            try
            {
                if (PushedFromLogin.Contains(start))
                {
                    await GoToAsync("//" + nameof(LoginPage));
                    await GoToAsync(start);
                }
                else if (PushedFromOrgTabs.Contains(start))
                {
                    await GoToAsync("//" + nameof(OrgDashboardPage));
                    await GoToAsync(start);
                }
                else if (PushedFromTabs.Contains(start))
                {
                    await GoToAsync("//" + nameof(HomePage));
                    await GoToAsync(start);
                }
                else
                {
                    await GoToAsync("//" + start);
                }
            }
            catch (Exception)
            {
                await DisplayAlertAsync("KindHome", "StartRoute inconnu : " + start, "OK");
            }
        }
    }
}
