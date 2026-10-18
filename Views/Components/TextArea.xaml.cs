namespace KindHome.Views.Components;

// Zone de texte multiligne : libelle, texte d'aide, nombre de caracteres maximum, hauteur du cadre.
public partial class TextArea : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(TextArea), "",
        propertyChanged: (b, o, n) => ((TextArea)b).Refresh());

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(TextArea), "",
        BindingMode.TwoWay);

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(TextArea), "");

    public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(
        nameof(MaxLength), typeof(int), typeof(TextArea), int.MaxValue);

    // Hauteur du cadre : 60 par defaut (message du don)
    public static readonly BindableProperty BoxHeightProperty = BindableProperty.Create(
        nameof(BoxHeight), typeof(double), typeof(TextArea), 60.0);

    // false : sans ombre (le message du don n'en a pas dans la maquette)
    public static readonly BindableProperty HasShadowProperty = BindableProperty.Create(
        nameof(HasShadow), typeof(bool), typeof(TextArea), true,
        propertyChanged: (b, o, n) => ((TextArea)b).Refresh());

    // Espace entre le libelle et la zone : 4 par defaut, 6 sur la page Contact
    public static readonly BindableProperty TitleSpacingProperty = BindableProperty.Create(
        nameof(TitleSpacing), typeof(double), typeof(TextArea), 4.0,
        propertyChanged: (b, o, n) => ((TextArea)b).Refresh());

    // Marge interieure en haut et en bas : 12 par defaut, 14 sur la page Contact
    public static readonly BindableProperty VerticalPaddingProperty = BindableProperty.Create(
        nameof(VerticalPadding), typeof(double), typeof(TextArea), 12.0,
        propertyChanged: (b, o, n) => ((TextArea)b).Refresh());

    // Etat Erreur : contour rouge, sans ombre (le message est affiche par la page)
    public static readonly BindableProperty IsErrorProperty = BindableProperty.Create(
        nameof(IsError), typeof(bool), typeof(TextArea), false,
        propertyChanged: (b, o, n) => ((TextArea)b).Refresh());

    public TextArea()
    {
        InitializeComponent();
        Refresh();
    }

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

    public int MaxLength
    {
        get => (int)GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public bool HasShadow
    {
        get => (bool)GetValue(HasShadowProperty);
        set => SetValue(HasShadowProperty, value);
    }

    public double TitleSpacing
    {
        get => (double)GetValue(TitleSpacingProperty);
        set => SetValue(TitleSpacingProperty, value);
    }

    public double VerticalPadding
    {
        get => (double)GetValue(VerticalPaddingProperty);
        set => SetValue(VerticalPaddingProperty, value);
    }

    public bool IsError
    {
        get => (bool)GetValue(IsErrorProperty);
        set => SetValue(IsErrorProperty, value);
    }

    public double BoxHeight
    {
        get => (double)GetValue(BoxHeightProperty);
        set => SetValue(BoxHeightProperty, value);
    }

    void Refresh()
    {
        if (TitleLabel == null) return;
        TitleLabel.Text = Title;
        TitleLabel.IsVisible = !string.IsNullOrEmpty(Title);

        TitleLabel.Margin = new Thickness(0, 0, 0, TitleSpacing);
        Box.Padding = new Thickness(18, VerticalPadding);
        VisualStateManager.GoToState(Box, IsError ? "Error" : "Normal");

        if (!HasShadow)
            Box.Shadow = null!;
    }
}
