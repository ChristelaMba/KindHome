using SQLite;

namespace KindHome.Models;

// Une ligne de la table des messages de contact dans SQLite
public class ContactRow
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // E-mail de la personne qui ecrit (vide si elle est invitee)
    [Indexed]
    public string UserEmail { get; set; } = "";

    public string OrphanageName { get; set; } = "";

    public string Subject { get; set; } = "";

    public string Message { get; set; } = "";

    // Moment de l'envoi
    public DateTime SentAt { get; set; }
}