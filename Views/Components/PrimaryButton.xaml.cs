using System.Windows.Input;

namespace KindHome.Views.Components;

// Bouton principal (degrade vert). Etats : Normal, Actif (appui), Desactive (IsEnabled = false), Erreur (IsError = true).
public partial class PrimaryButton : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(PrimaryButton), "");

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command), typeof(ICommand), typeof(PrimaryButton), null);

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        nameof(CommandParameter), typeof(object), typeof(PrimaryButton), null);

    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon), typeof(ImageSource), typeof(PrimaryButton), null,
        propertyChanged: (b, o, n) => ((PrimaryButton)b).Refresh());

    // Version pour fond sombre (onboarding)
    public static readonly BindableProperty OnDarkProperty = BindableProperty.Create(
        nameof(OnDark), typeof(bool), typeof(PrimaryButton), false,
        propertyChanged: (b, o, n) => ((PrimaryButton)b).Refresh());

    public static readonly BindableProperty IsErrorProperty = BindableProperty.Create(
        nameof(IsError), typeof(bool), typeof(PrimaryButton), false,
        propertyChanged: (b, o, n) => ((PrimaryButton)b).Refresh());

    // Envoi en cours (sans animation)
    public static readonly BindableProperty IsBusyProperty = BindableProperty.Create(
        nameof(IsBusy), typeof(bool), typeof(PrimaryButton), false,
        propertyChanged: (b, o, n) => ((PrimaryButton)b).Refresh());

    // Hauteur 48, texte 15 (boutons des etats vides)
    public static readonly BindableProperty CompactProperty = BindableProperty.Create(
        nameof(Compact), typeof(bool), typeof(PrimaryButton), false,
        propertyChanged: (b, o, n) => ((PrimaryButton)b).Refresh());

    // Compact avec 32 de marge (bouton "Reessayer")
    public static readonly BindableProperty WideProperty = BindableProperty.Create(
        nameof(Wide), typeof(bool), typeof(PrimaryButton), false,
        propertyChanged: (b, o, n) => ((PrimaryButton)b).Refresh());

    // Hauteur 52, texte 15 (deux boutons cote a cote)
    public static readonly BindableProperty SplitProperty = BindableProperty.Create(
        nameof(Split), typeof(bool), typeof(PrimaryButton), false,
        propertyChanged: (b, o, n) => ((PrimaryButton)b).Refresh());

    // Icone a gauche du texte (a droite par defaut)
    public static readonly BindableProperty IconOnLeftProperty = BindableProperty.Create(
        nameof(IconOnLeft), typeof(bool), typeof(PrimaryButton), false,
        propertyChanged: (b, o, n) => ((PrimaryButton)b).Refresh());

    public PrimaryButton()
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

    public ImageSource? Icon
    {
        get => (ImageSource?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public bool OnDark
    {
        get => (bool)GetValue(OnDarkProperty);
        set => SetValue(OnDarkProperty, value);
    }

    public bool Compact
    {
        get => (bool)GetValue(CompactProperty);
        set => SetValue(CompactProperty, value);
    }

    public bool Wide
    {
        get => (bool)GetValue(WideProperty);
        set => SetValue(WideProperty, value);
    }

    public bool Split
    {
        get => (bool)GetValue(SplitProperty);
        set => SetValue(SplitProperty, value);
    }

    public bool IconOnLeft
    {
        get => (bool)GetValue(IconOnLeftProperty);
        set => SetValue(IconOnLeftProperty, value);
    }

    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
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

        string compact = Wide ? "PrimaryButtonCompactWide" : "PrimaryButtonCompact";
        string style = IsError ? "ErrorButton" : IsBusy ? "PrimaryButtonBusy" : OnDark ? "PrimaryButtonOnDark" : Compact ? compact : Split ? "PrimaryButtonSplit" : "PrimaryButton";
        if (Application.Current!.Resources.TryGetValue(style, out var value) && value is Style s)
            Inner.Style = s;

        // Envoi en cours : le bouton ne reagit plus aux appuis
        Inner.InputTransparent = IsBusy;

        if (IsBusy)
        {
            Inner.ImageSource = "icon_spinner.png";
            Inner.ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Left, 10);
        }
        else if (IsError)
        {
            // Etat erreur : icone "reessayer" a gauche du texte
            Inner.ImageSource = "icon_retry.png";
            Inner.ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Left, 8);
        }
        else
        {
            Inner.ImageSource = Icon;
            if (IconOnLeft)
                Inner.ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Left, 8);
            else
                Inner.ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Right, 10);
        }
    }
}
