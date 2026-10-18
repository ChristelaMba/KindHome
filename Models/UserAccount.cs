namespace KindHome.Models;

public class UserAccount
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string Phone { get; set; } = "";
    public UserRole Role { get; set; }

    // Photo de profil (nom de fichier dans Resources/Images), vide si la personne n'en a pas
    public string Photo { get; set; } = "";

    // Chiffres du profil et du tableau de bord (vides pour un compte cree a l'inscription : tout est a 0)
    public ProfileSummary? Profile { get; set; }
    public OrganizationSummary? Organization { get; set; }

    // Historique et notifications d'exemple (vides pour un compte cree a l'inscription)
    public List<HistoryEntry> Donations { get; set; } = new();
    public List<HistoryEntry> Missions { get; set; } = new();
    public List<AppNotification> Notifications { get; set; } = new();

    // Espace organisation : demandes de benevolat et dons recus (vides pour une organisation creee a l'inscription)
    public List<OrgRequest> OrgRequests { get; set; } = new();
    public List<OrgDonation> OrgDonations { get; set; } = new();

    // Premier prenom, pour la salutation ("Christela")
    public string FirstName
    {
        get
        {
            string name = FullName.Trim();
            int space = name.IndexOf(' ');
            return space < 0 ? name : name.Substring(0, space);
        }
    }
}
