using System.Windows.Input;
using KindHome.Models;
using KindHome.Views;

namespace KindHome.ViewModels;

public class VolunteerConfirmationViewModel : BaseViewModel
{
    VolunteerRequest request = new() { Need = new Need { Orphanage = new Orphanage() } };

    public VolunteerConfirmationViewModel()
    {
        HomeCommand = new Command(async () => await GoHomeAsync());
        MissionsCommand = new Command(async () => await Shell.Current.GoToAsync("//" + nameof(HomePage) + "/" + nameof(HistoryPage),
            new Dictionary<string, object> { { "Tab", "Missions" } }));
    }

    public ICommand HomeCommand { get; }
    public ICommand MissionsCommand { get; }

    public VolunteerRequest Request
    {
        get => request;
        set
        {
            request = value;
            OnPropertyChanged(string.Empty);
        }
    }

    // "Merci pour votre engagement aupres du Centre d'Espoir."
    public string Subtitle => "Merci pour votre engagement auprès " + request.Orphanage.OfName + ".";

    // Orphelinat, Domaine d'aide, Jours, Creneau
    public List<SummaryRow> Rows => new()
    {
        new SummaryRow("Orphelinat", request.Orphanage.Name, true),
        new SummaryRow("Domaine d'aide", request.DomainsText, true),
        new SummaryRow("Jours", request.DaysText, true),
        new SummaryRow("Créneau", request.Slot, false)
    };

    // Retour a l'accueil en vidant la pile de pages
    static async Task GoHomeAsync()
    {
        await Shell.Current.GoToAsync("//" + nameof(HomePage));
    }
}
