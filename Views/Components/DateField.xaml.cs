using System.Globalization;

namespace KindHome.Views.Components;

// Champ de date : appui = selecteur de date natif (date minimale : aujourd'hui).
// Une fois choisie, la date s'affiche au format jj/mm/aaaa. Etats : Normal, Erreur (ErrorMessage non vide).
public partial class DateField : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(DateField), "",
        propertyChanged: (b, o, n) => ((DateField)b).Refresh());

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(DateField), "jj/mm/aaaa",
        propertyChanged: (b, o, n) => ((DateField)b).Refresh());

    // null tant qu'aucune date n'est choisie
    public static readonly BindableProperty DateProperty = BindableProperty.Create(
        nameof(Date), typeof(DateTime?), typeof(DateField), null,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((DateField)b).Refresh());

    public static readonly BindableProperty ErrorMessageProperty = BindableProperty.Create(
        nameof(ErrorMessage), typeof(string), typeof(DateField), "",
        propertyChanged: (b, o, n) => ((DateField)b).Refresh());

    public DateField()
    {
        InitializeComponent();
        Picker.MinimumDate = DateTime.Today;
        Picker.Date = DateTime.Today;
        // Ouverture du selecteur : deux signaux possibles selon la plateforme (focus ou ouverture)
        Picker.Opened += (s, e) => ProposeToday();
        Refresh();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public DateTime? Date
    {
        get => (DateTime?)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }

    public string ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    // Le selecteur natif prend toute la taille du champ pour que tout le champ reponde a l'appui
    void OnHostSizeChanged(object? sender, EventArgs e)
    {
        if (Host.Width <= 0 || Host.Height <= 0) return;
        Picker.WidthRequest = Host.Width;
        Picker.HeightRequest = Host.Height;
    }

    // Le selecteur s'ouvre : sans date choisie, on propose aujourd'hui (choisir la date d'aujourd'hui ne declenche pas DateSelected)
    void OnPickerFocused(object? sender, FocusEventArgs e) => ProposeToday();

    void ProposeToday()
    {
        if (Date == null)
            Date = Picker.Date;
    }

    void OnDateSelected(object? sender, DateChangedEventArgs e)
    {
        Date = e.NewDate;
    }

    void Refresh()
    {
        if (Box == null) return;

        TitleLabel.Text = Title;
        TitleLabel.IsVisible = !string.IsNullOrEmpty(Title);

        if (Date is DateTime date)
        {
            ValueLabel.Text = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            SetStyle(ValueLabel, "FieldValueText");
            if (Picker.Date != date)
                Picker.Date = date;
        }
        else
        {
            ValueLabel.Text = Placeholder;
            SetStyle(ValueLabel, "FieldPlaceholderText");
        }

        bool hasError = !string.IsNullOrEmpty(ErrorMessage);
        MessageLabel.Text = ErrorMessage;
        MessageLabel.IsVisible = hasError;
        VisualStateManager.GoToState(Box, hasError ? "Error" : "Normal");
    }

    static void SetStyle(VisualElement element, string key)
    {
        if (Application.Current!.Resources.TryGetValue(key, out var value) && value is Style style)
            element.Style = style;
    }
}
