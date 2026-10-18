using System.Windows.Input;

namespace KindHome.Views.Components;

// Bouton secondaire (contour vert). Etats : Normal, Actif (appui), Desactive (IsEnabled = false), Erreur (IsError = true).
public partial class SecondaryButton : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(SecondaryButton), "");

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command), typeof(ICommand), typeof(SecondaryButton), null);

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        nameof(CommandParameter), typeof(object), typeof(SecondaryButton), null);

    public static readonly BindableProperty IsErrorProperty = BindableProperty.Create(
        nameof(IsError), typeof(bool), typeof(SecondaryButton), false,
        propertyChanged: (b, o, n) => ((SecondaryButton)b).Refresh());

    // Hauteur 48, texte 15 (boutons des etats vides)
    public static readonly BindableProperty CompactProperty = BindableProperty.Create(
        nameof(Compact), typeof(bool), typeof(SecondaryButton), false,
        propertyChanged: (b, o, n) => ((SecondaryButton)b).Refresh());

    // Hauteur 52, texte 15 (deux boutons cote a cote)
    public static readonly BindableProperty SplitProperty = BindableProperty.Create(
        nameof(Split), typeof(bool), typeof(SecondaryButton), false,
        propertyChanged: (b, o, n) => ((SecondaryButton)b).Refresh());

    // Icone a gauche du texte
    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon), typeof(ImageSource), typeof(SecondaryButton), null,
        propertyChanged: (b, o, n) => ((SecondaryButton)b).Refresh());

    public SecondaryButton()
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

    public bool Compact
    {
        get => (bool)GetValue(CompactProperty);
        set => SetValue(CompactProperty, value);
    }

    public bool Split
    {
        get => (bool)GetValue(SplitProperty);
        set => SetValue(SplitProperty, value);
    }

    public ImageSource? Icon
    {
        get => (ImageSource?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
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

        string style = IsError ? "SecondaryButtonError" : Compact ? "SecondaryButtonCompact" : Split ? "SecondaryButtonSplit" : "SecondaryButton";
        if (Application.Current!.Resources.TryGetValue(style, out var value) && value is Style s)
            Inner.Style = s;

        Inner.ImageSource = Icon;
        Inner.ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Left, 8);
    }
}
