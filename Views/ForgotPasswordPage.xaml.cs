using KindHome.ViewModels;

namespace KindHome.Views;

public partial class ForgotPasswordPage : ContentPage
{
    readonly ForgotPasswordViewModel viewModel = new();

    public ForgotPasswordPage()
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Le contenu prend au moins la hauteur visible : le bas de page reste en bas,
        // et quand le clavier reduit la zone visible, la page defile.
        Scroller.SizeChanged += (s, e) => ContentGrid.MinimumHeightRequest = Scroller.Height;
    }

    // Entree dans l'e-mail : on passe au telephone
    void OnEmailCompleted(object? sender, EventArgs e) => PhoneField.FocusInput();

    // Entree dans le telephone : on lance la verification
    void OnPhoneCompleted(object? sender, EventArgs e)
    {
        viewModel.CheckCommand.Execute(null);
    }

    // Entree dans le nouveau mot de passe : on passe a la confirmation
    void OnNewPasswordCompleted(object? sender, EventArgs e) => ConfirmField.FocusInput();

    // Entree dans la confirmation : on lance le changement
    void OnConfirmCompleted(object? sender, EventArgs e)
    {
        viewModel.ResetCommand.Execute(null);
    }
}
