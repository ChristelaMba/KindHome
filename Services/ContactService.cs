using KindHome.Data;
using KindHome.Models;

namespace KindHome.Services;

public static class ContactService
{
    static bool tableReady;

    // Cree la table des messages la premiere fois
    static async Task PrepareAsync()
    {
        if (tableReady) return;
        await AppDb.Db.CreateTableAsync<ContactRow>();
        tableReady = true;
    }

    // Enregistre le message dans la base
    public static async Task SaveAsync(Orphanage orphanage, string subject, string message, string userEmail)
    {
        await PrepareAsync();

        var row = new ContactRow
        {
            UserEmail = userEmail.Trim().ToLower(),
            OrphanageName = orphanage.Name,
            Subject = subject,
            Message = message.Trim(),
            SentAt = DateTime.Now
        };

        await AppDb.Db.InsertAsync(row);
    }
}