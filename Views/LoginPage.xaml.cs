using KindHome.ViewModels;

namespace KindHome.Views;

public partial class LoginPage : ContentPage
{
    readonly LoginViewModel viewModel = new();

    public LoginPage()
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Le contenu prend au moins la hauteur visible : le bas de page reste en bas,
        // et quand le clavier reduit la zone visible, la page defile.
        Scroller.SizeChanged += (s, e) => ContentGrid.MinimumHeightRequest = Scroller.Height;
    }

    void OnPasswordCompleted(object? sender, EventArgs e)
    {
        viewModel.LoginCommand.Execute(null);
    }
}
