namespace KindHome.Views.Components;

// Liste deroulante arrondie avec fleche : appui = selecteur natif. SelectedItem est la valeur choisie.
public partial class PickerField : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(PickerField), "",
        propertyChanged: (b, o, n) => ((PickerField)b).Refresh());

    public static readonly BindableProperty ItemsProperty = BindableProperty.Create(
        nameof(Items), typeof(IList<string>), typeof(PickerField), null,
        propertyChanged: (b, o, n) => ((PickerField)b).UpdateItems());

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem), typeof(string), typeof(PickerField), null,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((PickerField)b).Refresh());

    public PickerField()
    {
        InitializeComponent();
        Refresh();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public IList<string>? Items
    {
        get => (IList<string>?)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public string? SelectedItem
    {
        get => (string?)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    // Le selecteur natif prend toute la taille du champ pour que tout le champ reponde a l'appui
    void OnHostSizeChanged(object? sender, EventArgs e)
    {
        if (Host.Width <= 0 || Host.Height <= 0) return;
        Selector.WidthRequest = Host.Width;
        Selector.HeightRequest = Host.Height;
    }

    void UpdateItems()
    {
        Selector.ItemsSource = Items?.ToList();
        Refresh();
    }

    void OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (Selector.SelectedItem is string item && item != SelectedItem)
            SelectedItem = item;
    }

    void Refresh()
    {
        if (Box == null) return;

        TitleLabel.Text = Title;
        TitleLabel.IsVisible = !string.IsNullOrEmpty(Title);
        ValueLabel.Text = SelectedItem ?? "";

        if (SelectedItem != null && Selector.SelectedItem as string != SelectedItem)
            Selector.SelectedItem = SelectedItem;
    }
}