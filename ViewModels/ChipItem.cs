namespace KindHome.ViewModels;

// Une puce d'un groupe de choix (domaines, jours, creneaux) : son texte, son etat "choisie" et son etat "erreur"
public class ChipItem : BaseViewModel
{
    bool isSelected;
    bool isError;

    public ChipItem(string name, int index)
    {
        Name = name;
        Index = index;
    }

    public string Name { get; }

    // Position dans le groupe (0 = lundi pour les jours)
    public int Index { get; }

    public bool IsSelected
    {
        get => isSelected;
        set => SetProperty(ref isSelected, value);
    }

    public bool IsError
    {
        get => isError;
        set => SetProperty(ref isError, value);
    }
}