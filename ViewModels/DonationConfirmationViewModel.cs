using System.Windows.Input;
using KindHome.Models;
using KindHome.Views;

namespace KindHome.ViewModels;

// Une ligne du recapitulatif : libelle, valeur et filet de separation
public class SummaryRow
{
    public SummaryRow(string label, string value, bool showDivider)
    {
        Label = label;
        Value = value;
        ShowDivider = showDivider;
    }

    public string Label { get; }
    public string Value { get; }
    public bool ShowDivider { get; }
}

public class DonationConfirmationViewModel : BaseViewModel
{
    Donation donation = new() { Need = new Need { Orphanage = new Orphanage() } };

    public DonationConfirmationViewModel()
    {
        HomeCommand = new Command(async () => await GoHomeAsync());
        ContactCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(ContactPage),
            new Dictionary<string, object> { { "Orphanage", donation.Need.Orphanage } }));
    }

    public ICommand HomeCommand { get; }
    public ICommand ContactCommand { get; }

    public Donation Donation
    {
        get => donation;
        set
        {
            donation = value;
            OnPropertyChanged(string.Empty);
        }
    }

    // "Votre proposition a bien ete envoyee au Centre d'Espoir."
    public string Subtitle => "Votre proposition a bien été envoyée " + donation.Need.Orphanage.ToName + ".";

    // Produit, Quantite (ou Montant), Orphelinat, Remise du don
    public List<SummaryRow> Rows => new()
    {
        new SummaryRow("Produit", donation.Need.Title, true),
        new SummaryRow(donation.AmountLabel, donation.AmountText, true),
        new SummaryRow("Orphelinat", donation.Need.Orphanage.Name, true),
        new SummaryRow("Remise du don", donation.DeliveryText, false)
    };

    // Retour a l'accueil en vidant la pile de pages
    public static async Task GoHomeAsync()
    {
        await Shell.Current.GoToAsync("//" + nameof(HomePage));
    }
}
