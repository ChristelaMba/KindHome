using System.Windows.Input;
using KindHome.Models;
using KindHome.Views;

namespace KindHome.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    string name = "";
    string roleText = "";
    string city = "Douala, Cameroun";
    string photo = "";
    string initial = "";
    int donationsCount;
    int missionsCount;
    int followedCount;
    string impactBefore = "";
    string impactBold = "";
    string impactAfter = "";
    string buttonText = "";
    bool isGuest;

    public ProfileViewModel()
    {
        SettingsCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(SettingsPage)));
        ActionCommand = new Command(async () => await OnActionAsync());
        Refresh();
    }

    public ICommand SettingsCommand { get; }
    public ICommand ActionCommand { get; }

    public string Name { get => name; private set => SetProperty(ref name, value); }
    public string RoleText { get => roleText; private set => SetProperty(ref roleText, value); }
    public string City { get => city; private set => SetProperty(ref city, value); }
    public string Initial { get => initial; private set => SetProperty(ref initial, value); }
    public int DonationsCount { get => donationsCount; private set => SetProperty(ref donationsCount, value); }
    public int MissionsCount { get => missionsCount; private set => SetProperty(ref missionsCount, value); }
    public int FollowedCount { get => followedCount; private set => SetProperty(ref followedCount, value); }

    // Texte de "Mon impact" en trois morceaux : le morceau du milieu est en gras
    public string ImpactBefore { get => impactBefore; private set => SetProperty(ref impactBefore, value); }
    public string ImpactBold { get => impactBold; private set => SetProperty(ref impactBold, value); }
    public string ImpactAfter { get => impactAfter; private set => SetProperty(ref impactAfter, value); }

    public string ButtonText { get => buttonText; private set => SetProperty(ref buttonText, value); }

    public string Photo
    {
        get => photo;
        private set
        {
            if (!SetProperty(ref photo, value)) return;
            OnPropertyChanged(nameof(HasPhoto));
            OnPropertyChanged(nameof(HasNoPhoto));
        }
    }

    public bool HasPhoto => photo.Length > 0;
    public bool HasNoPhoto => photo.Length == 0;

    // A rappeler quand la page s'affiche : la personne connectee peut avoir change
    public void Refresh()
    {
        var user = UserSession.CurrentUser;
        isGuest = user == null;

        // Meme lieu pour tout le monde, sauf si le profil en donne un
        City = user?.Profile?.City ?? "Douala, Cameroun";

        if (user == null)
        {
            // Invite (cas absent de la maquette)
            Name = "Invité";
            RoleText = "Sans compte";
            Photo = "";
            SetCounts(0, 0, 0);
            SetImpact("Créez un compte pour suivre vos dons et vos missions.", "", "");
            ButtonText = "Créer un compte";
        }
        else if (user.Profile != null)
        {
            // Christela : textes de la maquette
            var p = user.Profile;
            Name = user.FullName;
            RoleText = p.RoleText;
            City = p.City;
            Photo = user.Photo;
            SetCounts(p.DonationsCount, p.MissionsCount, p.FollowedCount);
            SetImpact("Grâce à vous, ", p.ChildrenHelped + " enfants", " ont reçu une aide cette année. Merci !");
            ButtonText = "Voir mon historique";
        }
        else
        {
            // Compte cree a l'inscription
            Name = user.FullName;
            RoleText = user.Role == UserRole.Volunteer ? "Bénévole" : "Donateur";
            Photo = user.Photo;
            SetCounts(0, 0, 0);
            SetImpact("Vos dons et vos missions apparaîtront ici. Merci de votre engagement !", "", "");
            ButtonText = "Voir mon historique";
        }

        string first = user?.FirstName ?? "Invité";
        Initial = first.Length > 0 ? first.Substring(0, 1).ToUpper() : "";
    }

    // Les dons et candidatures envoyes pendant la session s'ajoutent aux chiffres
    void SetCounts(int donations, int missions, int followed)
    {
        DonationsCount = donations + UserSession.SessionDonations.Count;
        MissionsCount = missions + UserSession.SessionMissions.Count;
        FollowedCount = followed;
    }

    void SetImpact(string before, string bold, string after)
    {
        ImpactBefore = before;
        ImpactBold = bold;
        ImpactAfter = after;
    }

    async Task OnActionAsync()
    {
        if (isGuest)
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        else
            await Shell.Current.GoToAsync(nameof(HistoryPage), new Dictionary<string, object> { { "Tab", "Donations" } });
    }
}
