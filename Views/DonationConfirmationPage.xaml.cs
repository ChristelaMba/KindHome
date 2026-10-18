using KindHome.Models;
using KindHome.ViewModels;

namespace KindHome.Views;

// Le don est passe par la navigation (GoToAsync avec "Donation")
[QueryProperty(nameof(Donation), "Donation")]
public partial class DonationConfirmationPage : ContentPage
{
    readonly DonationConfirmationViewModel viewModel = new();
    Donation? donation;

    public DonationConfirmationPage()
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Le contenu prend au moins la hauteur visible : les boutons restent en bas,
        // et sur un petit ecran la page defile.
        Scroller.SizeChanged += (s, e) => ContentGrid.MinimumHeightRequest = Scroller.Height;
    }

    public Donation? Donation
    {
        get => donation;
        set
        {
            donation = value;
            if (value != null)
                viewModel.Donation = value;
        }
    }

    // Le bouton retour d'Android ramene a l'accueil, comme la fleche
    protected override bool OnBackButtonPressed()
    {
        viewModel.HomeCommand.Execute(null);
        return true;
    }
}
