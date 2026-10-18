namespace KindHome.Views.Components;

// Case a cocher avec son texte et son message d'erreur.
// Etats : Normal, Actif (cochee), Desactive (IsEnabled = false), Erreur (ErrorMessage non vide).
public partial class CheckBoxField : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(CheckBoxField), "");

    public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(
        nameof(IsChecked), typeof(bool), typeof(CheckBoxField), false,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((CheckBoxField)b).Refresh());

    public static readonly BindableProperty ErrorMessageProperty = BindableProperty.Create(
        nameof(ErrorMessage), typeof(string), typeof(CheckBoxField), "",
        propertyChanged: (b, o, n) => ((CheckBoxField)b).Refresh());

    public CheckBoxField()
    {
        InitializeComponent();
        Refresh();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public string ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == IsEnabledProperty.PropertyName)
            Refresh();
    }

    void OnTapped(object? sender, TappedEventArgs e)
    {
        if (IsEnabled)
            IsChecked = !IsChecked;
    }

    void Refresh()
    {
        if (Box == null) return;

        bool hasError = !string.IsNullOrEmpty(ErrorMessage);
        string state = !IsEnabled ? "Disabled" : hasError ? "Error" : IsChecked ? "Checked" : "Normal";

        CheckIcon.IsVisible = IsChecked;
        TextLabel.IsEnabled = IsEnabled;
        ErrorLabel.Text = ErrorMessage;
        ErrorLabel.IsVisible = hasError;

        VisualStateManager.GoToState(Box, state);
    }
}
