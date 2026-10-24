namespace KindHome.Models;

// La candidature de benevole en cours de saisie, puis envoyee a la page de confirmation
public class VolunteerRequest
{
    static readonly string[] DayNames = { "lundi", "mardi", "mercredi", "jeudi", "vendredi", "samedi", "dimanche" };

    // Orphelinat vide, utilise tant que la candidature n'a pas de besoin
    static readonly Orphanage EmptyOrphanage = new();

    public Need Need { get; set; } = null!;
    public Orphanage Orphanage => Need?.Orphanage ?? EmptyOrphanage;

    // Domaines choisis, dans l'ordre des puces
    public List<string> Domains { get; set; } = new();

    // Jours choisis : 0 = lundi ... 6 = dimanche
    public List<int> Days { get; set; } = new();

    // "Matin", "Après-midi" ou "Soirée"
    public string Slot { get; set; } = "";

    public string Motivation { get; set; } = "";
    public bool CharterAccepted { get; set; }

    // "Soutien scolaire, Animation"
    public string DomainsText => string.Join(", ", Domains);

    // "Samedi, dimanche" : jours dans l'ordre de la semaine, premiere lettre en majuscule
    public string DaysText
    {
        get
        {
            string text = string.Join(", ", Days.OrderBy(d => d).Select(d => DayNames[d]));
            return text.Length == 0 ? "" : char.ToUpper(text[0]) + text.Substring(1);
        }
    }

    // Carte de l'etat d'echec : "Soutien scolaire" et "Centre d'Espoir - Samedi, dimanche - Matin"
    public string RecapTitle => DomainsText;
    public string RecapSubtitle => Orphanage.Name + " - " + DaysText + " - " + Slot;
}