using System.Windows.Input;
using KindHome.Models;
using KindHome.Views;

namespace KindHome.ViewModels;

public class OrphanageDetailViewModel : BaseViewModel
{
    const string TabAbout = "About";
    const string TabNeeds = "Needs";
    const string TabNews = "News";

    Orphanage orphanage = new();
    string tab = TabAbout;
    bool isFavorite;

    public OrphanageDetailViewModel()
    {
        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        ToggleFavoriteCommand = new Command(() => IsFavorite = !IsFavorite);
        ShareCommand = new Command(async () => await Notice.ShowNotAvailableAsync());
        SelectTabCommand = new Command<string>(t => Tab = t);
        MessageCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(ContactPage),
            new Dictionary<string, object> { { "Orphanage", orphanage } }));
        SupportCommand = new Command(async () => await SupportAsync());
        OpenNeedCommand = new Command<Need>(async need => await OpenNeedAsync(need));
    }

    public ICommand BackCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }
    public ICommand ShareCommand { get; }
    public ICommand SelectTabCommand { get; }
    public ICommand MessageCommand { get; }
    public ICommand SupportCommand { get; }
    public ICommand OpenNeedCommand { get; }

    public Orphanage Orphanage
    {
        get => orphanage;
        set
        {
            orphanage = value;
            OnPropertyChanged(string.Empty);
        }
    }

    public string Tab
    {
        get => tab;
        set
        {
            if (!SetProperty(ref tab, value)) return;
            OnPropertyChanged(nameof(IsAboutTab));
            OnPropertyChanged(nameof(IsNeedsTab));
            OnPropertyChanged(nameof(IsNewsTab));
            OnPropertyChanged(nameof(ShowNeedsList));
            OnPropertyChanged(nameof(ShowNeedsEmpty));
        }
    }

    public bool IsAboutTab => tab == TabAbout;
    public bool IsNeedsTab => tab == TabNeeds;
    public bool IsNewsTab => tab == TabNews;

    public bool IsFavorite
    {
        get => isFavorite;
        private set
        {
            if (!SetProperty(ref isFavorite, value)) return;
            OnPropertyChanged(nameof(HeartIcon));
        }
    }

    // Coeur contour ou plein (en memoire seulement)
    public string HeartIcon => isFavorite ? "icon_heart_filled_22.png" : "icon_heart_outline_22.png";

    public string LocationLine => orphanage.District + ", Douala";
    public string ReviewsText => "(" + orphanage.ReviewsText + ")";

    public bool ShowNeedsList => IsNeedsTab && orphanage.Needs.Count > 0;
    public bool ShowNeedsEmpty => IsNeedsTab && orphanage.Needs.Count == 0;

    // "Soutenir cet orphelinat" : ouvre le don pour le premier besoin, ou bascule sur l'onglet Besoins (etat vide) s'il n'y en a pas
    async Task SupportAsync()
    {
        var first = orphanage.Needs.FirstOrDefault();
        if (first == null)
        {
            Tab = TabNeeds;
            return;
        }
        await Shell.Current.GoToAsync(nameof(DonationPage), new Dictionary<string, object> { { "Need", first } });
    }

    static async Task OpenNeedAsync(Need? need)
    {
        if (need == null) return;
        await Shell.Current.GoToAsync(nameof(NeedDetailPage), new Dictionary<string, object> { { "Need", need } });
    }
}