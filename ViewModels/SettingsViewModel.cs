using System.Windows.Input;
using KindHome.Models;
using KindHome.Views;

namespace KindHome.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    const string TermsText = "En utilisant KindHome, vous acceptez de partager des informations exactes. Vos dons et vos candidatures sont transmis aux orphelinats choisis. Nous ne cédons jamais vos coordonnées à d'autres personnes.";

    const string CharterText = "Chaque enfant doit se sentir en sécurité et respecté. Les bénévoles ne restent jamais seuls avec un enfant et ne partagent aucune photo sans accord. En cas de doute, prévenez toujours l'orphelinat.";

    public SettingsViewModel()
    {
        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        TermsCommand = new Command(async () => await Shell.Current.DisplayAlertAsync("Conditions d'utilisation", TermsText, "OK"));
        CharterCommand = new Command(async () => await Shell.Current.DisplayAlertAsync("Charte de protection de l'enfant", CharterText, "OK"));
        SignOutCommand = new Command(async () => await SignOutAsync());
    }

    public ICommand BackCommand { get; }
    public ICommand TermsCommand { get; }
    public ICommand CharterCommand { get; }
    public ICommand SignOutCommand { get; }

    static bool IsGuest => UserSession.CurrentUser == null;

    // Section "Mon compte" : valeurs de la personne connectee ("Invité" et "Non renseigné" pour un invite)
    // "Nom de la structure" pour une organisation
    public string NameLabel => UserSession.Role == UserRole.Organization ? "Nom de la structure" : "Nom";

    public string NameText => UserSession.CurrentUser?.FullName ?? "Invité";
    public string EmailText => Or(UserSession.CurrentUser?.Email);
    public string PhoneText => Or(UserSession.CurrentUser?.Phone);

    public string SignOutText => IsGuest ? "Se connecter" : "Se déconnecter";

    // Interrupteur "Notifications" (en memoire seulement)
    public bool NotificationsOn
    {
        get => UserSession.NotificationsEnabled;
        set
        {
            if (UserSession.NotificationsEnabled == value) return;
            UserSession.NotificationsEnabled = value;
            OnPropertyChanged();
        }
    }

    static string Or(string? value) => string.IsNullOrWhiteSpace(value) ? "Non renseigné" : value;

    async Task SignOutAsync()
    {
        // L'invite n'a rien a quitter : il va directement a la connexion
        if (!IsGuest)
        {
            bool confirmed = await Shell.Current.DisplayAlertAsync("Se déconnecter ?",
                "Vous devrez vous reconnecter pour retrouver votre compte.", "Se déconnecter", "Annuler");
            if (!confirmed) return;
            UserSession.SignOut();
        }

        // Pile de pages videe : le retour ne ramene pas aux pages de la session
        await Shell.Current.GoToAsync("//" + nameof(LoginPage));
    }
}