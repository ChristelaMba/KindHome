using System.Windows.Input;
using KindHome.Data;
using KindHome.Models;
using KindHome.Services;
using KindHome.Views;

namespace KindHome.ViewModels;

public class ContactViewModel : BaseViewModel
{
    const string SendText = "Envoyer le message";
    const string RetryText = "Réessayer l'envoi";
    const string BusyText = "Envoi en cours";
    const int MaxMessage = 500;

    Orphanage orphanage = new();
    string subject = "Question générale";
    string message = "";
    string messageError = "";
    string buttonText = SendText;
    bool submitted;
    bool isBusy;
    bool showFailure;

    public ContactViewModel()
    {
        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        CallCommand = new Command(async () => await DeviceActions.CallAsync(orphanage));
        EmailCommand = new Command(async () => await DeviceActions.EmailAsync(orphanage, subject));
        RouteCommand = new Command(async () => await DeviceActions.RouteAsync(orphanage));
        SendCommand = new Command(async () => await SendAsync());
    }

    public ICommand BackCommand { get; }
    public ICommand CallCommand { get; }
    public ICommand EmailCommand { get; }
    public ICommand RouteCommand { get; }
    public ICommand SendCommand { get; }

    // Liste de l'objet : les 3 choix du HTML de la maquette
    public List<string> Subjects { get; } = new()
    {
        "Question générale", "Un don", "Du bénévolat"
    };

    public Orphanage Orphanage
    {
        get => orphanage;
        set
        {
            orphanage = value;
            OnPropertyChanged(string.Empty);
        }
    }

    // "Bonapriso, Douala - 3,2 km"
    public string LocationLine => orphanage.District + ", Douala - " + orphanage.DistanceText;

    public string Subject
    {
        get => subject;
        set => SetProperty(ref subject, value);
    }

    public string Message
    {
        get => message;
        set
        {
            if (!SetProperty(ref message, value)) return;
            OnPropertyChanged(nameof(MessageCounter));
            if (submitted)
                ValidateMessage();
        }
    }

    // "0 / 500"
    public string MessageCounter => message.Length + " / " + MaxMessage;

    public string MessageError
    {
        get => messageError;
        set
        {
            if (!SetProperty(ref messageError, value)) return;
            OnPropertyChanged(nameof(HasMessageError));
        }
    }

    public bool HasMessageError => messageError.Length > 0;

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

    // Bandeau rouge en haut apres un echec d'envoi
    public bool ShowFailure
    {
        get => showFailure;
        set => SetProperty(ref showFailure, value);
    }

    bool ValidateMessage()
    {
        MessageError = message.Trim().Length == 0 ? "Message obligatoire" : "";
        return MessageError.Length == 0;
    }

    async Task SendAsync()
    {
        if (IsBusy) return;

        submitted = true;
        if (!ValidateMessage()) return;

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

        try
        {
            // Le message est enregistre dans la base
            await ContactService.SaveAsync(orphanage, subject, message, UserSession.CurrentUser?.Email ?? "");
        }
        catch (Exception)
        {
            // Echec de l'enregistrement : on affiche l'etat d'echec existant
            IsBusy = false;
            ShowFailure = true;
            ButtonText = RetryText;
            return;
        }

        IsBusy = false;
        ShowFailure = false;

        // "Le Centre d'Espoir vous repondra rapidement." (pluriel : "Les Petits Cœurs vous repondront rapidement.")
        string verb = orphanage.HasPluralName ? "répondront" : "répondra";
        await Shell.Current.DisplayAlertAsync("Message envoyé",
            orphanage.TheName + " vous " + verb + " rapidement.", "OK");

        // La page est recreee a chaque ouverture : l'objet et le message sont vides la prochaine fois
        await Shell.Current.GoToAsync("..");
    }
}