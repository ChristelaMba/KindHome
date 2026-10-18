using System.Windows.Input;

namespace KindHome.Views.Components;

// Option radio : IsSelected (choisie) ou non. Command est lance a l'appui, avec CommandParameter.
public partial class RadioOption : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(RadioOption), "");

    public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
        nameof(IsSelected), typeof(bool), typeof(RadioOption), false,
        propertyChanged: (b, o, n) => ((RadioOption)b).Refresh());

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command), typeof(ICommand), typeof(RadioOption), null);

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        nameof(CommandParameter), typeof(object), typeof(RadioOption), null);

    public RadioOption()
    {
        InitializeComponent();
        Refresh();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
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

    void OnTapped(object? sender, TappedEventArgs e)
    {
        if (Command?.CanExecute(CommandParameter) == true)
            Command.Execute(CommandParameter);
    }

    void Refresh()
    {
        if (Box == null) return;

        VisualStateManager.GoToState(Box, IsSelected ? "Selected" : "Normal");
        Dot.IsVisible = IsSelected;
        SetStyle(Ring, IsSelected ? "RadioRingSelected" : "RadioRing");
        SetStyle(TextLabel, IsSelected ? "RadioLabelSelected" : "RadioLabel");
    }

    static void SetStyle(VisualElement element, string key)
    {
        if (Application.Current!.Resources.TryGetValue(key, out var value) && value is Style style)
            element.Style = style;
    }
}
