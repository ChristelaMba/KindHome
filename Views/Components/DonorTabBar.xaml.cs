using KindHome.Views;

namespace KindHome.Views.Components;

// Barre d'onglets du donateur (Accueil, Carte, Besoins, Profil), dessinee comme la maquette.
// Chaque page d'onglet la place en bas et indique son onglet avec ActiveTab.
public partial class DonorTabBar : ContentView
{
    public static readonly BindableProperty ActiveTabProperty = BindableProperty.Create(
        nameof(ActiveTab), typeof(string), typeof(DonorTabBar), "Home",
        propertyChanged: (b, o, n) => ((DonorTabBar)b).Refresh());

    public DonorTabBar()
    {
        InitializeComponent();
        Refresh();
    }

    // "Home", "Directory", "Needs" ou "Profile"
    public string ActiveTab
    {
        get => (string)GetValue(ActiveTabProperty);
        set => SetValue(ActiveTabProperty, value);
    }

    void Refresh()
    {
        if (HomeIcon == null) return;

        Apply(HomeIcon, HomeLabel, "icon_tab_home", ActiveTab == "Home");
        Apply(MapIcon, MapLabel, "icon_tab_map", ActiveTab == "Directory");
        Apply(NeedsIcon, NeedsLabel, "icon_tab_needs", ActiveTab == "Needs");
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

    static async Task Go(string page)
    {
        await Shell.Current.GoToAsync("//" + page);
    }

    async void OnHomeTapped(object? sender, TappedEventArgs e) => await Go(nameof(HomePage));

    async void OnMapTapped(object? sender, TappedEventArgs e) => await Go(nameof(DirectoryPage));

    async void OnNeedsTapped(object? sender, TappedEventArgs e) => await Go(nameof(NeedsPage));

    async void OnProfileTapped(object? sender, TappedEventArgs e) => await Go(nameof(ProfilePage));
}
