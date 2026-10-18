using System.Windows.Input;

namespace KindHome.Views.Components;

// Champ de saisie arrondi : libelle, icone, prefixe, texte, oeil (mot de passe), icone d'etat et message.
// Etats : Normal, Active (focus), Valid (IsValid), Disabled (IsEnabled = false), Error (ErrorMessage non vide).
public partial class FormField : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(FormField), "",
        propertyChanged: (b, o, n) => ((FormField)b).Refresh());

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(FormField), "",
        BindingMode.TwoWay);

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(FormField), "");

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
        nameof(Keyboard), typeof(Keyboard), typeof(FormField), Keyboard.Default);

    public static readonly BindableProperty ReturnTypeProperty = BindableProperty.Create(
        nameof(ReturnType), typeof(ReturnType), typeof(FormField), ReturnType.Default);

    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon), typeof(ImageSource), typeof(FormField), null,
        propertyChanged: (b, o, n) => ((FormField)b).Refresh());

    // Icone affichee a la place de Icon quand le champ est actif (optionnelle)
    public static readonly BindableProperty ActiveIconProperty = BindableProperty.Create(
        nameof(ActiveIcon), typeof(ImageSource), typeof(FormField), null,
        propertyChanged: (b, o, n) => ((FormField)b).Refresh());

    public static readonly BindableProperty PrefixProperty = BindableProperty.Create(
        nameof(Prefix), typeof(string), typeof(FormField), "",
        propertyChanged: (b, o, n) => ((FormField)b).Refresh());

    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(
        nameof(IsPassword), typeof(bool), typeof(FormField), false,
        propertyChanged: (b, o, n) => ((FormField)b).Refresh());

    public static readonly BindableProperty IsValidProperty = BindableProperty.Create(
        nameof(IsValid), typeof(bool), typeof(FormField), false,
        propertyChanged: (b, o, n) => ((FormField)b).Refresh());

    public static readonly BindableProperty ErrorMessageProperty = BindableProperty.Create(
        nameof(ErrorMessage), typeof(string), typeof(FormField), "",
        propertyChanged: (b, o, n) => ((FormField)b).Refresh());

    // Texte d'aide gris sous le champ (remplace par le message d'erreur, en rouge, quand il y en a un)
    public static readonly BindableProperty HelperTextProperty = BindableProperty.Create(
        nameof(HelperText), typeof(string), typeof(FormField), "",
        propertyChanged: (b, o, n) => ((FormField)b).Refresh());

    // Espace entre le champ et son message (4 par defaut, 3 sur la page Inscription)
    public static readonly BindableProperty MessageTopMarginProperty = BindableProperty.Create(
        nameof(MessageTopMargin), typeof(double), typeof(FormField), 4.0,
        propertyChanged: (b, o, n) => ((FormField)b).Refresh());

    // Commande lancee quand on quitte le champ
    public static readonly BindableProperty BlurCommandProperty = BindableProperty.Create(
        nameof(BlurCommand), typeof(ICommand), typeof(FormField), null);

    bool passwordVisible;

    public FormField()
    {
        InitializeComponent();
        Refresh();
    }

    public event EventHandler? Completed;

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public ReturnType ReturnType
    {
        get => (ReturnType)GetValue(ReturnTypeProperty);
        set => SetValue(ReturnTypeProperty, value);
    }

    public ImageSource? Icon
    {
        get => (ImageSource?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public ImageSource? ActiveIcon
    {
        get => (ImageSource?)GetValue(ActiveIconProperty);
        set => SetValue(ActiveIconProperty, value);
    }

    public string Prefix
    {
        get => (string)GetValue(PrefixProperty);
        set => SetValue(PrefixProperty, value);
    }

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    // Champ correct : affiche la coche verte a droite
    public bool IsValid
    {
        get => (bool)GetValue(IsValidProperty);
        set => SetValue(IsValidProperty, value);
    }

    public string ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    public string HelperText
    {
        get => (string)GetValue(HelperTextProperty);
        set => SetValue(HelperTextProperty, value);
    }

    public double MessageTopMargin
    {
        get => (double)GetValue(MessageTopMarginProperty);
        set => SetValue(MessageTopMarginProperty, value);
    }

    public ICommand? BlurCommand
    {
        get => (ICommand?)GetValue(BlurCommandProperty);
        set => SetValue(BlurCommandProperty, value);
    }

    bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    // Met le curseur dans ce champ (pour passer au champ suivant avec Entree)
    public void FocusInput() => Input.Focus();

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == IsEnabledProperty.PropertyName)
            Refresh();
    }

    void OnInputFocused(object? sender, FocusEventArgs e) => Refresh();

    void OnInputUnfocused(object? sender, FocusEventArgs e)
    {
        Refresh();
        if (BlurCommand?.CanExecute(null) == true)
            BlurCommand.Execute(null);
    }

    void OnInputCompleted(object? sender, EventArgs e) => Completed?.Invoke(this, EventArgs.Empty);

    void OnEyeClicked(object? sender, EventArgs e)
    {
        passwordVisible = !passwordVisible;
        Refresh();
    }

    // Applique l'etat visuel et l'affichage des elements
    void Refresh()
    {
        if (Input == null) return;

        bool showError = HasError;
        bool focused = Input.IsFocused;
        string state = !IsEnabled ? "Disabled" : showError ? "Error" : focused ? "Active" : IsValid ? "Valid" : "Normal";

        TitleLabel.Text = Title;
        TitleLabel.IsVisible = !string.IsNullOrEmpty(Title);

        var icon = state == "Active" && ActiveIcon != null ? ActiveIcon : Icon;
        LeadingIcon.Source = icon;
        LeadingIcon.IsVisible = icon != null;

        PrefixLabel.Text = Prefix;
        PrefixLabel.IsVisible = !string.IsNullOrEmpty(Prefix);
        PrefixDivider.IsVisible = PrefixLabel.IsVisible;

        Input.IsEnabled = IsEnabled;
        Input.IsPassword = IsPassword && !passwordVisible;

        bool showEye = IsPassword && !showError && IsEnabled;
        // Sur un mot de passe correct, l'oeil reste affiche et la coche verte n'apparait pas
        bool showStatus = showError || (IsValid && !showEye);

        EyeButton.IsVisible = showEye;
        StatusIcon.IsVisible = showStatus;
        StatusIcon.Source = showError ? "icon_alert_circle.png" : "icon_check_circle.png";

        // Sous le champ : message d'erreur (rouge) ou texte d'aide (gris)
        bool showHelper = !showError && !string.IsNullOrEmpty(HelperText);
        MessageLabel.Text = showError ? ErrorMessage : HelperText;
        MessageLabel.IsVisible = showError || showHelper;
        MessageLabel.Margin = new Thickness(18, MessageTopMargin, 0, 0);
        SetMessageStyle(showHelper ? "HintText" : "ErrorText");

        string padding = showEye ? "FieldPaddingWithButton" : showStatus ? "FieldPaddingWithIcon" : "FieldPadding";
        if (Application.Current!.Resources.TryGetValue(padding, out var value) && value is Thickness thickness)
            Box.Padding = thickness;

        VisualStateManager.GoToState(Box, state);
        VisualStateManager.GoToState(Ring, state);
    }

    void SetMessageStyle(string key)
    {
        if (Application.Current!.Resources.TryGetValue(key, out var value) && value is Style style)
            MessageLabel.Style = style;
    }
}
