using KindHome.ViewModels;

namespace KindHome.Views;

public partial class ProfilePage : ContentPage
{
    readonly ProfileViewModel viewModel = new();

    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // La personne connectee peut avoir change depuis la derniere fois
        viewModel.Refresh();
    }
}
