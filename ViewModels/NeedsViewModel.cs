using System.Collections.ObjectModel;
using System.Windows.Input;
using KindHome.Data;
using KindHome.Models;
using KindHome.Views;

namespace KindHome.ViewModels;

public class NeedsViewModel : BaseViewModel
{
    const string AllName = "Tous";

    NeedFilter selected;
    bool isNetworkError;

    public NeedsViewModel()
    {
        Filters = new ObservableCollection<NeedFilter>(
            new[] { AllName, "Alimentation", "Scolarité", "Santé", "Vêtements", "Financier" }
                .Select(name => new NeedFilter(name)));
        selected = Filters[0];
        selected.IsSelected = true;

        Needs = new ObservableCollection<Need>(MockData.Needs);
        isNetworkError = DebugFlags.SimulateNetworkError;

        BackCommand = new Command(async () => await Shell.Current.GoToAsync("//" + nameof(HomePage)));
        SelectFilterCommand = new Command<NeedFilter>(Select);
        ResetFiltersCommand = new Command(() => Select(Filters[0]));
        OpenNeedCommand = new Command<Need>(async need => await OpenNeedAsync(need));
        RetryCommand = new Command(() => IsNetworkError = DebugFlags.SimulateNetworkError);
        ShowSavedCommand = new Command(() => IsNetworkError = false);
    }

    public ObservableCollection<NeedFilter> Filters { get; }
    public ObservableCollection<Need> Needs { get; }

    public ICommand BackCommand { get; }
    public ICommand SelectFilterCommand { get; }
    public ICommand ResetFiltersCommand { get; }
    public ICommand OpenNeedCommand { get; }
    public ICommand RetryCommand { get; }
    public ICommand ShowSavedCommand { get; }

    public bool IsNetworkError
    {
        get => isNetworkError;
        private set
        {
            if (!SetProperty(ref isNetworkError, value)) return;
            RefreshStates();
        }
    }

    // Etats de la page (un seul bloc visible a la fois)
    public bool ShowMain => !isNetworkError;
    public bool ShowList => ShowMain && Needs.Count > 0;
    public bool ShowEmpty => ShowMain && Needs.Count == 0;

    void RefreshStates()
    {
        OnPropertyChanged(nameof(ShowMain));
        OnPropertyChanged(nameof(ShowList));
        OnPropertyChanged(nameof(ShowEmpty));
    }

    // Une seule puce a la fois ; "Tous" montre tout, les autres filtrent par categorie
    void Select(NeedFilter? filter)
    {
        if (filter == null) return;

        selected.IsSelected = false;
        selected = filter;
        selected.IsSelected = true;

        Needs.Clear();
        foreach (var need in MockData.Needs.Where(n => filter.Name == AllName || n.Category == filter.Name))
            Needs.Add(need);
        RefreshStates();
    }

    static async Task OpenNeedAsync(Need? need)
    {
        if (need == null) return;
        await Shell.Current.GoToAsync(nameof(NeedDetailPage), new Dictionary<string, object> { { "Need", need } });
    }
}
