using KindHome.Models;
using KindHome.ViewModels;

namespace KindHome.Views;

// L'orphelinat a afficher est passe par la navigation (GoToAsync avec "Orphanage")
[QueryProperty(nameof(Orphanage), "Orphanage")]
public partial class OrphanageDetailPage : ContentPage
{
    readonly OrphanageDetailViewModel viewModel = new();
    Orphanage? orphanage;

    public OrphanageDetailPage()
    {
        InitializeComponent();
        BindingContext = viewModel;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
        UpdateTabs();
    }

    public Orphanage? Orphanage
    {
        get => orphanage;
        set
        {
            orphanage = value;
            if (value != null)
                viewModel.Orphanage = value;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
#if ANDROID
        // Icones blanches de la barre d'etat sur la photo
        KindHome.MainActivity.SetSystemBarIcons(false);
#endif
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
#if ANDROID
        KindHome.MainActivity.SetSystemBarIcons(true);
#endif
    }

    void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(OrphanageDetailViewModel.Tab))
            UpdateTabs();
    }

    // Onglet choisi : texte en semi-gras vert fonce et trait de 2 en dessous
    void UpdateTabs()
    {
        SetTab(AboutTabLabel, AboutTabLine, viewModel.IsAboutTab);
        SetTab(NeedsTabLabel, NeedsTabLine, viewModel.IsNeedsTab);
        SetTab(NewsTabLabel, NewsTabLine, viewModel.IsNewsTab);
    }

    static void SetTab(Label label, BoxView line, bool active)
    {
        string key = active ? "DetailTabActive" : "DetailTab";
        if (Application.Current!.Resources.TryGetValue(key, out var value) && value is Style style)
            label.Style = style;
        line.IsVisible = active;
    }
}