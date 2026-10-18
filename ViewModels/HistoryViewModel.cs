using System.Windows.Input;
using KindHome.Models;
using KindHome.Views;

namespace KindHome.ViewModels;

public class HistoryViewModel : BaseViewModel
{
    bool showDonations = true;

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

    // Envois de la session d'abord (le plus recent en premier), puis les donnees d'exemple du compte
    public List<HistoryEntry> Items
    {
        get
        {
            var user = UserSession.CurrentUser;
            if (showDonations)
                return UserSession.SessionDonations.Concat(user?.Donations ?? new()).ToList();
            return UserSession.SessionMissions.Concat(user?.Missions ?? new()).ToList();
        }
    }

    public bool ShowList => Items.Count > 0;
    public bool ShowEmpty => Items.Count == 0;

    public string EmptyIcon => showDonations ? "icon_gift_empty.png" : "icon_people_empty.png";
    public string EmptyTitle => showDonations ? "Aucun don pour l'instant" : "Aucune mission pour l'instant";

    public string EmptyMessage => showDonations
        ? "Vos dons apparaîtront ici dès votre première proposition."
        : "Vos missions apparaîtront ici dès votre première candidature.";
}