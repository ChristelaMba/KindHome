using KindHome.Models;

namespace KindHome.Views;

// Actions de l'appareil : appel, e-mail, itineraire, partage.
// Si l'appareil ne sait pas faire l'action, on affiche un message au lieu de planter.
public static class DeviceActions
{
    public static async Task CallAsync(Orphanage orphanage)
    {
        try
        {
            if (!PhoneDialer.Default.IsSupported)
            {
                await Notice.ShowAsync("Aucun composeur téléphonique n'est disponible sur cet appareil.");
                return;
            }
            PhoneDialer.Default.Open(orphanage.Phone);
        }
        catch (Exception)
        {
            await Notice.ShowAsync("Aucun composeur téléphonique n'est disponible sur cet appareil.");
        }
    }

    public static async Task EmailAsync(Orphanage orphanage, string subject)
    {
        try
        {
            if (!Email.Default.IsComposeSupported)
            {
                await Notice.ShowAsync("Aucune application d'e-mail n'est disponible sur cet appareil.");
                return;
            }
            var message = new EmailMessage
            {
                Subject = subject,
                To = new List<string> { orphanage.Email }
            };
            await Email.Default.ComposeAsync(message);
        }
        catch (Exception)
        {
            await Notice.ShowAsync("Aucune application d'e-mail n'est disponible sur cet appareil.");
        }
    }

    public static async Task RouteAsync(Orphanage orphanage)
    {
        try
        {
            var options = new MapLaunchOptions { Name = orphanage.Name };
            await Map.Default.OpenAsync(orphanage.Latitude, orphanage.Longitude, options);
        }
        catch (Exception)
        {
            await Notice.ShowAsync("Aucune application de cartes n'est disponible sur cet appareil.");
        }
    }

    // "Decouvre le Centre d'Espoir sur KindHome : Bonapriso, Douala, 32 enfants."
    public static async Task ShareAsync(Orphanage orphanage)
    {
        try
        {
            string name = orphanage.TheName;
            if (name.StartsWith("Les ") || name.StartsWith("Le ") || name.StartsWith("La "))
                name = char.ToLower(name[0]) + name.Substring(1);

            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = "KindHome",
                Text = "Découvre " + name + " sur KindHome : " + orphanage.District + ", Douala, " + orphanage.ChildrenText + "."
            });
        }
        catch (Exception)
        {
            await Notice.ShowAsync("Le partage n'est pas disponible sur cet appareil.");
        }
    }
}
