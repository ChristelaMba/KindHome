namespace KindHome.Models;

// Une notification d'exemple
public class AppNotification
{
    public string Title { get; set; } = "";
    public string Text { get; set; } = "";

    // "Il y a 2 h"
    public string TimeText { get; set; } = "";

    // Nom d'une icone de Resources/Images
    public string Icon { get; set; } = "";
    public bool IsRead { get; set; }
}