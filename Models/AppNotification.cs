using KindHome.Services;

namespace KindHome.Models;

// Une notification affichee (exemple, ou lue dans la base quand DbId est superieur a 0)
public class AppNotification
{
    // Numero de la ligne dans la base (0 pour les notifications d'exemple)
    public int DbId { get; set; }

    public string Title { get; set; } = "";
    public string Text { get; set; } = "";

    // "Il y a 2 h"
    public string TimeText { get; set; } = "";

    // Nom d'une icone de Resources/Images
    public string Icon { get; set; } = "";

    bool isRead;

    // Quand une notification de la base passe a "lue", le changement est enregistre
    public bool IsRead
    {
        get => isRead;
        set
        {
            if (isRead == value) return;
            isRead = value;
            if (value && DbId > 0)
                NotificationService.MarkReadInBackground(DbId);
        }
    }
}
