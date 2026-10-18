using System.Windows.Input;

namespace KindHome.Views.Components;

// Bouton rond de quantite. IsPlus : "+" (contour vert) ou "-" (contour gris).
// Etats : Normal, Desactive (IsEnabled = false, quand la limite est atteinte).
public partial class QuantityButton : ContentView
{
    public static readonly BindableProperty IsPlusProperty = BindableProperty.Create(
        nameof(IsPlus), typeof(bool), typeof(QuantityButton), false,
        propertyChanged: (b, o, n) => ((QuantityButton)b).Refresh());

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command), typeof(ICommand), typeof(QuantityButton), null);

    public QuantityButton()
    {
        InitializeComponent();
        Refresh();
    }

    public bool IsPlus
    {
        get => (bool)GetValue(IsPlusProperty);
        set => SetValue(IsPlusProperty, value);
    }

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == IsEnabledProperty.PropertyName)
            Refresh();
    }

    void OnTapped(object? sender, TappedEventArgs e)
    {
        if (IsEnabled && Command?.CanExecute(null) == true)
            Command.Execute(null);
    }

    void Refresh()
    {
        if (Circle == null) return;

        string style = !IsEnabled ? "QuantityButtonBorderDisabled" : IsPlus ? "QuantityButtonBorderActive" : "QuantityButtonBorder";
        if (Application.Current!.Resources.TryGetValue(style, out var value) && value is Style s)
            Circle.Style = s;

        Glyph.Source = IsPlus ? "icon_plus.png" : "icon_minus.png";
        Glyph.Opacity = IsEnabled ? 1 : 0.4;
    }
}
