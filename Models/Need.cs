namespace KindHome.Models;

public class Need
{
    public string Title { get; set; } = "";
    public Orphanage Orphanage { get; set; } = null!;
    public int Collected { get; set; }
    public int Total { get; set; }
    public string Unit { get; set; } = "";
    public string Category { get; set; } = "";
    public string WhyImportant { get; set; } = "";

    // Noms des fichiers dans Resources/Images
    public string Photo { get; set; } = "";
    public string CoverPhoto { get; set; } = "";

    // Sans photo de couverture : nom d'une couleur de Colors.xaml (TileRose, TileBeige) pour le fond du haut
    public string CoverTile { get; set; } = "";
    public bool HasCoverPhoto => CoverPhoto != "";

    // Un besoin publie par une organisation n'a pas de photo : la carte montre une tuile avec une icone
    public bool HasPhoto => Photo != "";
    public bool HasNoPhoto => Photo == "";

    // "20 unites necessaires"
    public string TotalText => Total + " " + Unit + " nécessaires";

    // "12 deja collectees"
    public string CollectedText => Collected + " déjà collectées";

    // Ce qu'il reste a couvrir (20 moins 12 = 8)
    public int Remaining => Total - Collected;

    // 0.6 pour 12 sur 20
    public double Progress => Total == 0 ? 0 : (double)Collected / Total;

    // "12 / 20 unites"
    public string ProgressText => Collected + " / " + Total + " " + Unit;
}
