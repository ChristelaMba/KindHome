namespace KindHome.Models;

// Chiffres de l'ecran Profil (donateur et benevole)
public class ProfileSummary
{
    public string RoleText { get; set; } = "";
    public string City { get; set; } = "";
    public int DonationsCount { get; set; }
    public int MissionsCount { get; set; }
    public int FollowedCount { get; set; }
    public int ChildrenHelped { get; set; }
}
