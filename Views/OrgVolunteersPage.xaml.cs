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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // L'organisation connectee peut avoir change, et de nouvelles candidatures ont pu arriver
        await viewModel.RefreshAsync();
    }
}