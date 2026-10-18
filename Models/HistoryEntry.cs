namespace KindHome.Models;

// Une ligne de l'historique : un don ou une mission
public class HistoryEntry
{
    public string Title { get; set; } = "";
    public string Orphanage { get; set; } = "";

    // Date du don, ou jour et creneau de la mission
    public string Detail { get; set; } = "";

    // "Remis", "Confirmé", "Acceptée", "En attente"
    public string Status { get; set; } = "";

    // Ligne en plus (date d'envoi d'une mission), vide sinon
    public string ExtraLine { get; set; } = "";
    public bool HasExtraLine => ExtraLine.Length > 0;
}