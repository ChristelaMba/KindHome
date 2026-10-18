using System.Windows.Input;
using KindHome.Models;

namespace KindHome.Views.Components;

// Carte de besoin reutilisable (fiche orphelinat, page Besoins). Command recoit le besoin en parametre.
public partial class NeedCard : ContentView
{
    public static readonly BindableProperty NeedProperty = BindableProperty.Create(
        nameof(Need), typeof(Need), typeof(NeedCard), null);

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command), typeof(ICommand), typeof(NeedCard), null);

    // false : la carte n'est pas cliquable, sans fleche (liste des besoins de l'organisation)
    public static readonly BindableProperty ShowChevronProperty = BindableProperty.Create(
        nameof(ShowChevron), typeof(bool), typeof(NeedCard), true);

    public NeedCard()
    {
        InitializeComponent();
    }

    public Need? Need
    {
        get => (Need?)GetValue(NeedProperty);
        set => SetValue(NeedProperty, value);
    }

    public bool ShowChevron
    {
        get => (bool)GetValue(ShowChevronProperty);
        set => SetValue(ShowChevronProperty, value);
    }

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    void OnTapped(object? sender, TappedEventArgs e)
    {
        if (Command?.CanExecute(Need) == true)
            Command.Execute(Need);
    }
}