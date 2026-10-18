using System.Windows.Input;

namespace KindHome.Views.Components;

// Puce de choix. Etats : Normal, Actif (IsSelected), Desactive (IsEnabled = false), Erreur (IsError).
public partial class ChoiceChip : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(ChoiceChip), "");

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command), typeof(ICommand), typeof(ChoiceChip), null);

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        nameof(CommandParameter), typeof(object), typeof(ChoiceChip), null);

    public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
        nameof(IsSelected), typeof(bool), typeof(ChoiceChip), false,
        propertyChanged: (b, o, n) => ((ChoiceChip)b).Refresh());

    public static readonly BindableProperty IsErrorProperty = BindableProperty.Create(
        nameof(IsError), typeof(bool), typeof(ChoiceChip), false,
        propertyChanged: (b, o, n) => ((ChoiceChip)b).Refresh());

    // Puce de jour (Lun, Mar...) : ronde, largeur 43
    public static readonly BindableProperty DayProperty = BindableProperty.Create(
        nameof(Day), typeof(bool), typeof(ChoiceChip), false,
        propertyChanged: (b, o, n) => ((ChoiceChip)b).Refresh());

    // Marge de 18 au lieu de 20 de chaque cote du texte (domaines d'aide)
    public static readonly BindableProperty TightProperty = BindableProperty.Create(
        nameof(Tight), typeof(bool), typeof(ChoiceChip), false,
        propertyChanged: (b, o, n) => ((ChoiceChip)b).Refresh());

    public ChoiceChip()
    {
        InitializeComponent();
        Refresh();
    }

    public event EventHandler? Clicked;

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public bool Day
    {
        get => (bool)GetValue(DayProperty);
        set => SetValue(DayProperty, value);
    }

    public bool Tight
    {
        get => (bool)GetValue(TightProperty);
        set => SetValue(TightProperty, value);
    }

    public bool IsError
    {
        get => (bool)GetValue(IsErrorProperty);
        set => SetValue(IsErrorProperty, value);
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == IsEnabledProperty.PropertyName && Inner != null)
            Inner.IsEnabled = IsEnabled;
    }

    void OnInnerClicked(object? sender, EventArgs e) => Clicked?.Invoke(this, EventArgs.Empty);

    void Refresh()
    {
        if (Inner == null) return;

        string style = Day ? "DayChipButton" : Tight ? "ChipButtonTight" : "ChipButton";
        if (Application.Current!.Resources.TryGetValue(style, out var value) && value is Style s && Inner.Style != s)
            Inner.Style = s;

        // Largeur : texte invisible dans la police de l'etat (choisie : semi-gras), plus la marge de chaque cote (+0.5 de securite)
        string stateFont = IsSelected ? "FontSemiBold" : "FontMedium";
        if (Application.Current!.Resources.TryGetValue(stateFont, out var font) && font is string fontName)
            Sizer.FontFamily = fontName;
        Sizer.FontSize = Day ? 13 : 14;
        Sizer.Margin = Day ? new Thickness(0) : new Thickness(Tight ? 18.5 : 20.5, 0);

        string state = IsSelected ? "Selected" : IsError ? "Error" : "Unselected";
        VisualStateManager.GoToState(Inner, state);
    }
}
