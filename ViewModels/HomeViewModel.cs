using System.Windows.Input;
using KindHome.Data;
using KindHome.Models;
using KindHome.Views;

namespace KindHome.ViewModels;

public class HomeViewModel : BaseViewModel
{
    string greetingName = "";
    string avatarPhoto = "";
    string avatarInitial = "";

    public HomeViewModel()
    {
        GoToDirectoryCommand = new Command(async () => await Shell.Current.GoToAsync("//" + nameof(DirectoryPage)));
        GoToProfileCommand = new Command(async () => await Shell.Current.GoToAsync("//" + nameof(ProfilePage)));
        NotificationsCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(NotificationsPage)));
        OpenNeedCommand = new Command<Need>(async need => await OpenNeedAsync(need));
        OpenOrphanageCommand = new Command<Orphanage>(async orphanage => await OpenOrphanageAsync(orphanage));
        Refresh();
    }

    public ICommand GoToDirectoryCommand { get; }
    public ICommand GoToProfileCommand { get; }
    public ICommand NotificationsCommand { get; }
    public ICommand OpenNeedCommand { get; }
    public ICommand OpenOrphanageCommand { get; }

    // Salutation : prenom de la personne connectee, "Invité" pour un invite
    public string GreetingName
    {
        get => greetingName;
        private set => SetProperty(ref greetingName, value);
    }

    public string AvatarPhoto
    {
        get => avatarPhoto;
        private set
        {
            if (!SetProperty(ref avatarPhoto, value)) return;
            OnPropertyChanged(nameof(HasAvatarPhoto));
            OnPropertyChanged(nameof(HasNoAvatarPhoto));
        }
    }

    public string AvatarInitial
    {
        get => avatarInitial;
        private set => SetProperty(ref avatarInitial, value);
    }

    public bool HasAvatarPhoto => avatarPhoto.Length > 0;
    public bool HasNoAvatarPhoto => avatarPhoto.Length == 0;

    // Besoin mis en avant dans la grande carte
    public Need FeaturedNeed => MockData.FeaturedNeed;
    public Orphanage FeaturedOrphanage => MockData.FeaturedNeed.Orphanage;

    // Textes de la grande carte, comme sur la maquette ("Douala" et non "Bonapriso", "Il manque 12 unites")
    public string FeaturedLocationText => "Douala - " + FeaturedOrphanage.DistanceText;
    public string FeaturedMissingText => "Il manque 12 unités";

    public Orphanage NearbyLeft => MockData.NearbyOnHome[0];
    public Orphanage NearbyRight => MockData.NearbyOnHome[1];

    // A rappeler quand la page s'affiche : la personne connectee peut avoir change
    public void Refresh()
    {
        var user = UserSession.CurrentUser;
        string name = user == null ? "Invité" : user.FirstName;
        GreetingName = name;
        AvatarPhoto = user?.Photo ?? "";
        AvatarInitial = name.Length > 0 ? name.Substring(0, 1).ToUpper() : "";
    }

    static async Task OpenNeedAsync(Need? need)
    {
        if (need == null) return;
        await Shell.Current.GoToAsync(nameof(NeedDetailPage), new Dictionary<string, object> { { "Need", need } });
    }

    static async Task OpenOrphanageAsync(Orphanage? orphanage)
    {
        if (orphanage == null) return;
        await Shell.Current.GoToAsync(nameof(OrphanageDetailPage), new Dictionary<string, object> { { "Orphanage", orphanage } });
    }
}
