using KindHome.Data;
using KindHome.Models;

namespace KindHome.Services;

public static class NotificationService
{
    static bool tablesReady;

    // Cree les tables si elles n'existent pas encore
    static async Task PrepareAsync()
    {
        if (tablesReady) return;
        await AppDb.Db.CreateTableAsync<NotificationRow>();
        await AppDb.Db.CreateTableAsync<AccountRow>();
        tablesReady = true;
    }

    // Enregistre une notification pour une personne (ne plante jamais, pour ne pas bloquer un envoi)
    public static async Task AddAsync(string userEmail, string title, string text, string icon)
    {
        string mail = userEmail.Trim().ToLower();
        if (mail.Length == 0) return;

        try
        {
            await PrepareAsync();

            var row = new NotificationRow
            {
                UserEmail = mail,
                Title = title,
                Text = text,
                Icon = icon,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            await AppDb.Db.InsertAsync(row);
        }
        catch (Exception)
        {
            // La notification est secondaire : on ignore l'echec
        }
    }

    // Ajoute les notifications de la base en tete de la liste du compte (les exemples restent en dessous)
    public static async Task LoadForUserAsync(UserAccount user)
    {
        string mail = user.Email.Trim().ToLower();
        if (mail.Length == 0) return;

        await PrepareAsync();

        var rows = await AppDb.Db.Table<NotificationRow>()
            .Where(r => r.UserEmail == mail)
            .OrderByDescending(r => r.Id)
            .ToListAsync();

        // La personne connectee a change pendant la lecture : on ignore le resultat
        if (UserSession.CurrentUser != user) return;

        // Notifications deja lues en memoire (la mise a jour de la base peut ne pas etre finie)
        var readInMemory = user.Notifications
            .Where(n => n.DbId > 0 && n.IsRead)
            .Select(n => n.DbId)
            .ToHashSet();

        // On retire les anciennes notifications de la base (pas de doublon), puis on remet les a jour
        user.Notifications.RemoveAll(n => n.DbId > 0);

        var loaded = new List<AppNotification>();
        foreach (var row in rows)
        {
            // IsRead est affecte AVANT DbId : tant que DbId vaut 0, le setter n'ecrit rien en base
            loaded.Add(new AppNotification
            {
                IsRead = row.IsRead || readInMemory.Contains(row.Id),
                DbId = row.Id,
                Title = row.Title,
                Text = row.Text,
                TimeText = TimeAgo(row.CreatedAt),
                Icon = row.Icon
            });
        }

        user.Notifications.InsertRange(0, loaded);
    }

    // Enregistre "lu" en base, sans bloquer l'ecran
    public static void MarkReadInBackground(int id)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await PrepareAsync();
                await AppDb.Db.ExecuteAsync("UPDATE NotificationRow SET IsRead = 1 WHERE Id = ?", id);
            }
            catch (Exception)
            {
                // Pas grave : la notification restera non lue
            }
        });
    }

    // Une organisation qui recoit un nouveau don
    public static async Task NotifyNewDonationAsync(string orphanageName, string needTitle, string amountText, string donorEmail)
    {
        try
        {
            string orgEmail = await OrgEmailAsync(orphanageName);
            if (orgEmail.Length == 0) return;

            string donor = await UserNameAsync(donorEmail);
            if (donor.Length == 0) donor = "un invité";

            await AddAsync(orgEmail, "Nouveau don reçu",
                needTitle + " - " + amountText + " proposé par " + donor + ".", "icon_archive_plus.png");
        }
        catch (Exception)
        {
            // La notification est secondaire : on ignore l'echec
        }
    }

    // Une organisation qui recoit une nouvelle candidature de benevole
    public static async Task NotifyNewVolunteerAsync(string orphanageName, string volunteerName, string domains)
    {
        try
        {
            string orgEmail = await OrgEmailAsync(orphanageName);
            if (orgEmail.Length == 0) return;

            string name = volunteerName.Trim().Length > 0 ? volunteerName.Trim() : "Un invité";

            await AddAsync(orgEmail, "Nouvelle demande de bénévolat",
                name + " souhaite aider : " + domains + ".", "icon_people_20.png");
        }
        catch (Exception)
        {
            // La notification est secondaire : on ignore l'echec
        }
    }

    // Une organisation qui recoit un nouveau message de contact (le debut du message est dans la notification)
    public static async Task NotifyNewContactAsync(string orphanageName, string senderEmail, string subject, string message)
    {
        try
        {
            string orgEmail = await OrgEmailAsync(orphanageName);
            if (orgEmail.Length == 0) return;

            string sender = await UserNameAsync(senderEmail);
            if (sender.Length == 0) sender = "Un invité";

            string preview = message.Trim();
            if (preview.Length > 80) preview = preview.Substring(0, 80) + "...";

            await AddAsync(orgEmail, "Nouveau message",
                sender + " - " + subject + " : " + preview, "icon_message.png");
        }
        catch (Exception)
        {
            // La notification est secondaire : on ignore l'echec
        }
    }

    // E-mail du compte de l'organisation (son nom est celui de l'orphelinat), vide si aucun compte
    static async Task<string> OrgEmailAsync(string orphanageName)
    {
        string name = orphanageName.Trim().ToLower();
        if (name.Length == 0) return "";

        var sample = MockData.Accounts.FirstOrDefault(a => a.Role == UserRole.Organization && a.FullName.Trim().ToLower() == name);
        if (sample != null) return sample.Email.Trim().ToLower();

        await PrepareAsync();

        int orgRole = (int)UserRole.Organization;
        var rows = await AppDb.Db.Table<AccountRow>().Where(r => r.Role == orgRole).ToListAsync();
        var row = rows.FirstOrDefault(r => r.FullName.Trim().ToLower() == name);
        return row?.Email.Trim().ToLower() ?? "";
    }

    // Nom de la personne a partir de son e-mail (vide pour un invite)
    static async Task<string> UserNameAsync(string email)
    {
        string mail = email.Trim().ToLower();
        if (mail.Length == 0) return "";

        var sample = MockData.Accounts.FirstOrDefault(a => a.Email.Trim().ToLower() == mail);
        if (sample != null) return sample.FullName;

        await PrepareAsync();
        var row = await AppDb.Db.Table<AccountRow>().Where(r => r.Email == mail).FirstOrDefaultAsync();
        return row?.FullName ?? "";
    }

    // "Il y a 5 min", "Il y a 2 h", "Il y a 3 j"
    static string TimeAgo(DateTime date)
    {
        var span = DateTime.Now - date;
        if (span.TotalMinutes < 1) return "À l'instant";
        if (span.TotalMinutes < 60) return "Il y a " + (int)span.TotalMinutes + " min";
        if (span.TotalHours < 24) return "Il y a " + (int)span.TotalHours + " h";
        return "Il y a " + (int)span.TotalDays + " j";
    }
}
