using System.Collections.ObjectModel;
using System.Windows.Input;
using KindHome.Data;
using KindHome.Models;
using KindHome.Views;

namespace KindHome.ViewModels;

public class VolunteerViewModel : BaseViewModel
{
    const string SendText = "Envoyer ma candidature";
    const string RetryText = "Réessayer l'envoi";
    const string BusyText = "Envoi en cours";
    const int MaxMotivation = 300;

    Need need = new() { Orphanage = new Orphanage() };
    string motivation = "";
    bool charterAccepted;
    string domainsError = "";
    string daysError = "";
    string charterError = "";
    string buttonText = SendText;
    bool submitted;
    bool isBusy;
    bool showFailure;
    bool recorded;
    VolunteerRequest request = new();

    public VolunteerViewModel()
    {
        Domains = new ObservableCollection<ChipItem>(
            new[] { "Soutien scolaire", "Animation", "Santé", "Cuisine", "Autre" }
                .Select((name, i) => new ChipItem(name, i)));
        Days = new ObservableCollection<ChipItem>(
            new[] { "Lun", "Mar", "Mer", "Jeu", "Ven", "Sam", "Dim" }
                .Select((name, i) => new ChipItem(name, i)));
        Slots = new ObservableCollection<ChipItem>(
            new[] { "Matin", "Après-midi", "Soirée" }
                .Select((name, i) => new ChipItem(name, i)));

        // Valeurs de depart comme la maquette : Soutien scolaire, Sam et Dim, Matin
        Domains[0].IsSelected = true;
        Days[5].IsSelected = true;
        Days[6].IsSelected = true;
        Slots[0].IsSelected = true;

        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        ToggleDomainCommand = new Command<ChipItem>(item => Toggle(item));
        ToggleDayCommand = new Command<ChipItem>(item => Toggle(item));
        SelectSlotCommand = new Command<ChipItem>(SelectSlot);
        SubmitCommand = new Command(async () => await SubmitAsync());
    }

    public ObservableCollection<ChipItem> Domains { get; }
    public ObservableCollection<ChipItem> Days { get; }
    public ObservableCollection<ChipItem> Slots { get; }

    public ICommand BackCommand { get; }
    public ICommand ToggleDomainCommand { get; }
    public ICommand ToggleDayCommand { get; }
    public ICommand SelectSlotCommand { get; }
    public ICommand SubmitCommand { get; }

    public Need Need
    {
        get => need;
        set
        {
            need = value;
            OnPropertyChanged(string.Empty);
        }
    }

    // "Centre d'Espoir - Bonapriso"
    public string OrphanageLine => need.Orphanage.Name + " - " + need.Orphanage.District;

    public string Motivation
    {
        get => motivation;
        set
        {
            if (!SetProperty(ref motivation, value)) return;
            OnPropertyChanged(nameof(MotivationCounter));
        }
    }

    // "0 / 300"
    public string MotivationCounter => motivation.Length + " / " + MaxMotivation;

    public bool CharterAccepted
    {
        get => charterAccepted;
        set
        {
            if (SetProperty(ref charterAccepted, value) && submitted)
                ValidateCharter();
        }
    }

    public string DomainsError
    {
        get => domainsError;
        set
        {
            if (SetProperty(ref domainsError, value))
                OnPropertyChanged(nameof(HasDomainsError));
        }
    }

    public bool HasDomainsError => domainsError.Length > 0;

    public string DaysError
    {
        get => daysError;
        set
        {
            if (SetProperty(ref daysError, value))
                OnPropertyChanged(nameof(HasDaysError));
        }
    }

    public bool HasDaysError => daysError.Length > 0;

    public string CharterError
    {
        get => charterError;
        set => SetProperty(ref charterError, value);
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

    public VolunteerRequest Request
    {
        get => request;
        private set
        {
            request = value;
            OnPropertyChanged(nameof(Request));
        }
    }

    // Domaines et jours : choix multiple
    void Toggle(ChipItem? item)
    {
        if (item == null) return;
        item.IsSelected = !item.IsSelected;
        if (submitted)
        {
            ValidateDomains();
            ValidateDays();
        }
    }

    // Creneau : un seul choix
    void SelectSlot(ChipItem? item)
    {
        if (item == null) return;
        foreach (var slot in Slots)
            slot.IsSelected = slot == item;
    }

    // Message sous le groupe et puces en etat Erreur tant que rien n'est choisi
    bool ValidateDomains()
    {
        bool ok = Domains.Any(d => d.IsSelected);
        DomainsError = ok ? "" : "Choisissez au moins un domaine.";
        foreach (var chip in Domains)
            chip.IsError = !ok;
        return ok;
    }

    bool ValidateDays()
    {
        bool ok = Days.Any(d => d.IsSelected);
        DaysError = ok ? "" : "Choisissez au moins un jour.";
        foreach (var chip in Days)
            chip.IsError = !ok;
        return ok;
    }

    bool ValidateCharter()
    {
        CharterError = charterAccepted ? "" : "Ce champ est obligatoire.";
        return charterAccepted;
    }

    async Task SubmitAsync()
    {
        if (IsBusy) return;

        // Apres un echec, le formulaire n'est plus affiche : on renvoie la meme candidature
        if (!showFailure)
        {
            submitted = true;
            bool domainsOk = ValidateDomains();
            bool daysOk = ValidateDays();
            bool charterOk = ValidateCharter();
            if (!domainsOk || !daysOk || !charterOk) return;

            Request = new VolunteerRequest
            {
                Need = need,
                Domains = Domains.Where(d => d.IsSelected).Select(d => d.Name).ToList(),
                Days = Days.Where(d => d.IsSelected).Select(d => d.Index).ToList(),
                Slot = Slots.First(s => s.IsSelected).Name,
                Motivation = motivation,
                CharterAccepted = charterAccepted
            };
        }

        // Envoi simule : rien n'est envoye nulle part
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

        // La candidature rejoint l'historique de la session (une seule fois)
        if (!recorded)
        {
            recorded = true;
            UserSession.AddMission(request);
        }

        // On vide la pile de pages : le retour ne ramene pas au formulaire deja envoye.
        // Le bouton reste en "Envoi en cours" jusqu'a l'arrivee de la confirmation.
        try
        {
            await Shell.Current.GoToAsync("//" + nameof(HomePage) + "/" + nameof(VolunteerConfirmationPage),
                new Dictionary<string, object> { { "VolunteerRequest", request } });
        }
        catch (Exception)
        {
            IsBusy = false;
            ButtonText = showFailure ? RetryText : SendText;
        }
    }
}
