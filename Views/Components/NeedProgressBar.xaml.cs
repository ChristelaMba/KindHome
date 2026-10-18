namespace KindHome.Views.Components;

// Barre de progression arrondie. Size : "Small" (6), "Medium" (8) ou "Large" (10, avec degrade).
public partial class NeedProgressBar : ContentView
{
    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(
        nameof(Progress), typeof(double), typeof(NeedProgressBar), 0.0,
        propertyChanged: (b, o, n) => ((NeedProgressBar)b).UpdateFill());

    public static readonly BindableProperty SizeProperty = BindableProperty.Create(
        nameof(Size), typeof(string), typeof(NeedProgressBar), "Small",
        propertyChanged: (b, o, n) => ((NeedProgressBar)b).ApplySize());

    public NeedProgressBar()
    {
        InitializeComponent();
        ApplySize();
    }

    // Entre 0 et 1
    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public string Size
    {
        get => (string)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    void ApplySize()
    {
        if (Track == null) return;
        SetStyle(Track, "ProgressTrack" + Size);
        SetStyle(Fill, "ProgressFill" + Size);
        UpdateFill();
    }

    static void SetStyle(Border border, string key)
    {
        if (Application.Current!.Resources.TryGetValue(key, out var value) && value is Style style)
            border.Style = style;
    }

    void OnHostSizeChanged(object? sender, EventArgs e) => UpdateFill();

    void UpdateFill()
    {
        if (Fill == null || Host.Width <= 0) return;
        double progress = Math.Clamp(Progress, 0, 1);
        Fill.WidthRequest = Host.Width * progress;
        Fill.IsVisible = progress > 0;
    }
}
