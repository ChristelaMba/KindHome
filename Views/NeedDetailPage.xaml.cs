using KindHome.Models;
using KindHome.ViewModels;

namespace KindHome.Views;

// Le besoin a afficher est passe par la navigation (GoToAsync avec "Need")
[QueryProperty(nameof(Need), "Need")]
public partial class NeedDetailPage : ContentPage
{
    readonly NeedDetailViewModel viewModel = new();
    Need? need;

    public NeedDetailPage()
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public Need? Need
    {
        get => need;
        set
        {
            need = value;
            if (value != null)
                viewModel.Need = value;
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
}
