namespace KindHome.Models;

// Une demande de benevolat recue par l'organisation (donnees d'exemple, statut modifiable en memoire)
public class OrgRequest
{
    public string Name { get; set; } = "";
    public string Domain { get; set; } = "";

    // "Samedi, matin"
    public string Schedule { get; set; } = "";

    // "En attente", "Acceptée" ou "Refusée"
    public string Status { get; set; } = "En attente";
}

// Un don recu par l'organisation (donnees d'exemple, statut modifiable en memoire)
public class OrgDonation
{
    public Need Need { get; set; } = null!;
    public string Title { get; set; } = "";

    // "Don de Christela Mba"
    public string Donor { get; set; } = "";
    public string Date { get; set; } = "";

    // "À confirmer" ou "Remis"
    public string Status { get; set; } = "À confirmer";
}