using System.Collections.ObjectModel;
using System.Windows.Input;
using KindHome.Models;

namespace KindHome.ViewModels;

public class NotificationsViewModel : BaseViewModel
{
    public NotificationsViewModel()
    {
        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        MarkAllCommand = new Command(MarkAll);
        MarkReadCommand = new Command<NotificationItem>(item => { if (item != null) item.IsRead = true; });

        // Interrupteur des reglages coupe : aucune notification montree
        var source = UserSession.NotificationsEnabled
            ? UserSession.CurrentUser?.Notifications ?? new List<AppNotification>()
            : new List<AppNotification>();
        Items = new ObservableCollection<NotificationItem>(source.Select(n => new NotificationItem(n)));

        if (!UserSession.NotificationsEnabled)
        {
            EmptyTitle = "Notifications désactivées";
            EmptyMessage = "Vous pouvez les réactiver dans les réglages.";
        }
    }

    public ObservableCollection<NotificationItem> Items { get; }

    public ICommand BackCommand { get; }
    public ICommand MarkAllCommand { get; }
    public ICommand MarkReadCommand { get; }

    public bool HasItems => Items.Count > 0;
    public bool IsEmpty => Items.Count == 0;

    public string EmptyTitle { get; } = "Aucune notification";
    public string EmptyMessage { get; } = "Vos notifications apparaîtront ici.";

    void MarkAll()
    {
        foreach (var item in Items)
            item.IsRead = true;
    }
}