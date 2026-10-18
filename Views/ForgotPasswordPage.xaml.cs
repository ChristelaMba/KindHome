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

    void OnEmailCompleted(object? sender, EventArgs e)
    {
        viewModel.SendCommand.Execute(null);
    }
}
