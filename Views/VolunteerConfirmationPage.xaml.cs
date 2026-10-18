using KindHome.Models;
using KindHome.ViewModels;

namespace KindHome.Views;

// La candidature est passee par la navigation (GoToAsync avec "VolunteerRequest")
[QueryProperty(nameof(Request), "VolunteerRequest")]
public partial class VolunteerConfirmationPage : ContentPage
{
    readonly VolunteerConfirmationViewModel viewModel = new();
    VolunteerRequest? request;

    public VolunteerConfirmationPage()
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Le contenu prend au moins la hauteur visible : les boutons restent en bas,
        // et sur un petit ecran la page defile.
        Scroller.SizeChanged += (s, e) => ContentGrid.MinimumHeightRequest = Scroller.Height;
    }

    public VolunteerRequest? Request
    {
        get => request;
        set
        {
            request = value;
            if (value != null)
                viewModel.Request = value;
        }
    }

    // Le bouton retour d'Android ramene a l'accueil, comme la fleche
    protected override bool OnBackButtonPressed()
    {
        viewModel.HomeCommand.Execute(null);
        return true;
    }
}