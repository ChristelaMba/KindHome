using KindHome.ViewModels;

namespace KindHome.Views;

public partial class RegisterPage : ContentPage
{
    readonly RegisterViewModel viewModel = new();

    public RegisterPage()
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Le contenu prend au moins la hauteur visible : le bas de page reste en bas,
        // et quand le clavier reduit la zone visible, la page defile.
        Scroller.SizeChanged += (s, e) => ContentGrid.MinimumHeightRequest = Scroller.Height;
    }

    // Entree passe au champ suivant
    void OnNameCompleted(object? sender, EventArgs e) => EmailField.FocusInput();

    void OnEmailCompleted(object? sender, EventArgs e) => PhoneField.FocusInput();

    void OnPhoneCompleted(object? sender, EventArgs e) => PasswordField.FocusInput();

    void OnPasswordCompleted(object? sender, EventArgs e)
    {
        if (viewModel.CanSubmit)
            viewModel.CreateCommand.Execute(null);
    }
}
