using KindHome.Data;
using KindHome.Models;

namespace KindHome.Services;

public static class VolunteerService
{
    static bool tableReady;

    // Cree la table des candidatures la premiere fois
    static async Task PrepareAsync()
    {
        if (tableReady) return;
        await AppDb.Db.CreateTableAsync<VolunteerRow>();
        tableReady = true;
    }

    // Enregistre la candidature dans la base
    public static async Task SaveAsync(VolunteerRequest request, string userEmail, string userName)
    {
        // Un besoin sans numero n'existe pas dans la base : on refuse
        if (request.Need == null || request.Need.Id <= 0)
            throw new InvalidOperationException("Besoin non enregistre dans la base");

        await PrepareAsync();

        var row = new VolunteerRow
        {
            UserEmail = userEmail.Trim().ToLower(),
            UserName = userName,
            NeedId = request.Need.Id,
            NeedTitle = request.Need.Title,
            OrphanageName = request.Orphanage.Name,
            Domains = string.Join("|", request.Domains),
            Days = string.Join(",", request.Days.OrderBy(d => d)),
            Slot = request.Slot,
            Motivation = request.Motivation,
            CharterAccepted = request.CharterAccepted,
            SentAt = DateTime.Now,
            Status = "En attente"
        };

        await AppDb.Db.InsertAsync(row);

        // L'organisation est prevenue (cette notification ne peut pas faire echouer la candidature)
        await NotificationService.NotifyNewVolunteerAsync(request.Orphanage.Name, userName, string.Join(", ", request.Domains));
    }
}
