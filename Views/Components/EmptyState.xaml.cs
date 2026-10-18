using System.Windows.Input;

namespace KindHome.Views.Components;

// Vue d'etat vide : icone dans un cercle, titre, texte et un bouton d'action.
public partial class EmptyState : ContentView
{
    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon), typeof(ImageSource), typeof(EmptyState), null,
        propertyChanged: (b, o, n) => ((EmptyState)b).Refresh());

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(EmptyState), "");

    public static readonly BindableProperty MessageProperty = BindableProperty.Create(
        nameof(Message), typeof(string), typeof(EmptyState), "");

    public static readonly BindableProperty ButtonTextProperty = BindableProperty.Create(
        nameof(ButtonText), typeof(string), typeof(EmptyState), "",
        propertyChanged: (b, o, n) => ((EmptyState)b).Refresh());

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command), typeof(ICommand), typeof(EmptyState), null);

    // Cercle rose pour les erreurs (pas de connexion, localisation desactivee)
    public static readonly BindableProperty IsErrorProperty = BindableProperty.Create(
        nameof(IsError), typeof(bool), typeof(EmptyState), false,
        propertyChanged: (b, o, n) => ((EmptyState)b).Refresh());

    // Bouton d'action en contour vert (Reinitialiser les filtres) au lieu du bouton plein
    public static readonly BindableProperty SecondaryActionProperty = BindableProperty.Create(
        nameof(SecondaryAction), typeof(bool), typeof(EmptyState), false,
        propertyChanged: (b, o, n) => ((EmptyState)b).Refresh());

    public EmptyState()
    {
        InitializeComponent();
        Refresh();
    }

    public ImageSource? Icon
    {
        get => (ImageSource?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public bool IsError
    {
        get => (bool)GetValue(IsErrorProperty);
        set => SetValue(IsErrorProperty, value);
    }

    public bool SecondaryAction
    {
        get => (bool)GetValue(SecondaryActionProperty);
        set => SetValue(SecondaryActionProperty, value);
    }

    void OnActionClicked(object? sender, EventArgs e)
    {
        if (Command?.CanExecute(null) == true)
            Command.Execute(null);
    }

    void Refresh()
    {
        if (IconImage == null) return;
        IconImage.Source = Icon;
        Circle.IsVisible = Icon != null;
        // Sans texte de bouton, pas de bouton
        bool hasButton = !string.IsNullOrEmpty(ButtonText);
        ActionButton.IsVisible = hasButton && !SecondaryAction;
        SecondaryActionButton.IsVisible = hasButton && SecondaryAction;
        // Dans l'etat d'erreur, le bouton a 32 de marge de chaque cote
        ActionButton.Wide = IsError;

        string style = IsError ? "StateIconCircleError" : "StateIconCircle";
        if (Application.Current!.Resources.TryGetValue(style, out var value) && value is Style s)
            Circle.Style = s;
    }
}
