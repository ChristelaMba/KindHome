using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using KindHome.Data;
using KindHome.Models;
using KindHome.Views;

namespace KindHome.ViewModels;

public class DirectoryViewModel : BaseViewModel
{
    string searchText = "";
    bool isLoading = true;
    bool isLocationDenied;
    bool isMapMode;
    DirectoryItem? selected;

    readonly List<DirectoryItem> allItems;

    public DirectoryViewModel()
    {
        allItems = MockData.Orphanages.Select(o => new DirectoryItem(o)).ToList();
        Results = new ObservableCollection<DirectoryItem>(allItems);
        MarkLast();
        isLocationDenied = DebugFlags.SimulateLocationDenied;

        BackCommand = new Command(async () => await Shell.Current.GoToAsync("//" + nameof(HomePage)));
        OpenCommand = new Command<Orphanage>(async o => await SelectAndOpenAsync(o));
        ShowListCommand = new Command(() => IsMapMode = false);
        ShowMapCommand = new Command(() => IsMapMode = true);
        ClearSearchCommand = new Command(() => SearchText = "");
        UseCityCommand = new Command(() => IsLocationDenied = false);
        LocateCommand = new Command(Recenter);
    }

    public ObservableCollection<DirectoryItem> Results { get; }

    public ICommand BackCommand { get; }
    public ICommand OpenCommand { get; }
    public ICommand ShowListCommand { get; }
    public ICommand ShowMapCommand { get; }
    public ICommand ClearSearchCommand { get; }
    public ICommand UseCityCommand { get; }
    public ICommand LocateCommand { get; }

    // Prevenu quand les resultats changent (la page met a jour les reperes)
    public event EventHandler? ResultsChanged;

    // Prevenu quand un repere est choisi (la page fait defiler la liste)
    public event EventHandler<DirectoryItem>? ItemSelected;

    // Prevenu quand le bouton de position est appuye
    public event EventHandler? RecenterRequested;

    public string SearchText
    {
        get => searchText;
        set
        {
            if (!SetProperty(ref searchText, value)) return;
            ApplyFilter();
        }
    }

    public bool IsLoading
    {
        get => isLoading;
        private set
        {
            if (!SetProperty(ref isLoading, value)) return;
            RefreshStates();
        }
    }

    public bool IsLocationDenied
    {
        get => isLocationDenied;
        private set
        {
            if (!SetProperty(ref isLocationDenied, value)) return;
            RefreshStates();
        }
    }

    public bool IsMapMode
    {
        get => isMapMode;
        private set
        {
            if (!SetProperty(ref isMapMode, value)) return;
            OnPropertyChanged(nameof(IsListMode));
            RefreshStates();
        }
    }

    public bool IsListMode => !isMapMode;

    public bool HasResults => Results.Count > 0;

    // Etats de la page (un seul bloc visible a la fois)
    public bool ShowLoading => isLoading && !isLocationDenied;
    public bool ShowDenied => isLocationDenied;
    public bool ShowMain => !isLoading && !isLocationDenied;
    public bool ShowEmpty => ShowMain && !HasResults;
    public bool ShowMap => ShowMain && HasResults;
    public bool ShowList => ShowMap && !isMapMode;

    // Chargement simule de la carte : environ une seconde
    public async Task LoadAsync()
    {
        await Task.Delay(1000);
        IsLoading = false;
    }

    void RefreshStates()
    {
        OnPropertyChanged(nameof(ShowLoading));
        OnPropertyChanged(nameof(ShowDenied));
        OnPropertyChanged(nameof(ShowMain));
        OnPropertyChanged(nameof(ShowEmpty));
        OnPropertyChanged(nameof(ShowMap));
        OnPropertyChanged(nameof(ShowList));
    }

    static bool Contains(string text, string part)
    {
        return CultureInfo.InvariantCulture.CompareInfo.IndexOf(
            text, part, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;
    }

    // Nom ou quartier, pendant la saisie
    void ApplyFilter()
    {
        string part = searchText.Trim();
        var matches = allItems
            .Where(i => part.Length == 0 || Contains(i.Orphanage.Name, part) || Contains(i.Orphanage.District, part))
            .ToList();

        Results.Clear();
        foreach (var item in matches)
            Results.Add(item);
        MarkLast();

        if (selected != null && !Results.Contains(selected))
            Select(null);

        OnPropertyChanged(nameof(HasResults));
        RefreshStates();
        ResultsChanged?.Invoke(this, EventArgs.Empty);
    }

    // Le dernier de la liste n'a pas de filet en dessous
    void MarkLast()
    {
        for (int i = 0; i < Results.Count; i++)
            Results[i].IsLast = i == Results.Count - 1;
    }

    void Select(DirectoryItem? item)
    {
        if (selected != null) selected.IsSelected = false;
        selected = item;
        if (selected != null) selected.IsSelected = true;
    }

    public DirectoryItem? Selected => selected;

    // Bouton de position : retire la selection, puis la page remet la carte dans son etat de depart
    void Recenter()
    {
        Select(null);
        RecenterRequested?.Invoke(this, EventArgs.Empty);
    }

    // Un repere ou une ligne : choisit l'orphelinat (repere mis en evidence, ligne visible) puis ouvre sa fiche
    async Task SelectAndOpenAsync(Orphanage? orphanage)
    {
        if (orphanage == null) return;

        var item = allItems.FirstOrDefault(i => i.Orphanage == orphanage);
        Select(item);
        if (item != null)
            ItemSelected?.Invoke(this, item);

        await Shell.Current.GoToAsync(nameof(OrphanageDetailPage), new Dictionary<string, object> { { "Orphanage", orphanage } });
    }
}