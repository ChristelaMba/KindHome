using KindHome.ViewModels;

namespace KindHome.Views;

public partial class OrgNeedsPage : ContentPage
{
    readonly OrgNeedsViewModel viewModel = new();

    public OrgNeedsPage()
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Un besoin a pu etre publie, ou l'organisation connectee a pu changer
        viewModel.Refresh();
    }
}