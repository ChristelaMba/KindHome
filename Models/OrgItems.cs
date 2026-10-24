namespace KindHome.Models;

// Une demande de benevolat recue par l'organisation (statut modifiable)
public class OrgRequest
{
    // Numero de la ligne dans la base (0 pour les donnees d'exemple)
    public int DbId { get; set; }

    public string Name { get; set; } = "";
    public string Domain { get; set; } = "";

    // "Samedi, matin"
    public string Schedule { get; set; } = "";

    // "En attente", "Acceptée" ou "Refusée"
    public string Status { get; set; } = "En attente";
}

// Un don recu par l'organisation (statut modifiable)
public class OrgDonation
{
    // Numero de la ligne dans la base (0 pour les donnees d'exemple)
    public int DbId { get; set; }

    public Need Need { get; set; } = null!;
    public string Title { get; set; } = "";

    // "Don de Christela Mba"
    public string Donor { get; set; } = "";
    public string Date { get; set; } = "";

    // "À confirmer" ou "Remis"
    public string Status { get; set; } = "À confirmer";
}