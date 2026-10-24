using SQLite;

namespace KindHome.Models;

// Une ligne de la table des candidatures de benevoles dans SQLite
public class VolunteerRow
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // E-mail et nom du candidat (vides si la personne est invitee)
    [Indexed]
    public string UserEmail { get; set; } = "";

    public string UserName { get; set; } = "";

    public int NeedId { get; set; }

    public string NeedTitle { get; set; } = "";

    public string OrphanageName { get; set; } = "";

    // Domaines choisis, separes par |  (exemple : Soutien scolaire|Animation)
    public string Domains { get; set; } = "";

    // Jours choisis, separes par une virgule : 0 = lundi ... 6 = dimanche (exemple : 5,6)
    public string Days { get; set; } = "";

    public string Slot { get; set; } = "";

    public string Motivation { get; set; } = "";

    public bool CharterAccepted { get; set; }

    // Moment de l'envoi
    public DateTime SentAt { get; set; }

    // "En attente", "Acceptée" ou "Refusée"
    public string Status { get; set; } = "En attente";
}