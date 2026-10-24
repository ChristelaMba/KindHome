using SQLite;

namespace KindHome.Models;

// Une ligne de la table des notifications dans SQLite
public class NotificationRow
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // E-mail de la personne qui recoit la notification
    [Indexed]
    public string UserEmail { get; set; } = "";

    public string Title { get; set; } = "";

    public string Text { get; set; } = "";

    // Nom d'une icone de Resources/Images, par exemple icon_check_circle.png
    public string Icon { get; set; } = "";

    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; }
}
