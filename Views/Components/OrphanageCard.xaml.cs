using System.Windows.Input;
using KindHome.Models;

namespace KindHome.Views.Components;

public partial class OrphanageCard : ContentView
{
    public static readonly BindableProperty OrphanageProperty = BindableProperty.Create(
        nameof(Orphanage), typeof(Orphanage), typeof(OrphanageCard), null);

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command), typeof(ICommand), typeof(OrphanageCard), null);

    public OrphanageCard()
    {
        InitializeComponent();
    }

    public Orphanage? Orphanage
    {
        get => (Orphanage?)GetValue(OrphanageProperty);
        set => SetValue(OrphanageProperty, value);
    }

    // Recoit l'orphelinat en parametre
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    void OnTapped(object? sender, TappedEventArgs e)
    {
        if (Command?.CanExecute(Orphanage) == true)
            Command.Execute(Orphanage);
    }
}
