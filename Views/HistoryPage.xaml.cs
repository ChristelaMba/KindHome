using KindHome.ViewModels;

namespace KindHome.Views;

// L'onglet a montrer est passe par la navigation (GoToAsync avec "Tab" : "Donations" ou "Missions")
[QueryProperty(nameof(Tab), "Tab")]
public partial class HistoryPage : ContentPage
{
    readonly HistoryViewModel viewModel = new();
    string tab = "Donations";

    public HistoryPage()
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public string Tab
    {
        get => tab;
        set
        {
            tab = value;
            viewModel.ShowDonations = value != "Missions";
        }
    }

    // A chaque ouverture, l'historique est relu dans la base
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.LoadAsync();
    }
}