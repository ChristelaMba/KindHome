using KindHome.Models;

namespace KindHome.ViewModels;

// Un don recu affiche : le statut passe a "Remis" quand l'organisation confirme la remise
public class OrgDonationItem : BaseViewModel
{
    public OrgDonationItem(OrgDonation donation)
    {
        Donation = donation;
    }

    public OrgDonation Donation { get; }

    public Need Need => Donation.Need;
    public string Title => Donation.Title;
    public string Donor => Donation.Donor;
    public string Date => Donation.Date;

    public string Status
    {
        get => Donation.Status;
        set
        {
            if (Donation.Status == value) return;
            Donation.Status = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(NeedsConfirmation));
        }
    }

    public bool NeedsConfirmation => Donation.Status == "À confirmer";
}