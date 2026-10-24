using KindHome.ViewModels;

namespace KindHome.Views;

public partial class OrgDashboardPage : ContentPage
{
    readonly OrgDashboardViewModel viewModel = new();

    public OrgDashboardPage()
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // L'organisation connectee peut avoir change depuis la derniere fois
        viewModel.Refresh();
        await viewModel.LoadPendingAsync();
    }
}