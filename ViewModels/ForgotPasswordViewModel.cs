using System.Text.RegularExpressions;
using System.Windows.Input;

namespace KindHome.ViewModels;

public class ForgotPasswordViewModel : BaseViewModel
{
    const string SendText = "Envoyer le lien";
    const string BusyText = "Envoi en cours";
    const string ResendText = "Renvoyer le lien";

    string email = "";
    string emailError = "";
    string buttonText = SendText;
    bool submitted;
    bool isBusy;
    bool isSent;

    public ForgotPasswordViewModel()
    {
        SendCommand = new Command(async () => await SendAsync());
        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    public ICommand SendCommand { get; }
    public ICommand BackCommand { get; }

    public string Email
    {
        get => email;
        set
        {
            if (SetProperty(ref email, value) && submitted)
                ValidateEmail();
        }
    }

    public string EmailError
    {
        get => emailError;
        set => SetProperty(ref emailError, value);
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

    // Une fois le lien "envoye", le bloc de succes remplace le bloc de conseil
    public bool IsSent
    {
        get => isSent;
        set
        {
            if (!SetProperty(ref isSent, value)) return;
            OnPropertyChanged(nameof(IsNotSent));
        }
    }

    public bool IsNotSent => !isSent;

    bool ValidateEmail()
    {
        string value = Email.Trim();
        if (value.Length == 0)
            EmailError = "Adresse e-mail obligatoire";
        else if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            EmailError = "Adresse e-mail invalide. Exemple : nom@mail.com";
        else
            EmailError = "";
        return EmailError.Length == 0;
    }

    async Task SendAsync()
    {
        if (IsBusy) return;

        submitted = true;
        if (!ValidateEmail()) return;

        // Envoi simule : rien n'est envoye nulle part
        IsBusy = true;
        ButtonText = BusyText;
        await Task.Delay(1000);

        IsBusy = false;
        IsSent = true;
        ButtonText = ResendText;
    }
}
