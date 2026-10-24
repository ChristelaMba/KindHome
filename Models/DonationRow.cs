using SQLite;

namespace KindHome.Models;

// Une ligne de la table des dons dans SQLite
public class DonationRow
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // E-mail du donateur (vide si la personne est invitee)
    [Indexed]
    public string UserEmail { get; set; } = "";

    public int NeedId { get; set; }

    public string NeedTitle { get; set; } = "";

    public string OrphanageName { get; set; } = "";

    public bool IsFinancial { get; set; }

    public int Quantity { get; set; }

    public long Amount { get; set; }

    public bool OnSiteDelivery { get; set; }

    // Date souhaitee pour la remise du don
    public DateTime Date { get; set; }

    public string Message { get; set; } = "";

    // Moment de l'envoi
    public DateTime SentAt { get; set; }

    public string Status { get; set; } = "En attente";
}
