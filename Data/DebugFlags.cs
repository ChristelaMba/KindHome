namespace KindHome.Data;

public static class DebugFlags
{
    // Page affichee au demarrage. Mettre le nom d'une page, par exemple "HomePage" ou "NeedDetailPage".
    public static string StartRoute = "OnboardingPage";

    // true : meme le compte maison@mail.com voit l'etat "Aucun besoin publie" dans l'espace organisation
    public static bool SimulateNoOrgNeeds = false;

    // true : la page Besoins affiche l'etat "Pas de connexion"
    public static bool SimulateNetworkError = false;

    // true : l'envoi (don, candidature de benevole, message de contact) echoue apres la seconde d'attente
    public static bool SimulateSendFailure = false;

    // true : l'annuaire affiche l'etat "Localisation desactivee"
    public static bool SimulateLocationDenied = false;
}











