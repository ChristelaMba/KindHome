using System.Text.RegularExpressions;
using System.Windows.Input;
using KindHome.Data;
using KindHome.Models;
using KindHome.Services;
using KindHome.Views;

namespace KindHome.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    const string CreateText = "Créer mon compte";
    const string BusyText = "Création en cours";

    UserRole role = UserRole.Donor;
    string fullName = "";
    string email = "";
    string phone = "";
    string password = "";
    bool acceptedTerms;
    bool termsWasChecked;
    bool isBusy;
    bool emailTaken;
    string buttonText = CreateText;

    // Un champ affiche son erreur seulement apres avoir ete quitte une premiere fois
    bool nameTouched;
    bool emailTouched;
    bool phoneTouched;
    bool passwordTouched;

    public RegisterViewModel()
    {
        SelectRoleCommand = new Command<UserRole>(r => Role = r);
        BlurNameCommand = new Command(() => { nameTouched = true; RefreshName(); });
        BlurEmailCommand = new Command(() => { emailTouched = true; RefreshEmail(); });
        BlurPhoneCommand = new Command(() => { phoneTouched = true; RefreshPhone(); });
        BlurPasswordCommand = new Command(() => { passwordTouched = true; RefreshPassword(); });
        CreateCommand = new Command(async () => await CreateAsync());
        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        // "Se connecter" mene a la connexion, meme quand l'inscription a ete ouverte depuis le profil
        LoginLinkCommand = new Command(async () => await Shell.Current.GoToAsync("//" + nameof(LoginPage)));
    }

    public ICommand SelectRoleCommand { get; }
    public ICommand BlurNameCommand { get; }
    public ICommand BlurEmailCommand { get; }
    public ICommand BlurPhoneCommand { get; }
    public ICommand BlurPasswordCommand { get; }
    public ICommand CreateCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand LoginLinkCommand { get; }

    public UserRole Role
    {
        get => role;
        set
        {
            if (!SetProperty(ref role, value)) return;
            OnPropertyChanged(nameof(IsDonorSelected));
            OnPropertyChanged(nameof(IsVolunteerSelected));
            OnPropertyChanged(nameof(IsOrganizationSelected));
        }
    }

    public bool IsDonorSelected => role == UserRole.Donor;
    public bool IsVolunteerSelected => role == UserRole.Volunteer;
    public bool IsOrganizationSelected => role == UserRole.Organization;

    public string FullName
    {
        get => fullName;
        set { if (SetProperty(ref fullName, value)) RefreshName(); }
    }

    public string Email
    {
        get => email;
        set
        {
            if (!SetProperty(ref email, value)) return;
            // Si la personne change l'e-mail, l'erreur "deja utilise" disparait
            emailTaken = false;
            RefreshEmail();
        }
    }

    public string Phone
    {
        get => phone;
        set { if (SetProperty(ref phone, value)) RefreshPhone(); }
    }

    public string Password
    {
        get => password;
        set { if (SetProperty(ref password, value)) RefreshPassword(); }
    }

    public bool AcceptedTerms
    {
        get => acceptedTerms;
        set
        {
            if (!SetProperty(ref acceptedTerms, value)) return;
            if (value) termsWasChecked = true;
            OnPropertyChanged(nameof(TermsError));
            OnPropertyChanged(nameof(CanSubmit));
        }
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

    // Erreurs affichees (vides tant que le champ n'a pas ete quitte)
    public string NameError => nameTouched ? NameMessage() : "";
    public string EmailError => emailTouched ? EmailMessage() : "";
    public string PhoneError => phoneTouched ? PhoneMessage() : "";
    public string PasswordError => passwordTouched ? PasswordMessage() : "";
    public string TermsError => termsWasChecked && !acceptedTerms ? "Ce champ est obligatoire." : "";

    // Champs corrects (coche verte)
    public bool NameValid => NameMessage().Length == 0;
    public bool EmailValid => EmailMessage().Length == 0;
    public bool PhoneValid => PhoneMessage().Length == 0;
    public bool PasswordValid => PasswordMessage().Length == 0;

    // Le bouton reste grise tant que tout n'est pas valide
    public bool CanSubmit => NameValid && EmailValid && PhoneValid && PasswordValid && acceptedTerms;

    string NameMessage() => fullName.Trim().Length == 0 ? "Nom complet obligatoire" : "";

    string EmailMessage()
    {
        string value = email.Trim();
        if (value.Length == 0) return "Adresse e-mail obligatoire";
        if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) return "Adresse e-mail invalide. Exemple : nom@mail.com";
        if (emailTaken) return "Cette adresse e-mail est déjà utilisée";
        return "";
    }

    string PhoneMessage()
    {
        string digits = Regex.Replace(phone, @"\s", "");
        if (digits.Length == 0) return "Numéro de téléphone obligatoire";
        if (!Regex.IsMatch(digits, @"^6\d{8}$")) return "Numéro invalide. Exemple : 6 XX XX XX XX";
        return "";
    }

    string PasswordMessage()
    {
        if (password.Length == 0) return "Mot de passe obligatoire";
        if (password.Length < 8 || !password.Any(char.IsDigit)) return "8 caractères minimum, dont un chiffre.";
        return "";
    }

    void RefreshName()
    {
        OnPropertyChanged(nameof(NameError));
        OnPropertyChanged(nameof(NameValid));
        OnPropertyChanged(nameof(CanSubmit));
    }

    void RefreshEmail()
    {
        OnPropertyChanged(nameof(EmailError));
        OnPropertyChanged(nameof(EmailValid));
        OnPropertyChanged(nameof(CanSubmit));
    }

    void RefreshPhone()
    {
        OnPropertyChanged(nameof(PhoneError));
        OnPropertyChanged(nameof(PhoneValid));
        OnPropertyChanged(nameof(CanSubmit));
    }

    void RefreshPassword()
    {
        OnPropertyChanged(nameof(PasswordError));
        OnPropertyChanged(nameof(PasswordValid));
        OnPropertyChanged(nameof(CanSubmit));
    }

    async Task CreateAsync()
    {
        if (IsBusy || !CanSubmit) return;

        IsBusy = true;
        ButtonText = BusyText;
        await Task.Delay(1000);

        // Un seul compte par adresse e-mail
        if (await AccountService.EmailExistsAsync(email))
        {
            emailTaken = true;
            emailTouched = true;
            RefreshEmail();
            IsBusy = false;
            ButtonText = CreateText;
            return;
        }

        var account = new UserAccount
        {
            FullName = fullName.Trim(),
            Email = email.Trim(),
            Phone = phone.Trim(),
            Password = password,
            Role = role
        };
        await AccountService.SaveAsync(account);
        MockData.Accounts.Add(account);
        UserSession.SignIn(account);

        string route = role == UserRole.Organization ? nameof(OrgDashboardPage) : nameof(HomePage);
        await Shell.Current.GoToAsync("//" + route);

        IsBusy = false;
        ButtonText = CreateText;
    }
}
