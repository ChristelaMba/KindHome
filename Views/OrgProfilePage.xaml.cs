using KindHome.ViewModels;

namespace KindHome.Views;

public partial class OrgProfilePage : ContentPage
{
    readonly OrgProfileViewModel viewModel = new();

    public OrgProfilePage()
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