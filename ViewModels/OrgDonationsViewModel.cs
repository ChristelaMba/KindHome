using System.Collections.ObjectModel;
using System.Windows.Input;
using KindHome.Models;
using KindHome.Services;

namespace KindHome.ViewModels;

public class OrgDonationsViewModel : BaseViewModel
{
    public OrgDonationsViewModel()
    {
        ConfirmCommand = new Command<OrgDonationItem>(async item => await ConfirmAsync(item));
    }

    public ObservableCollection<OrgDonationItem> Items { get; } = new();

    public ICommand ConfirmCommand { get; }

    public bool HasItems => Items.Count > 0;
    public bool IsEmpty => Items.Count == 0;

    // A rappeler quand la page s'affiche : dons de la base, puis donnees d'exemple du compte
    public async Task RefreshAsync()
    {
        var user = UserSession.CurrentUser;
        var list = new List<OrgDonation>();

        if (user != null && user.Role == UserRole.Organization)
        {
            try
            {
                list.AddRange(await OrgService.GetDonationsAsync(user.FullName));
            }
            catch (Exception)
            {
                // Lecture impossible : on garde seulement les donnees d'exemple
            }
        }

        // La personne connectee a change pendant la lecture : on ignore le resultat
        if (UserSession.CurrentUser != user) return;

        list.AddRange(user?.OrgDonations ?? new List<OrgDonation>());

        Items.Clear();
        foreach (var donation in list)
            Items.Add(new OrgDonationItem(donation));

        OnPropertyChanged(nameof(HasItems));
        OnPropertyChanged(nameof(IsEmpty));
    }

    // La remise est enregistree dans la base ; en cas d'echec on revient en arriere
    async Task ConfirmAsync(OrgDonationItem? item)
    {
        if (item == null) return;

        string oldStatus = item.Status;
        item.Status = "Remis";

        // Les dons d'exemple (DbId = 0) ne sont pas dans la base
        if (item.Donation.DbId <= 0) return;

        try
        {
            await OrgService.SetDonationStatusAsync(item.Donation.DbId, "Remis");
        }
        catch (Exception)
        {
            item.Status = oldStatus;
            await Shell.Current.DisplayAlertAsync("Erreur", "Le changement n'a pas pu être enregistré.", "OK");
        }
    }
}