using KindHome.ViewModels;

namespace KindHome.Views;

public partial class NeedsPage : ContentPage
{
    bool firstTime = true;

    public NeedsPage()
    {
        InitializeComponent();
        BindingContext = new NeedsViewModel();
    }

    // A chaque affichage de la page, la liste des besoins est relue
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (firstTime)
        {
            firstTime = false;
            return;
        }

        BindingContext = new NeedsViewModel();
    }
}
