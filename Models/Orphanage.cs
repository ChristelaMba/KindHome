using System.Globalization;

namespace KindHome.Models;

public class Orphanage
{
    static readonly CultureInfo French = new("fr-FR");

    public string Name { get; set; } = "";
    public string District { get; set; } = "";
    public double DistanceKm { get; set; }
    public int ChildrenCount { get; set; }
    public List<string> Categories { get; set; } = new();
    public bool IsVerified { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public string About { get; set; } = "";

    // Noms des fichiers dans Resources/Images
    public string Photo { get; set; } = "";
    public string CoverPhoto { get; set; } = "";

    // Besoins de cet orphelinat (remplis dans MockData)
    public List<Need> Needs { get; set; } = new();

    // Actualites affichees dans l'onglet "Actualites" de la fiche
    public List<NewsItem> News { get; set; } = new();

    // Coordonnees d'exemple (approximatives, selon le quartier)
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Position du repere sur la carte dessinee : valeurs "translate" du HTML (carte de 390 x 252)
    public double MapX { get; set; }
    public double MapY { get; set; }

    // "3,2 km"
    public string DistanceText => DistanceKm.ToString("0.0", French) + " km";

    // "Bonapriso - 3,2 km"
    public string LocationText => District + " - " + DistanceText;

    // "du Centre d'Espoir", "de la Maison des Anges", "des Petits Cœurs" : le nom precede de "de", avec son article
    public string OfName => WithArticle("du", "de la", "des", "de", "d'");

    // "au Centre d'Espoir", "a la Maison des Anges", "aux Petits Cœurs"
    public string ToName => WithArticle("au", "à la", "aux", "à", "à ");

    // "Le Centre d'Espoir", "La Maison des Anges", "Les Petits Cœurs" ; "Espoir de Demain" reste tel quel
    public string TheName
    {
        get
        {
            if (Name.StartsWith("Centre")) return "Le " + Name;
            if (Name.StartsWith("Maison")) return "La " + Name;
            return Name;
        }
    }

    // "Les Petits Cœurs" : le verbe est au pluriel
    public bool HasPluralName => Name.StartsWith("Les ");

    string WithArticle(string masculine, string feminine, string plural, string plain, string vowel)
    {
        if (Name.Length == 0) return "";
        if (Name.StartsWith("Les ")) return plural + " " + Name.Substring(4);
        if (Name.StartsWith("La ")) return feminine + " " + Name.Substring(3);
        if (Name.StartsWith("Le ")) return masculine + " " + Name.Substring(3);
        if (Name.StartsWith("Centre")) return masculine + " " + Name;
        if (Name.StartsWith("Maison")) return feminine + " " + Name;
        if ("AEIOUHÉ".Contains(char.ToUpper(Name[0]))) return vowel + Name;
        return plain + " " + Name;
    }

    // "32 enfants"
    public string ChildrenText => ChildrenCount + " enfants";

    // "4,6"
    public string RatingText => Rating.ToString("0.0", French);

    // "12 avis"
    public string ReviewsText => ReviewCount + " avis";
}
