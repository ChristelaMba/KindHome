using KindHome.Data;
using KindHome.Models;

namespace KindHome.Services;

public static class DonationService
{
    static bool tableReady;

    // Cree la table des dons la premiere fois
    static async Task PrepareAsync()
    {
        if (tableReady) return;
        await AppDb.Db.CreateTableAsync<DonationRow>();
        tableReady = true;
    }

    // Enregistre le don dans la base, puis fait monter le compteur du besoin (don en nature seulement)
    public static async Task SaveAsync(Donation donation, string userEmail)
    {
        await PrepareAsync();

        var need = donation.Need;

        // Un besoin sans numero n'existe pas dans la base : on refuse
        if (need.Id <= 0)
            throw new InvalidOperationException("Besoin non enregistre dans la base");

        var row = new DonationRow
        {
            UserEmail = userEmail.Trim().ToLower(),
            NeedId = need.Id,
            NeedTitle = need.Title,
            OrphanageName = need.Orphanage.Name,
            IsFinancial = donation.IsFinancial,
            Quantity = donation.Quantity,
            Amount = donation.Amount,
            OnSiteDelivery = donation.OnSiteDelivery,
            Date = donation.Date,
            Message = donation.Message,
            SentAt = DateTime.Now,
            Status = "En attente"
        };

        // Un don en nature fait monter le compteur du besoin (jamais au-dessus du total)
        int newValue = need.Collected;
        if (!donation.IsFinancial)
            newValue = Math.Min(need.Total, need.Collected + donation.Quantity);

        // Le don et le compteur sont enregistres ensemble : si l'un echoue, rien n'est garde
        await AppDb.Db.RunInTransactionAsync(conn =>
        {
            conn.Insert(row);

            if (!donation.IsFinancial)
                conn.Execute("UPDATE NeedRow SET Collected = ? WHERE Id = ?", newValue, need.Id);
        });

        need.Collected = newValue;

        // L'organisation est prevenue (cette notification ne peut pas faire echouer le don)
        await NotificationService.NotifyNewDonationAsync(need.Orphanage.Name, need.Title, donation.AmountText, userEmail);
    }
}
