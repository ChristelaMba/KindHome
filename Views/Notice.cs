namespace KindHome.Views;

public static class Notice
{
    // Message partage pour les boutons qui n'ont pas encore de page derriere
    public static Task ShowNotAvailableAsync()
    {
        return Shell.Current.DisplayAlertAsync("KindHome", "Cette fonction n'est pas encore disponible.", "OK");
    }
}
