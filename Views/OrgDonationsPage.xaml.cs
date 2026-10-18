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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // L'organisation connectee peut avoir change
        viewModel.Refresh();
    }
}