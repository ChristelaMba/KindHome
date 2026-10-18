namespace KindHome.Views;

public partial class OnboardingPage : ContentPage
{
    public OnboardingPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
#if ANDROID
        // Icones blanches de la barre d'etat sur la photo
        KindHome.MainActivity.SetSystemBarIcons(false);
#endif
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
#if ANDROID
        KindHome.MainActivity.SetSystemBarIcons(true);
#endif
    }

    async void OnStartClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//" + nameof(LoginPage));
    }
}
