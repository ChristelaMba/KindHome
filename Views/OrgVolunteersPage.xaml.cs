using KindHome.ViewModels;

namespace KindHome.Views;

public partial class OrgVolunteersPage : ContentPage
{
    readonly OrgVolunteersViewModel viewModel = new();

    public OrgVolunteersPage()
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // L'organisation connectee peut avoir change
        viewModel.Refresh();
    }
}