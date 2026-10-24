using System.Windows.Input;
using KindHome.Data;
using KindHome.Models;
using KindHome.Services;
using KindHome.Views;

namespace KindHome.ViewModels;

public class OrgDashboardViewModel : BaseViewModel
{
    string orgName = "";
    int children;
    int volunteers;
    int donationsThisMonth;
    bool hasNeeds;
    bool hasOrgData;
    Need? featuredNeed;
    string volunteerTitle = "";
    string volunteerSubtitle = "";
    Need? lastDonationNeed;
    string lastDonationSubtitle = "";

    public OrgDashboardViewModel()
    {
        GoToNeedsCommand = new Command(async () => await Shell.Current.GoToAsync("//OrgNeedsPage"));
        GoToVolunteersCommand = new Command(async () => await Shell.Current.GoToAsync("//OrgVolunteersPage"));
        GoToDonationsCommand = new Command(async () => await Shell.Current.GoToAsync("//OrgDonationsPage"));
        PublishCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(PublishNeedPage)));
        NotificationsCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(NotificationsPage)));
        Refresh();
    }

    public ICommand GoToNeedsCommand { get; }
    public ICommand GoToVolunteersCommand { get; }
    public ICommand GoToDonationsCommand { get; }
    public ICommand PublishCommand { get; }
    public ICommand NotificationsCommand { get; }

    public string OrgName { get => orgName; private set => SetProperty(ref orgName, value); }
    public int Children { get => children; private set => SetProperty(ref children, value); }
    public int Volunteers { get => volunteers; private set => SetProperty(ref volunteers, value); }
    public int DonationsThisMonth { get => donationsThisMonth; private set => SetProperty(ref donationsThisMonth, value); }

    // Faux : on affiche l'etat "Aucun besoin publie" a la place des sections
    public bool HasNeeds
    {
        get => hasNeeds;
        private set
        {
            if (!SetProperty(ref hasNeeds, value)) return;
            OnPropertyChanged(nameof(HasNoNeeds));
        }
    }

    public bool HasNoNeeds => !hasNeeds;

    // Demandes et dons : visibles pour une organisation qui a des donnees (pas pour une organisation creee a l'inscription)
    public bool HasOrgData { get => hasOrgData; private set => SetProperty(ref hasOrgData, value); }

    public Need? FeaturedNeed { get => featuredNeed; private set => SetProperty(ref featuredNeed, value); }
    public string VolunteerTitle { get => volunteerTitle; private set => SetProperty(ref volunteerTitle, value); }
    public string VolunteerSubtitle { get => volunteerSubtitle; private set => SetProperty(ref volunteerSubtitle, value); }
    public Need? LastDonationNeed { get => lastDonationNeed; private set => SetProperty(ref lastDonationNeed, value); }
    public string LastDonationSubtitle { get => lastDonationSubtitle; private set => SetProperty(ref lastDonationSubtitle, value); }

    // A rappeler quand la page s'affiche : l'organisation connectee peut avoir change
    public void Refresh()
    {
        var user = UserSession.CurrentUser;
        var summary = user?.Organization;

        OrgName = user?.FullName ?? MockData.MaisonAngesDashboard.Name;

        // Organisation creee a l'inscription : chiffres a 0 et aucun besoin publie
        Children = summary?.Children ?? 0;
        Volunteers = summary?.Volunteers ?? 0;
        DonationsThisMonth = summary?.DonationsThisMonth ?? 0;

        // Le premier besoin de la liste (publies d'abord) ; l'etat vide disparait des qu'il y a un besoin
        var needs = UserSession.OrgNeeds();
        FeaturedNeed = needs.FirstOrDefault();
        HasNeeds = FeaturedNeed != null;
        HasOrgData = summary != null;

        VolunteerTitle = summary?.VolunteerRequestTitle ?? "";
        // Demandes encore en attente (donnees d'exemple ; LoadPendingAsync ajoute celles de la base)
        int pending = user?.OrgRequests.Count(r => r.Status == "En attente") ?? 0;
        VolunteerSubtitle = summary == null ? "" : PendingText(pending);

        LastDonationNeed = summary?.LastDonationNeed;
        LastDonationSubtitle = summary?.LastDonationNeed == null
            ? ""
            : summary.LastDonationQuantity + " " + summary.LastDonationNeed.Unit;
    }

    // Ajoute les candidatures de la base au nombre de demandes en attente
    public async Task LoadPendingAsync()
    {
        var user = UserSession.CurrentUser;
        if (user == null || user.Role != UserRole.Organization || user.Organization == null) return;

        int dbPending;
        try
        {
            dbPending = await OrgService.CountPendingRequestsAsync(user.FullName);
        }
        catch (Exception)
        {
            // Lecture impossible : on garde le texte affiche
            return;
        }

        // La personne connectee a change pendant la lecture : on ignore le resultat
        if (UserSession.CurrentUser != user) return;

        int pending = dbPending + user.OrgRequests.Count(r => r.Status == "En attente");
        VolunteerSubtitle = PendingText(pending);
    }

    static string PendingText(int pending)
    {
        if (pending == 0) return "Aucune demande en attente";
        if (pending == 1) return "1 demande";
        return pending + " demandes";
    }
}