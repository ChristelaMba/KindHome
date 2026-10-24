using SQLite;

namespace KindHome.Models;

// Une ligne de la table des besoins dans SQLite
public class NeedRow
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; } = "";

    // L'orphelinat est retrouve grace a son nom
    public string OrphanageName { get; set; } = "";

    public int Collected { get; set; }

    public int Total { get; set; }

    public string Unit { get; set; } = "";

    public string Category { get; set; } = "";

    public string WhyImportant { get; set; } = "";

    public string Photo { get; set; } = "";

    public string CoverPhoto { get; set; } = "";

    public string CoverTile { get; set; } = "";
}