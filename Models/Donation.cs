using System.Globalization;

namespace KindHome.Models;

// Le don en cours de saisie, puis envoye a la page de confirmation
public class Donation
{
    static readonly CultureInfo French = new("fr-FR");

    // Besoin vide tant que le don n'est pas rempli (evite les erreurs a l'ouverture du formulaire)
    public Need Need { get; set; } = new() { Orphanage = new Orphanage() };

    // false : don en nature (Quantity) ; true : don financier (Amount en FCFA)
    public bool IsFinancial { get; set; }
    public int Quantity { get; set; }
    public long Amount { get; set; }

    // true : "Je depose sur place" ; false : "L'orphelinat vient recuperer"
    public bool OnSiteDelivery { get; set; } = true;

    public DateTime Date { get; set; }
    public string Message { get; set; } = "";

    // "Quantité" ou "Montant"
    public string AmountLabel => IsFinancial ? "Montant" : "Quantité";

    // "4 unités" ou "10 000 FCFA"
    public string AmountText => IsFinancial
        ? Amount.ToString("N0", French).Replace('\u202f', ' ').Replace('\u00a0', ' ') + " FCFA"
        : Quantity + " " + Need.Unit;

    // "Dépôt sur place" ou "Récupération par l'orphelinat"
    public string DeliveryText => OnSiteDelivery ? "Dépôt sur place" : "Récupération par l'orphelinat";

    // Carte de l'etat d'echec : "Lait en poudre : 4 unités" et "Centre d'Espoir - dépôt sur place"
    public string RecapTitle => Need.Title + " : " + AmountText;
    public string RecapSubtitle => Need.Orphanage.Name + " - " + (OnSiteDelivery ? "dépôt sur place" : "récupération par l'orphelinat");
}