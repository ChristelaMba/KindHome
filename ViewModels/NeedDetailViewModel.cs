using System.Windows.Input;
using KindHome.Models;
using KindHome.Views;

namespace KindHome.ViewModels;

public class NeedDetailViewModel : BaseViewModel
{
    Need need = new() { Orphanage = new Orphanage() };
    bool isFavorite;

    public NeedDetailViewModel()
    {
        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        ToggleFavoriteCommand = new Command(() => IsFavorite = !IsFavorite);
        DonateCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(DonationPage),
            new Dictionary<string, object> { { "Need", need } }));
        VolunteerCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(VolunteerPage),
            new Dictionary<string, object> { { "Need", need } }));
    }

    public ICommand BackCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }
    public ICommand DonateCommand { get; }
    public ICommand VolunteerCommand { get; }

    public Need Need
    {
        get => need;
        set
        {
            need = value;
            OnPropertyChanged(string.Empty);
        }
    }

    public bool IsFavorite
    {
        get => isFavorite;
        private set
        {
            if (!SetProperty(ref isFavorite, value)) return;
            OnPropertyChanged(nameof(HeartIcon));
        }
    }

    // Coeur contour ou plein rouge (en memoire seulement)
    public string HeartIcon => isFavorite ? "icon_heart_filled_22.png" : "icon_heart_outline_22.png";

    // "Centre d'Espoir - Bonapriso"
    public string OrphanageLine => need.Orphanage.Name + " - " + need.Orphanage.District;

    // Sans photo de couverture : fond uni (TileRose, TileBeige) pris dans Colors.xaml
    public bool HasNoCoverPhoto => !need.HasCoverPhoto;

    public Color CoverColor
    {
        get
        {
            if (Application.Current!.Resources.TryGetValue(need.CoverTile, out var value) && value is Color color)
                return color;
            return Colors.Transparent;
        }
    }
}
