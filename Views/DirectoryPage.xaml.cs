using KindHome.Models;
using KindHome.ViewModels;

namespace KindHome.Views;

public partial class DirectoryPage : ContentPage
{
    // Repere de la carte : zone tactile de 44 x 44 avec le dessin du repere en bas
    class Pin
    {
        public Orphanage Orphanage = null!;
        public Grid Host = null!;
        public Image Picture = null!;
    }

    readonly DirectoryViewModel viewModel = new();
    readonly List<Pin> pins = new();
    bool loaded;

    public DirectoryPage()
    {
        InitializeComponent();
        BindingContext = viewModel;

        BuildPins();
        viewModel.ResultsChanged += (s, e) => UpdatePins();
        viewModel.ItemSelected += OnItemSelected;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
        MapArea.SizeChanged += (s, e) => UpdateMapLayout();
        UpdatePins();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Chargement simule de la carte a la premiere ouverture seulement
        if (loaded) return;
        loaded = true;
        await viewModel.LoadAsync();
    }

    void BuildPins()
    {
        foreach (var orphanage in Data.MockData.Orphanages)
        {
            var picture = new Image
            {
                Source = "map_pin.png",
                WidthRequest = 22,
                HeightRequest = 29,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                AnchorY = 1
            };
            var host = new Grid { WidthRequest = 44, HeightRequest = 44 };
            host.Children.Add(picture);
            SemanticProperties.SetDescription(host, orphanage.Name);

            var orphanageCopy = orphanage;
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                if (viewModel.OpenCommand.CanExecute(orphanageCopy))
                    viewModel.OpenCommand.Execute(orphanageCopy);
            };
            host.GestureRecognizers.Add(tap);

            // Le bas du repere (la pointe) est a (MapX + 76, MapY + 14) dans le dessin de la maquette
            AbsoluteLayout.SetLayoutBounds(host, new Rect(orphanage.MapX + 76 - 22, orphanage.MapY + 14 - 44, 44, 44));
            MapCanvas.Children.Add(host);
            pins.Add(new Pin { Orphanage = orphanage, Host = host, Picture = picture });
        }
    }

    // Seuls les reperes des orphelinats trouves restent affiches ; le repere choisi est mis en evidence
    void UpdatePins()
    {
        foreach (var pin in pins)
        {
            var item = viewModel.Results.FirstOrDefault(i => i.Orphanage == pin.Orphanage);
            pin.Host.IsVisible = item != null;

            bool selected = item != null && item == viewModel.Selected;
            pin.Picture.Source = selected ? "map_pin_selected.png" : "map_pin.png";
            pin.Picture.Scale = selected ? 1.3 : 1;
            pin.Host.ZIndex = selected ? 10 : 0;
        }
    }

    void OnItemSelected(object? sender, DirectoryItem item)
    {
        UpdatePins();
        ScrollToItem(item);
    }

    // La ligne du repere choisi est rendue visible dans la liste
    void ScrollToItem(DirectoryItem item)
    {
        int index = viewModel.Results.IndexOf(item);
        if (index < 0 || index >= ListStack.Children.Count) return;
        if (ListStack.Children[index] is VisualElement row)
            _ = ListScroll.ScrollToAsync(row, ScrollToPosition.MakeVisible, false);
    }

    void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DirectoryViewModel.IsMapMode))
        {
            UpdateModeButtons();
            UpdateMapLayout();
        }
    }

    // "Liste" : carte de 252 de haut comme la maquette. "Carte" : la carte remplit la place et cache la liste.
    void UpdateMapLayout()
    {
        if (viewModel.IsMapMode)
        {
            MapArea.VerticalOptions = LayoutOptions.Fill;
            MapArea.HeightRequest = -1;
            LocateButton.Margin = new Thickness(0, 0, 16, 16 + 52 + 12);

            if (MapArea.Width > 0 && MapArea.Height > 0)
            {
                // La carte est agrandie pour couvrir toute la zone
                double scale = Math.Max(MapArea.Width / 390, MapArea.Height / 252);
                MapCanvas.Scale = scale;
            }
        }
        else
        {
            MapArea.VerticalOptions = LayoutOptions.Start;
            MapArea.HeightRequest = 252;
            LocateButton.Margin = new Thickness(0, 0, 16, 16);
            MapCanvas.Scale = 1;
        }
    }

    void UpdateModeButtons()
    {
        bool map = viewModel.IsMapMode;
        Apply(ListButton, !map, map ? "icon_list_dark.png" : "icon_list_white.png");
        Apply(MapButton, map, map ? "icon_map_white.png" : "icon_map_dark.png");
    }

    static void Apply(Button button, bool selected, string icon)
    {
        string key = selected ? "SegmentButtonSelected" : "SegmentButton";
        if (Application.Current!.Resources.TryGetValue(key, out var value) && value is Style style)
            button.Style = style;
        button.ImageSource = icon;
    }
}
