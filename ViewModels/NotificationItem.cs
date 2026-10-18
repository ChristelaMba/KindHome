using KindHome.Models;

namespace KindHome.ViewModels;

// Une notification affichee : lire la carte la marque comme lue (l'etat reste dans le modele)
public class NotificationItem : BaseViewModel
{
    public NotificationItem(AppNotification notification)
    {
        Notification = notification;
    }

    public AppNotification Notification { get; }

    public string Title => Notification.Title;
    public string Text => Notification.Text;
    public string TimeText => Notification.TimeText;
    public string Icon => Notification.Icon;

    public bool IsRead
    {
        get => Notification.IsRead;
        set
        {
            if (Notification.IsRead == value) return;
            Notification.IsRead = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsUnread));
        }
    }

    public bool IsUnread => !Notification.IsRead;
}