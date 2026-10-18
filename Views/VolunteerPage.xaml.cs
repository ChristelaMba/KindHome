using KindHome.Models;
using KindHome.ViewModels;

namespace KindHome.Views;

// Le besoin est passe par la navigation (GoToAsync avec "Need") : l'orphelinat est celui du besoin
[QueryProperty(nameof(Need), "Need")]
public partial class VolunteerPage : ContentPage
{
    readonly VolunteerViewModel viewModel = new();
    Need? need;

    public VolunteerPage()
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