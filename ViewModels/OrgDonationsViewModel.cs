using System.Collections.ObjectModel;
using System.Windows.Input;
using KindHome.Models;

namespace KindHome.ViewModels;

public class OrgDonationsViewModel : BaseViewModel
{
    public OrgDonationsViewModel()
    {
        ConfirmCommand = new Command<OrgDonationItem>(item => { if (item != null) item.Status = "Remis"; });
        Refresh();
    }

    public ObservableCollection<OrgDonationItem> Items { get; } = new();

    public ICommand ConfirmCommand { get; }

    public bool HasItems => Items.Count > 0;
    public bool IsEmpty => Items.Count == 0;

    // A rappeler quand la page s'affiche : l'organisation connectee peut avoir change
    public void Refresh()
    {
        Items.Clear();
        foreach (var donation in UserSession.CurrentUser?.OrgDonations ?? new List<OrgDonation>())
            Items.Add(new OrgDonationItem(donation));
        OnPropertyChanged(nameof(HasItems));
        OnPropertyChanged(nameof(IsEmpty));
    }
}