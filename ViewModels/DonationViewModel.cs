using System.Windows.Input;
using KindHome.Data;
using KindHome.Models;
using KindHome.Services;
using KindHome.Views;

namespace KindHome.ViewModels;

public class DonationViewModel : BaseViewModel
{
    const string SendText = "Proposer ce don";
    const string RetryText = "Réessayer l'envoi";
    const string BusyText = "Envoi en cours";
    const int MinimumAmount = 500;

    Need need = new() { Orphanage = new Orphanage() };
    bool isFinancial;
    int quantity = 1;
    bool onSiteDelivery = true;
    DateTime? dateValue;
    string dateError = "";
    string amountText = "";
    string amountError = "";
    string message = "";
    string buttonText = SendText;
    bool submitted;
    bool isBusy;
    bool showFailure;
    bool recorded;
    Donation donation = new();

    public DonationViewModel()
    {
        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        SelectInKindCommand = new Command(() => IsFinancial = false);
        SelectFinancialCommand = new Command(() => IsFinancial = true);
        DecreaseCommand = new Command(() => Quantity--);
        IncreaseCommand = new Command(() => Quantity++);
        SelectOnSiteCommand = new Command(() => OnSiteDelivery = true);
        SelectPickupCommand = new Command(() => OnSiteDelivery = false);
        SubmitCommand = new Command(async () => await SubmitAsync());
    }

    public ICommand BackCommand { get; }
    public ICommand SelectInKindCommand { get; }
    public ICommand SelectFinancialCommand { get; }
    public ICommand DecreaseCommand { get; }
    public ICommand IncreaseCommand { get; }
    public ICommand SelectOnSiteCommand { get; }
    public ICommand SelectPickupCommand { get; }
    public ICommand SubmitCommand { get; }

    public Need Need
    {
        get => need;
        set
        {
            need = value;
            // Valeur de depart : 4, ou le reste a couvrir s'il est plus petit
            quantity = Math.Max(1, Math.Min(4, need.Remaining));
            OnPropertyChanged(string.Empty);
        }
    }

    // "Centre d'Espoir - Bonapriso"
    public string OrphanageLine => need.Orphanage.Name + " - " + need.Orphanage.District;

    // "unites (reste 8 a couvrir)"
    public string QuantityUnitText => need.Unit + " (reste " + need.Remaining + " à couvrir)";

    public bool IsFinancial
    {
        get => isFinancial;
        set
        {
            if (!SetProperty(ref isFinancial, value)) return;
            OnPropertyChanged(nameof(IsInKind));
            // Le message du montant n'apparait qu'a l'appui sur le bouton d'envoi
            AmountError = "";
        }
    }

    public bool IsInKind => !isFinancial;

    // Entre 1 et le reste a couvrir
    public int Quantity
    {
        get => quantity;
        set
        {
            int max = Math.Max(1, need.Remaining);
            value = Math.Clamp(value, 1, max);
            if (!SetProperty(ref quantity, value)) return;
            OnPropertyChanged(nameof(CanDecrease));
            OnPropertyChanged(nameof(CanIncrease));
        }
    }

    public bool CanDecrease => quantity > 1;
    public bool CanIncrease => quantity < Math.Max(1, need.Remaining);

    public bool OnSiteDelivery
    {
        get => onSiteDelivery;
        set
        {
            if (!SetProperty(ref onSiteDelivery, value)) return;
            OnPropertyChanged(nameof(IsPickup));
        }
    }

    public bool IsPickup => !onSiteDelivery;

    public DateTime? DateValue
    {
        get => dateValue;
        set
        {
            if (SetProperty(ref dateValue, value) && submitted)
                ValidateDate();
        }
    }

    public string DateError
    {
        get => dateError;
        set => SetProperty(ref dateError, value);
    }

    public string AmountText
    {
        get => amountText;
        set
        {
            if (SetProperty(ref amountText, value) && submitted)
                ValidateAmount();
        }
    }

    public string AmountError
    {
        get => amountError;
        set => SetProperty(ref amountError, value);
    }

    public string Message
    {
        get => message;
        set => SetProperty(ref message, value);
    }

    public string ButtonText
    {
        get => buttonText;
        set => SetProperty(ref buttonText, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        set => SetProperty(ref isBusy, value);
    }

    // Apres un echec : bandeau, recapitulatif et bouton "Reessayer l'envoi" remplacent le formulaire
    public bool ShowFailure
    {
        get => showFailure;
        set
        {
            if (!SetProperty(ref showFailure, value)) return;
            OnPropertyChanged(nameof(ShowForm));
        }
    }

    public bool ShowForm => !showFailure;

    public Donation Donation
    {
        get => donation;
        private set
        {
            donation = value;
            OnPropertyChanged(nameof(Donation));
        }
    }

    bool ValidateDate()
    {
        DateError = dateValue == null ? "Date souhaitée obligatoire" : "";
        return DateError.Length == 0;
    }

    // Montant en chiffres (les espaces sont ignores) : obligatoire, 500 FCFA minimum
    bool ValidateAmount()
    {
        if (!isFinancial)
        {
            AmountError = "";
            return true;
        }

        string digits = new string(amountText.Where(char.IsDigit).ToArray());
        if (digits.Length == 0)
            AmountError = "Montant obligatoire";
        else if (!long.TryParse(digits, out long amount) || amount < MinimumAmount)
            AmountError = "Montant minimum : " + MinimumAmount + " FCFA";
        else
            AmountError = "";
        return AmountError.Length == 0;
    }

    long ParsedAmount()
    {
        string digits = new string(amountText.Where(char.IsDigit).ToArray());
        return long.TryParse(digits, out long amount) ? amount : 0;
    }

    async Task SubmitAsync()
    {
        if (IsBusy) return;

        // Apres un echec, le formulaire n'est plus affiche : on renvoie le meme don
        if (!showFailure)
        {
            submitted = true;
            bool dateOk = ValidateDate();
            bool amountOk = ValidateAmount();
            if (!dateOk || !amountOk) return;

            Donation = new Donation
            {
                Need = need,
                IsFinancial = isFinancial,
                Quantity = quantity,
                Amount = ParsedAmount(),
                OnSiteDelivery = onSiteDelivery,
                Date = dateValue!.Value,
                Message = message
            };
        }

        IsBusy = true;
        ButtonText = BusyText;
        await Task.Delay(1000);

        if (DebugFlags.SimulateSendFailure)
        {
            IsBusy = false;
            ShowFailure = true;
            ButtonText = RetryText;
            return;
        }

        // Le don est enregistre dans la base, puis rejoint l'historique de la session (une seule fois)
        if (!recorded)
        {
            try
            {
                await DonationService.SaveAsync(donation, UserSession.CurrentUser?.Email ?? "");
            }
            catch (Exception)
            {
                // Echec de l'enregistrement : on affiche l'etat d'echec existant
                IsBusy = false;
                ShowFailure = true;
                ButtonText = RetryText;
                return;
            }

            recorded = true;
            UserSession.AddDonation(donation);
        }

        // On vide la pile de pages : le retour ne ramene pas au formulaire deja envoye.
        // Le bouton reste en "Envoi en cours" jusqu'a l'arrivee de la confirmation.
        try
        {
            await Shell.Current.GoToAsync("//" + nameof(HomePage) + "/" + nameof(DonationConfirmationPage),
                new Dictionary<string, object> { { "Donation", donation } });
        }
        catch (Exception)
        {
            IsBusy = false;
            ButtonText = showFailure ? RetryText : SendText;
        }
    }
}