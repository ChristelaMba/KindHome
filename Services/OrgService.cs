using System.Globalization;
using KindHome.Data;
using KindHome.Models;

namespace KindHome.Services;

public static class OrgService
{
    static readonly CultureInfo French = new("fr-FR");
    static readonly string[] DayNames = { "lundi", "mardi", "mercredi", "jeudi", "vendredi", "samedi", "dimanche" };

    static bool tablesReady;

    // Cree les tables si elles n'existent pas encore
    static async Task PrepareAsync()
    {
        if (tablesReady) return;
        await AppDb.Db.CreateTableAsync<DonationRow>();
        await AppDb.Db.CreateTableAsync<VolunteerRow>();
        await AppDb.Db.CreateTableAsync<AccountRow>();
        tablesReady = true;
    }

    // Candidatures recues par l'orphelinat, la plus recente en premiere
    public static async Task<List<OrgRequest>> GetRequestsAsync(string orgName)
    {
        await PrepareAsync();

        var rows = await AppDb.Db.Table<VolunteerRow>()
            .Where(r => r.OrphanageName == orgName)
            .OrderByDescending(r => r.Id)
            .ToListAsync();

        var list = new List<OrgRequest>();
        foreach (var row in rows)
        {
            list.Add(new OrgRequest
            {
                DbId = row.Id,
                Name = row.UserName.Length > 0 ? row.UserName : "Invité",
                Domain = row.Domains.Replace("|", ", "),
                Schedule = DaysText(row.Days) + ", " + row.Slot.ToLower(French),
                Status = row.Status
            });
        }

        return list;
    }

    // Dons recus par l'orphelinat, le plus recent en premier
    public static async Task<List<OrgDonation>> GetDonationsAsync(string orgName)
    {
        await PrepareAsync();

        var rows = await AppDb.Db.Table<DonationRow>()
            .Where(r => r.OrphanageName == orgName)
            .OrderByDescending(r => r.Id)
            .ToListAsync();

        var list = new List<OrgDonation>();
        foreach (var row in rows)
        {
            var need = MockData.Needs.FirstOrDefault(n => n.Id == row.NeedId);
            if (need == null)
                need = new Need { Title = row.NeedTitle, Orphanage = new Orphanage { Name = row.OrphanageName } };

            string amount;
            if (row.IsFinancial)
                amount = row.Amount.ToString("N0", French).Replace(' ', ' ').Replace(' ', ' ') + " FCFA";
            else
                amount = (row.Quantity + " " + need.Unit).Trim();

            string donor = await DonorNameAsync(row.UserEmail);

            list.Add(new OrgDonation
            {
                DbId = row.Id,
                Need = need,
                Title = row.NeedTitle + " - " + amount,
                Donor = donor.Length > 0 ? "Don de " + donor : "Don d'un invité",
                Date = row.SentAt.ToString("d MMMM yyyy", French),
                // En base, un don non confirme est "En attente" ; l'organisation le voit "À confirmer"
                Status = row.Status == "En attente" ? "À confirmer" : row.Status
            });
        }

        return list;
    }

    // Nombre de candidatures encore en attente
    public static async Task<int> CountPendingRequestsAsync(string orgName)
    {
        await PrepareAsync();
        return await AppDb.Db.Table<VolunteerRow>()
            .Where(r => r.OrphanageName == orgName && r.Status == "En attente")
            .CountAsync();
    }

    // Accepter ou refuser une candidature (le benevole est prevenu)
    public static async Task SetRequestStatusAsync(int id, string status)
    {
        await PrepareAsync();
        int changed = await AppDb.Db.ExecuteAsync("UPDATE VolunteerRow SET Status = ? WHERE Id = ?", status, id);
        if (changed == 0)
            throw new InvalidOperationException("Candidature introuvable");

        // Le statut est enregistre : la notification est secondaire et ne doit pas annuler le changement
        try
        {
            var row = await AppDb.Db.Table<VolunteerRow>().Where(r => r.Id == id).FirstOrDefaultAsync();
            if (row == null) return;

            string domains = row.Domains.Replace("|", ", ");

            if (status == "Acceptée")
            {
                await NotificationService.AddAsync(row.UserEmail, "Candidature acceptée",
                    row.OrphanageName + " a accepté votre candidature : " + domains + ".", "icon_people_20.png");
            }
            else if (status == "Refusée")
            {
                await NotificationService.AddAsync(row.UserEmail, "Candidature refusée",
                    row.OrphanageName + " n'a pas pu retenir votre candidature : " + domains + ".", "icon_alert_circle.png");
            }
        }
        catch (Exception)
        {
            // La notification est secondaire : on ignore l'echec
        }
    }

    // Confirmer la remise d'un don (le donateur est prevenu)
    public static async Task SetDonationStatusAsync(int id, string status)
    {
        await PrepareAsync();
        int changed = await AppDb.Db.ExecuteAsync("UPDATE DonationRow SET Status = ? WHERE Id = ?", status, id);
        if (changed == 0)
            throw new InvalidOperationException("Don introuvable");

        if (status != "Remis") return;

        // Le statut est enregistre : la notification est secondaire et ne doit pas annuler le changement
        try
        {
            var row = await AppDb.Db.Table<DonationRow>().Where(r => r.Id == id).FirstOrDefaultAsync();
            if (row == null) return;

            await NotificationService.AddAsync(row.UserEmail, "Don remis",
                row.OrphanageName + " a confirmé la remise de votre don : " + row.NeedTitle + ".", "icon_check_circle.png");
        }
        catch (Exception)
        {
            // La notification est secondaire : on ignore l'echec
        }
    }

    // Nom du donateur : comptes d'exemple, puis comptes de la base (vide pour un invite)
    static async Task<string> DonorNameAsync(string email)
    {
        if (email.Length == 0) return "";

        var sample = MockData.Accounts.FirstOrDefault(a => a.Email.ToLower() == email);
        if (sample != null) return sample.FullName;

        var row = await AppDb.Db.Table<AccountRow>().Where(r => r.Email == email).FirstOrDefaultAsync();
        return row?.FullName ?? "un donateur";
    }

    // "5,6" devient "Samedi, dimanche"
    static string DaysText(string days)
    {
        var names = days.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(d => int.TryParse(d, out int n) && n >= 0 && n < 7 ? DayNames[n] : "")
            .Where(d => d.Length > 0);

        string text = string.Join(", ", names);
        return text.Length == 0 ? "" : char.ToUpper(text[0]) + text.Substring(1);
    }
}
