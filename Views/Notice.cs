namespace KindHome.Views;

public static class Notice
{
    // Petite fenetre partagee : sert quand l'appareil ne sait pas faire une action
    public static Task ShowAsync(string message)
    {
        return Shell.Current.DisplayAlertAsync("KindHome", message, "OK");
    }
}
