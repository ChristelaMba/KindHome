using System.Globalization;
using KindHome.Data;

namespace KindHome.Models;

// Personne connectee pendant que l'application est ouverte
public static class UserSession
{
    static readonly CultureInfo French = new("fr-FR");

    public static UserAccount? CurrentUser { get; private set; }

    public static UserRole Role => CurrentUser?.Role ?? UserRole.Guest;

    // Dons et candidatures envoyes pendant la session (invite compris), le plus recent en premier
    public static List<HistoryEntry> SessionDonations { get; } = new();
    public static List<HistoryEntry> SessionMissions { get; } = new();

    // Besoins publies pendant la session par l'organisation connectee, le plus recent en premier
    public static List<Need> PublishedNeeds { get; } = new();

    // Interrupteur "Notifications" des reglages (en memoire seulement)
    public static bool NotificationsEnabled { get; set; } = true;

    // Une nouvelle personne (connexion, inscription, invite) repart avec un historique de session vide
    public static void SignIn(UserAccount account)
    {
        ClearSession();
        CurrentUser = account;
    }

    public static void SignInAsGuest()
    {
        ClearSession();
        CurrentUser = null;
    }

    // Deconnexion : plus de personne connectee, plus d'historique de session
    public static void SignOut()
    {
        CurrentUser = null;
        ClearSession();
    }

    static void ClearSession()
    {
        SessionDonations.Clear();
        SessionMissions.Clear();
        PublishedNeeds.Clear();
        NotificationsEnabled = true;
    }

    public static void AddDonation(Donation donation)
    {
        SessionDonations.Insert(0, new HistoryEntry
        {
            Title = donation.Need.Title + " - " + donation.AmountText,
            Orphanage = donation.Need.Orphanage.Name,
            Detail = Today(),
            Status = "En attente"
        });
    }

    public static void AddMission(VolunteerRequest request)
    {
        SessionMissions.Insert(0, new HistoryEntry
        {
            Title = request.DomainsText,
            Orphanage = request.Orphanage.Name,
            Detail = request.DaysText + ", " + request.Slot.ToLower(French),
            Status = "En attente",
            ExtraLine = Today()
        });
    }

    // Besoins de l'organisation connectee, lus dans la liste des besoins (le plus recent en premier)
    public static List<Need> OrgNeeds()
    {
        var user = CurrentUser;
        if (user == null || user.Role != UserRole.Organization || DebugFlags.SimulateNoOrgNeeds)
            return new List<Need>();

        var list = MockData.Needs.Where(n => n.Orphanage.Name == user.FullName).ToList();
        list.Reverse();
        return list;
    }

    // "18 octobre 2026"
    static string Today() => DateTime.Today.ToString("d MMMM yyyy", French);
}