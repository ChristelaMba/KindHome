using System.Windows.Input;
using KindHome.Models;
using KindHome.Services;
using KindHome.Views;

namespace KindHome.ViewModels;

public class HistoryViewModel : BaseViewModel
{
    bool showDonations = true;

    // Dons et missions lus dans la base (restent vides tant que la lecture n'est pas finie, et pour un invite)
    List<HistoryEntry>? dbDonations;
    List<HistoryEntry>? dbMissions;

    public HistoryViewModel()
    {
        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        ShowDonationsCommand = new Command(() => ShowDonations = true);
        ShowMissionsCommand = new Command(() => ShowDonations = false);
        DiscoverCommand = new Command(async () => await Shell.Current.GoToAsync("//" + nameof(NeedsPage)));
    }

    public ICommand BackCommand { get; }
    public ICommand ShowDonationsCommand { get; }
    public ICommand ShowMissionsCommand { get; }
    public ICommand DiscoverCommand { get; }

    // Une seule puce choisie a la fois : Dons ou Missions
    public bool ShowDonations
    {
        get => showDonations;
        set
        {
            if (!SetProperty(ref showDonations, value)) return;
            OnPropertyChanged(nameof(ShowMissions));
            OnPropertyChanged(nameof(Items));
            OnPropertyChanged(nameof(ShowList));
            OnPropertyChanged(nameof(ShowEmpty));
            OnPropertyChanged(nameof(EmptyIcon));
            OnPropertyChanged(nameof(EmptyTitle));
            OnPropertyChanged(nameof(EmptyMessage));
        }
    }

    public bool ShowMissions => !showDonations;

    // Personne connectee : base de donnees, puis donnees d'exemple du compte.
    // Invite (ou lecture pas encore finie) : envois de la session.
    public List<HistoryEntry> Items
    {
        get
        {
            var user = UserSession.CurrentUser;
            if (showDonations)
            {
                var first = dbDonations ?? UserSession.SessionDonations;
                return first.Concat(user?.Donations ?? new()).ToList();
            }

            var firstMissions = dbMissions ?? UserSession.SessionMissions;
            return firstMissions.Concat(user?.Missions ?? new()).ToList();
        }
    }

    public bool ShowList => Items.Count > 0;
    public bool ShowEmpty => Items.Count == 0;

    public string EmptyIcon => showDonations ? "icon_gift_empty.png" : "icon_people_empty.png";
    public string EmptyTitle => showDonations ? "Aucun don pour l'instant" : "Aucune mission pour l'instant";

    public string EmptyMessage => showDonations
        ? "Vos dons apparaîtront ici dès votre première proposition."
        : "Vos missions apparaîtront ici dès votre première candidature.";

    // Lit la base pour la personne connectee (un invite n'a pas de compte : rien a lire)
    public async Task LoadAsync()
    {
        var user = UserSession.CurrentUser;
        if (user == null || user.Email.Length == 0) return;

        try
        {
            dbDonations = await HistoryService.GetDonationsAsync(user.Email);
            dbMissions = await HistoryService.GetMissionsAsync(user.Email);
        }
        catch (Exception)
        {
            // Lecture impossible : on garde l'affichage de la session
            return;
        }

        OnPropertyChanged(nameof(Items));
        OnPropertyChanged(nameof(ShowList));
        OnPropertyChanged(nameof(ShowEmpty));
    }
}