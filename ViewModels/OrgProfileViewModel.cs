using System.Windows.Input;
using KindHome.Models;
using KindHome.Views;

namespace KindHome.ViewModels;

public class OrgProfileViewModel : BaseViewModel
{
    string name = "";
    int children;
    int volunteers;
    int donationsThisMonth;

    public OrgProfileViewModel()
    {
        SettingsCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(SettingsPage)));
        NotificationsCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(NotificationsPage)));
        Refresh();
    }

    public ICommand SettingsCommand { get; }
    public ICommand NotificationsCommand { get; }

    public string Name { get => name; private set => SetProperty(ref name, value); }
    public int Children { get => children; private set => SetProperty(ref children, value); }
    public int Volunteers { get => volunteers; private set => SetProperty(ref volunteers, value); }
    public int DonationsThisMonth { get => donationsThisMonth; private set => SetProperty(ref donationsThisMonth, value); }

    // A rappeler quand la page s'affiche : l'organisation connectee peut avoir change
    public void Refresh()
    {
        var user = UserSession.CurrentUser;
        var summary = user?.Organization;
        Name = user?.FullName ?? "";

        // Organisation creee a l'inscription : tout est a 0
        Children = summary?.Children ?? 0;
        Volunteers = summary?.Volunteers ?? 0;
        DonationsThisMonth = summary?.DonationsThisMonth ?? 0;
    }
}