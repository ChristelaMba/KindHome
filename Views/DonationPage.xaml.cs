using KindHome.Models;
using KindHome.ViewModels;

namespace KindHome.Views;

// Le besoin est passe par la navigation (GoToAsync avec "Need")
[QueryProperty(nameof(Need), "Need")]
public partial class DonationPage : ContentPage
{
    readonly DonationViewModel viewModel = new();
    Need? need;

    public DonationPage()
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
}
