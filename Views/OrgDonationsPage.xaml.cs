using KindHome.ViewModels;

namespace KindHome.Views;

public partial class OrgDonationsPage : ContentPage
{
    readonly OrgDonationsViewModel viewModel = new();

    public OrgDonationsPage()
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // L'organisation connectee peut avoir change, et de nouveaux dons ont pu arriver
        await viewModel.RefreshAsync();
    }
}