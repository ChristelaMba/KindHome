using System.Text.RegularExpressions;
using System.Windows.Input;
using KindHome.Services;
using KindHome.Views;

namespace KindHome.ViewModels;

public class ForgotPasswordViewModel : BaseViewModel
{
    const string CheckText = "Vérifier mon compte";
    const string ResetText = "Changer le mot de passe";
    const string BusyCheckText = "Vérification en cours";
    const string BusyResetText = "Enregistrement en cours";
    const int MaxAttempts = 5;

    string email = "";
    string phone = "";
    string newPassword = "";
    string confirmPassword = "";
    string emailError = "";
    string phoneError = "";
    string newPasswordError = "";
    string confirmError = "";
    string formError = "";
    string checkButtonText = CheckText;
    string resetButtonText = ResetText;
    bool submittedIdentity;
    bool submittedPassword;
    bool isBusy;
    int step = 1;
    int attempts;

    public ForgotPasswordViewModel()
    {
        BackCommand = new Command(async () => await GoBackAsync());
        LoginLinkCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        GoToLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//" + nameof(LoginPage)));
        CheckCommand = new Command(async () => await CheckAsync());
        ResetCommand = new Command(async () => await ResetAsync());
    }

    // Fleche retour : de l'etape 2 on revient a l'etape 1, sinon on quitte la page
    public ICommand BackCommand { get; }

    // Lien "Retour a la connexion" du bas de page
    public ICommand LoginLinkCommand { get; }

    // Bouton "Se connecter" de l'ecran final (vide la pile de pages)
    public ICommand GoToLoginCommand { get; }

    public ICommand CheckCommand { get; }
    public ICommand ResetCommand { get; }

    // Etape 1 : e-mail et telephone. Etape 2 : nouveau mot de passe. Etape 3 : termine.
    public bool IsStep1 => step == 1;
    public bool IsStep2 => step == 2;
    public bool IsDone => step == 3;
    public bool ShowFooterLink => step != 3;

    public string HeadingText => step switch
    {
        2 => "Nouveau mot de passe",
        3 => "Mot de passe modifié",
        _ => "Mot de passe oublié ?"
    };

    public string SubtitleText => step switch
    {
        2 => "Choisissez un nouveau mot de passe pour votre compte.",
        3 => "Vous pouvez maintenant vous connecter avec votre nouveau mot de passe.",
        _ => "Saisissez l'adresse e-mail et le numéro de téléphone de votre compte."
    };

    public string Email
    {
        get => email;
        set
        {
            if (!SetProperty(ref email, value)) return;
            FormError = "";
            if (submittedIdentity) ValidateEmail();
        }
    }

    public string Phone
    {
        get => phone;
        set
        {
            if (!SetProperty(ref phone, value)) return;
            FormError = "";
            if (submittedIdentity) ValidatePhone();
        }
    }

    public string NewPassword
    {
        get => newPassword;
        set
        {
            if (!SetProperty(ref newPassword, value)) return;
            FormError = "";
            if (submittedPassword)
            {
                ValidateNewPassword();
                ValidateConfirm();
            }
        }
    }

    public string ConfirmPassword
    {
        get => confirmPassword;
        set
        {
            if (!SetProperty(ref confirmPassword, value)) return;
            FormError = "";
            if (submittedPassword) ValidateConfirm();
        }
    }

    public string EmailError { get => emailError; private set => SetProperty(ref emailError, value); }
    public string PhoneError { get => phoneError; private set => SetProperty(ref phoneError, value); }
    public string NewPasswordError { get => newPasswordError; private set => SetProperty(ref newPasswordError, value); }
    public string ConfirmError { get => confirmError; private set => SetProperty(ref confirmError, value); }

    // Message general (compte introuvable, compte de demonstration, trop d'essais...)
    public string FormError
    {
        get => formError;
        private set
        {
            if (!SetProperty(ref formError, value)) return;
            OnPropertyChanged(nameof(HasFormError));
        }
    }

    public bool HasFormError => formError.Length > 0;

    public string CheckButtonText { get => checkButtonText; private set => SetProperty(ref checkButtonText, value); }
    public string ResetButtonText { get => resetButtonText; private set => SetProperty(ref resetButtonText, value); }
    public bool IsBusy { get => isBusy; private set => SetProperty(ref isBusy, value); }

    bool Locked => attempts >= MaxAttempts;

    void SetStep(int value)
    {
        if (step == value) return;
        step = value;
        FormError = "";
        OnPropertyChanged(nameof(IsStep1));
        OnPropertyChanged(nameof(IsStep2));
        OnPropertyChanged(nameof(IsDone));
        OnPropertyChanged(nameof(ShowFooterLink));
        OnPropertyChanged(nameof(HeadingText));
        OnPropertyChanged(nameof(SubtitleText));
    }

    async Task GoBackAsync()
    {
        if (IsBusy) return;

        if (step == 2)
        {
            // Retour a l'etape 1 : les mots de passe saisis sont effaces
            ClearPasswords();
            SetStep(1);
            return;
        }

        await Shell.Current.GoToAsync("..");
    }

    void ClearPasswords()
    {
        newPassword = "";
        confirmPassword = "";
        submittedPassword = false;
        NewPasswordError = "";
        ConfirmError = "";
        OnPropertyChanged(nameof(NewPassword));
        OnPropertyChanged(nameof(ConfirmPassword));
    }

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

    bool ValidatePhone()
    {
        string digits = AccountService.NormalizePhone(Phone);
        if (digits.Length == 0)
            PhoneError = "Numéro de téléphone obligatoire";
        else if (!Regex.IsMatch(digits, @"^6\d{8}$"))
            PhoneError = "Numéro invalide. Exemple : 6 XX XX XX XX";
        else
            PhoneError = "";
        return PhoneError.Length == 0;
    }

    bool ValidateNewPassword()
    {
        if (NewPassword.Length == 0)
            NewPasswordError = "Mot de passe obligatoire";
        else if (NewPassword.Length < 8 || !NewPassword.Any(char.IsDigit))
            NewPasswordError = "8 caractères minimum, dont un chiffre.";
        else
            NewPasswordError = "";
        return NewPasswordError.Length == 0;
    }

    bool ValidateConfirm()
    {
        if (ConfirmPassword.Length == 0)
            ConfirmError = "Confirmation obligatoire";
        else if (ConfirmPassword != NewPassword)
            ConfirmError = "Les mots de passe ne correspondent pas";
        else
            ConfirmError = "";
        return ConfirmError.Length == 0;
    }

    async Task CheckAsync()
    {
        if (IsBusy) return;

        if (Locked)
        {
            FormError = "Trop de tentatives. Revenez en arrière et réessayez plus tard.";
            return;
        }

        submittedIdentity = true;
        bool emailOk = ValidateEmail();
        bool phoneOk = ValidatePhone();
        FormError = "";
        if (!emailOk || !phoneOk) return;

        IsBusy = true;
        CheckButtonText = BusyCheckText;
        try
        {
            var result = await AccountService.CheckIdentityAsync(Email, Phone);

            if (result == ResetCheck.Ok)
            {
                SetStep(2);
                return;
            }

            if (result == ResetCheck.SampleAccount)
            {
                FormError = "Ce compte de démonstration ne peut pas changer de mot de passe.";
                return;
            }

            attempts++;
            FormError = Locked
                ? "Trop de tentatives. Revenez en arrière et réessayez plus tard."
                : "Aucun compte ne correspond à ces informations.";
        }
        catch (Exception)
        {
            FormError = "La vérification a échoué. Réessayez.";
        }
        finally
        {
            IsBusy = false;
            CheckButtonText = CheckText;
        }
    }

    async Task ResetAsync()
    {
        if (IsBusy) return;

        submittedPassword = true;
        bool passwordOk = ValidateNewPassword();
        bool confirmOk = ValidateConfirm();
        FormError = "";
        if (!passwordOk || !confirmOk) return;

        IsBusy = true;
        ResetButtonText = BusyResetText;
        try
        {
            await AccountService.ResetPasswordAsync(Email, Phone, NewPassword);

            // Les mots de passe ne restent pas en memoire
            ClearPasswords();
            SetStep(3);
        }
        catch (InvalidOperationException)
        {
            // L'identite n'est plus valide : on recommence a l'etape 1
            ClearPasswords();
            SetStep(1);
            FormError = "La vérification a échoué. Recommencez.";
        }
        catch (Exception)
        {
            FormError = "Le changement a échoué. Réessayez.";
        }
        finally
        {
            IsBusy = false;
            ResetButtonText = ResetText;
        }
    }
}
