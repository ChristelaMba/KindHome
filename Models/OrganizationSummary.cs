namespace KindHome.Models;

// Chiffres du tableau de bord de l'espace organisation
public class OrganizationSummary
{
    public string Name { get; set; } = "";
    public int Children { get; set; }
    public int Volunteers { get; set; }
    public int DonationsThisMonth { get; set; }
    public Need? FeaturedNeed { get; set; }
    public string VolunteerRequestTitle { get; set; } = "";
    public int VolunteerRequestCount { get; set; }
    public Need? LastDonationNeed { get; set; }
    public int LastDonationQuantity { get; set; }
}
