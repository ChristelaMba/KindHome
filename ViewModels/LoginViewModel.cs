using System.Text.RegularExpressions;
using System.Windows.Input;
using KindHome.Data;
using KindHome.Models;
using KindHome.Views;

namespace KindHome.ViewModels;

public class LoginViewModel : BaseViewModel
{
    const string LoginText = "Se connecter";
    const string RetryText = "Réessayer";

    string email = "";
    string password = "";
    string emailError = "";
    string passwordError = "";
    string buttonText = LoginText;
    bool submitted;
    bool wrongCredentials;

    public LoginViewModel()
    {
        LoginCommand = new Command(async () => await LoginAsync());
        GuestCommand = new Command(async () => await GuestAsync());
        // Ces deux pages s'empilent par-dessus la connexion (la fleche retour revient avec "..")
        ForgotPasswordCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(ForgotPasswordPage)));
        RegisterCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(RegisterPage)));
    }

    public string Email
    {
        get => email;
        set
        {
            if (!SetProperty(ref email, value)) return;
            ClearWrongCredentials();
            if (submitted) ValidateEmail();
        }
    }

    public string Password
    {
        get => password;
        set
        {
            if (!SetProperty(ref password, value)) return;
            ClearWrongCredentials();
            if (submitted) ValidatePassword();
        }
    }

    public string EmailError
    {
        get => emailError;
        set => SetProperty(ref emailError, value);
    }

    public string PasswordError
    {
        get => passwordError;
        set => SetProperty(ref passwordError, value);
    }

    public string ButtonText
    {
        get => buttonText;
        set => SetProperty(ref buttonText, value);
    }

    public ICommand LoginCommand { get; }
    public ICommand GuestCommand { get; }
    public ICommand ForgotPasswordCommand { get; }
    public ICommand RegisterCommand { get; }

    // Quand la personne modifie un champ apres un mauvais couple, l'etat "Mot de passe incorrect" disparait
    void ClearWrongCredentials()
    {
        if (!wrongCredentials) return;
        wrongCredentials = false;
        PasswordError = "";
        ButtonText = LoginText;
    }

    bool ValidateEmail()
    {
        string value = Email.Trim();
        if (value.Length == 0)
            EmailError = "Adresse e-mail obligatoire";
        else if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            EmailError = "Adresse e-mail invalide. Exemple : nom@mail.com";
        else
            EmailError = "";
        return EmailError.Length == 0;
    }

    bool ValidatePassword()
    {
        PasswordError = Password.Length == 0 ? "Mot de passe obligatoire" : "";
        return PasswordError.Length == 0;
    }

    async Task LoginAsync()
    {
        submitted = true;
        bool emailOk = ValidateEmail();
        bool passwordOk = ValidatePassword();
        if (!emailOk || !passwordOk) return;

        var account = MockData.FindAccount(Email, Password);
        if (account == null)
        {
            wrongCredentials = true;
            PasswordError = "Mot de passe incorrect. Vérifiez-le ou réinitialisez-le.";
            ButtonText = RetryText;
            return;
        }

        UserSession.SignIn(account);
        string route = account.Role == UserRole.Organization ? nameof(OrgDashboardPage) : nameof(HomePage);
        await Shell.Current.GoToAsync("//" + route);
    }

    async Task GuestAsync()
    {
        UserSession.SignInAsGuest();
        await Shell.Current.GoToAsync("//" + nameof(HomePage));
    }
}