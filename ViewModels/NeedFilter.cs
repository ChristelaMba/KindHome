namespace KindHome.ViewModels;

// Une puce de filtre de la page Besoins : un nom de categorie et son etat "choisie"
public class NeedFilter : BaseViewModel
{
    bool isSelected;

    public NeedFilter(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public bool IsSelected
    {
        get => isSelected;
        set => SetProperty(ref isSelected, value);
    }
}
