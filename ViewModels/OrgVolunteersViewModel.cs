using System.Collections.ObjectModel;
using System.Windows.Input;
using KindHome.Models;

namespace KindHome.ViewModels;

public class OrgVolunteersViewModel : BaseViewModel
{
    public OrgVolunteersViewModel()
    {
        AcceptCommand = new Command<OrgRequestItem>(item => { if (item != null) item.Status = "Acceptée"; });
        RefuseCommand = new Command<OrgRequestItem>(item => { if (item != null) item.Status = "Refusée"; });
        Refresh();
    }

    public ObservableCollection<OrgRequestItem> Items { get; } = new();

    public ICommand AcceptCommand { get; }
    public ICommand RefuseCommand { get; }

    public bool HasItems => Items.Count > 0;
    public bool IsEmpty => Items.Count == 0;

    // A rappeler quand la page s'affiche : l'organisation connectee peut avoir change
    public void Refresh()
    {
        Items.Clear();
        foreach (var request in UserSession.CurrentUser?.OrgRequests ?? new List<OrgRequest>())
            Items.Add(new OrgRequestItem(request));
        OnPropertyChanged(nameof(HasItems));
        OnPropertyChanged(nameof(IsEmpty));
    }
}