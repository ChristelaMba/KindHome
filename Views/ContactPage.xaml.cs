using KindHome.Models;
using KindHome.ViewModels;

namespace KindHome.Views;

// L'orphelinat est passe par la navigation (GoToAsync avec "Orphanage")
[QueryProperty(nameof(Orphanage), "Orphanage")]
public partial class ContactPage : ContentPage
{
    readonly ContactViewModel viewModel = new();
    Orphanage? orphanage;

    public ContactPage()
    {
        InitializeComponent();
        BindingContext = viewModel;
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
}