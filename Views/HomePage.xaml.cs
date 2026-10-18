using KindHome.ViewModels;

namespace KindHome.Views;

public partial class HomePage : ContentPage
{
    readonly HomeViewModel viewModel = new();

    public HomePage()
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
