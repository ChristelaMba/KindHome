using System.Collections.ObjectModel;
using System.Windows.Input;
using KindHome.Models;
using KindHome.Views;

namespace KindHome.ViewModels;

public class OrgNeedsViewModel : BaseViewModel
{
    public OrgNeedsViewModel()
    {
        PublishCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(PublishNeedPage)));
        Refresh();
    }

    public ObservableCollection<Need> Items { get; } = new();

    public ICommand PublishCommand { get; }

    public bool HasItems => Items.Count > 0;
    public bool IsEmpty => Items.Count == 0;

    // A rappeler quand la page s'affiche : un besoin a pu etre publie
    public void Refresh()
    {
        Items.Clear();
        foreach (var need in UserSession.OrgNeeds())
            Items.Add(need);
        OnPropertyChanged(nameof(HasItems));
        OnPropertyChanged(nameof(IsEmpty));
    }
}