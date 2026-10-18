using KindHome.Models;

namespace KindHome.ViewModels;

// Une ligne de la liste de l'annuaire : l'orphelinat et son etat "choisi" (repere appuye)
public class DirectoryItem : BaseViewModel
{
    bool isSelected;

    public DirectoryItem(Orphanage orphanage)
    {
        Orphanage = orphanage;
    }

    public Orphanage Orphanage { get; }

    bool isLast;

    // Filet de separation entre les lignes (absent sous la derniere)
    public bool IsLast
    {
        get => isLast;
        set
        {
            if (!SetProperty(ref isLast, value)) return;
            OnPropertyChanged(nameof(ShowDivider));
        }
    }

    public bool ShowDivider => !isLast;

    public bool IsSelected
    {
        get => isSelected;
        set => SetProperty(ref isSelected, value);
    }
}