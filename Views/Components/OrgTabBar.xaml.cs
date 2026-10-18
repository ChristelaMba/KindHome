namespace KindHome.Views.Components;

// Barre d'onglets de l'organisation (Accueil, Besoins, Benevoles, Dons, Profil), dessinee comme la maquette.
public partial class OrgTabBar : ContentView
{
    public static readonly BindableProperty ActiveTabProperty = BindableProperty.Create(
        nameof(ActiveTab), typeof(string), typeof(OrgTabBar), "Home",
        propertyChanged: (b, o, n) => ((OrgTabBar)b).Refresh());

    public OrgTabBar()
    {
        InitializeComponent();
        Refresh();
    }

    // "Home", "Needs", "Volunteers", "Donations" ou "Profile"
    public string ActiveTab
    {
        get => (string)GetValue(ActiveTabProperty);
        set => SetValue(ActiveTabProperty, value);
    }

    void Refresh()
    {
        if (HomeIcon == null) return;

        Apply(HomeIcon, HomeLabel, "icon_tab_home", ActiveTab == "Home");
        Apply(NeedsIcon, NeedsLabel, "icon_tab_needs", ActiveTab == "Needs");
        Apply(VolunteersIcon, VolunteersLabel, "icon_tab_people", ActiveTab == "Volunteers");
        Apply(DonationsIcon, DonationsLabel, "icon_tab_cart", ActiveTab == "Donations");
        Apply(ProfileIcon, ProfileLabel, "icon_tab_profile", ActiveTab == "Profile");
    }

    // Onglet actif : icone pleine, texte en gras, vert fonce. Les autres : icone contour, texte gris.
    static void Apply(Image icon, Label label, string iconName, bool active)
    {
        icon.Source = active ? iconName + "_active.png" : iconName + ".png";
        string style = active ? "TabLabelActive" : "TabLabel";
        if (Application.Current!.Resources.TryGetValue(style, out var value) && value is Style s)
            label.Style = s;
    }

    static async Task Go(string route)
    {
        await Shell.Current.GoToAsync("//" + route);
    }

    async void OnHomeTapped(object? sender, TappedEventArgs e) => await Go("OrgDashboardPage");

    async void OnNeedsTapped(object? sender, TappedEventArgs e) => await Go("OrgNeedsPage");

    async void OnVolunteersTapped(object? sender, TappedEventArgs e) => await Go("OrgVolunteersPage");

    async void OnDonationsTapped(object? sender, TappedEventArgs e) => await Go("OrgDonationsPage");

    async void OnProfileTapped(object? sender, TappedEventArgs e) => await Go("OrgProfilePage");
}