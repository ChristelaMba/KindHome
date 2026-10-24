using System.Collections.ObjectModel;
using System.Windows.Input;
using KindHome.Data;
using KindHome.Models;
using KindHome.Services;
using KindHome.Views;

namespace KindHome.ViewModels;

public class PublishNeedViewModel : BaseViewModel
{
    const string SendText = "Publier ce besoin";
    const string RetryText = "Réessayer l'envoi";
    const string BusyText = "Envoi en cours";
    const int MaxWhy = 300;

    string title = "";
    string quantity = "";
    string unit = "";
    string why = "";
    string titleError = "";
    string categoryError = "";
    string quantityError = "";
    string unitError = "";
    string buttonText = SendText;
    bool submitted;
    bool isBusy;
    bool showFailure;

    public PublishNeedViewModel()
    {
        Categories = new ObservableCollection<ChipItem>(
            new[] { "Alimentation", "Scolarité", "Santé", "Vêtements", "Financier" }
                .Select((name, i) => new ChipItem(name, i)));

        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        SelectCategoryCommand = new Command<ChipItem>(SelectCategory);
        PublishCommand = new Command(async () => await PublishAsync());
    }

    public ObservableCollection<ChipItem> Categories { get; }

    public ICommand BackCommand { get; }
    public ICommand SelectCategoryCommand { get; }
    public ICommand PublishCommand { get; }

    public string Title
    {
        get => title;
        set
        {
            if (SetProperty(ref title, value) && submitted)
                ValidateTitle();
        }
    }

    public string Quantity
    {
        get => quantity;
        set
        {
            if (SetProperty(ref quantity, value) && submitted)
                ValidateQuantity();
        }
    }

    public string Unit
    {
        get => unit;
        set
        {
            if (SetProperty(ref unit, value) && submitted)
                ValidateUnit();
        }
    }

    public string Why
    {
        get => why;
        set
        {
            if (!SetProperty(ref why, value)) return;
            OnPropertyChanged(nameof(WhyCounter));
        }
    }

    // "0 / 300"
    public string WhyCounter => why.Length + " / " + MaxWhy;

    public string TitleError { get => titleError; set => SetProperty(ref titleError, value); }
    public string QuantityError { get => quantityError; set => SetProperty(ref quantityError, value); }
    public string UnitError { get => unitError; set => SetProperty(ref unitError, value); }

    public string CategoryError
    {
        get => categoryError;
        set
        {
            if (SetProperty(ref categoryError, value))
                OnPropertyChanged(nameof(HasCategoryError));
        }
    }

    public bool HasCategoryError => categoryError.Length > 0;

    public string ButtonText { get => buttonText; set => SetProperty(ref buttonText, value); }
    public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
    public bool ShowFailure { get => showFailure; set => SetProperty(ref showFailure, value); }

    // Une seule categorie a la fois, aucune au depart
    void SelectCategory(ChipItem? item)
    {
        if (item == null) return;
        foreach (var chip in Categories)
            chip.IsSelected = chip == item;
        if (submitted)
            ValidateCategory();
    }

    bool ValidateTitle()
    {
        TitleError = title.Trim().Length == 0 ? "Titre obligatoire" : "";
        return TitleError.Length == 0;
    }

    bool ValidateCategory()
    {
        bool ok = Categories.Any(c => c.IsSelected);
        CategoryError = ok ? "" : "Choisissez une catégorie.";
        foreach (var chip in Categories)
            chip.IsError = !ok;
        return ok;
    }

    bool ValidateQuantity()
    {
        string value = quantity.Trim();
        if (value.Length == 0)
            QuantityError = "Quantité obligatoire";
        else if (!int.TryParse(value, out int number) || number <= 0)
            QuantityError = "Quantité invalide";
        else
            QuantityError = "";
        return QuantityError.Length == 0;
    }

    bool ValidateUnit()
    {
        UnitError = unit.Trim().Length == 0 ? "Unité obligatoire" : "";
        return UnitError.Length == 0;
    }

    async Task PublishAsync()
    {
        if (IsBusy) return;

        submitted = true;
        bool titleOk = ValidateTitle();
        bool categoryOk = ValidateCategory();
        bool quantityOk = ValidateQuantity();
        bool unitOk = ValidateUnit();
        if (!titleOk || !categoryOk || !quantityOk || !unitOk) return;

        // Le bouton reste bloque jusqu'a la fin de l'enregistrement (pas de double publication)
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

        var user = UserSession.CurrentUser;
        var orphanage = MockData.Orphanages.FirstOrDefault(o => o.Name == user?.FullName)
            ?? new Orphanage { Name = user?.FullName ?? "" };

        var need = new Need
        {
            Title = title.Trim(),
            Orphanage = orphanage,
            Collected = 0,
            Total = int.Parse(quantity.Trim()),
            Unit = unit.Trim(),
            Category = Categories.First(c => c.IsSelected).Name,
            WhyImportant = why
        };

        try
        {
            // Le besoin est enregistre dans la base (il reste apres la fermeture de l'app)
            await NeedService.AddAsync(need);
        }
        catch
        {
            // Echec de l'enregistrement : on affiche l'etat d'echec existant
            IsBusy = false;
            ShowFailure = true;
            ButtonText = RetryText;
            return;
        }

        IsBusy = false;
        ShowFailure = false;
        await Shell.Current.DisplayAlertAsync("Besoin publié", "Les donateurs peuvent maintenant le voir.", "OK");

        // Retour a l'onglet Besoins de l'organisation (pile de pages videe)
        await Shell.Current.GoToAsync("//OrgNeedsPage");
    }
}
