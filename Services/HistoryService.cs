using System.Globalization;
using KindHome.Data;
using KindHome.Models;

namespace KindHome.Services;

public static class HistoryService
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
        tablesReady = true;
    }

    static string Clean(string email) => email.Trim().ToLower();

    // Dons de la personne, le plus recent en premier
    public static async Task<List<HistoryEntry>> GetDonationsAsync(string email)
    {
        await PrepareAsync();
        string mail = Clean(email);

        var rows = await AppDb.Db.Table<DonationRow>()
            .Where(r => r.UserEmail == mail)
            .OrderByDescending(r => r.Id)
            .ToListAsync();

        var list = new List<HistoryEntry>();
        foreach (var row in rows)
        {
            string amount;
            if (row.IsFinancial)
            {
                amount = row.Amount.ToString("N0", French).Replace('\u202f', ' ').Replace('\u00a0', ' ') + " FCFA";
            }
            else
            {
                // L'unite se retrouve grace au numero du besoin
                string unit = MockData.Needs.FirstOrDefault(n => n.Id == row.NeedId)?.Unit ?? "";
                amount = (row.Quantity + " " + unit).Trim();
            }

            list.Add(new HistoryEntry
            {
                Title = row.NeedTitle + " - " + amount,
                Orphanage = row.OrphanageName,
                Detail = row.SentAt.ToString("d MMMM yyyy", French),
                Status = row.Status
            });
        }

        return list;
    }

    // Candidatures de la personne, la plus recente en premiere
    public static async Task<List<HistoryEntry>> GetMissionsAsync(string email)
    {
        await PrepareAsync();
        string mail = Clean(email);

        var rows = await AppDb.Db.Table<VolunteerRow>()
            .Where(r => r.UserEmail == mail)
            .OrderByDescending(r => r.Id)
            .ToListAsync();

        var list = new List<HistoryEntry>();
        foreach (var row in rows)
        {
            list.Add(new HistoryEntry
            {
                Title = row.Domains.Replace("|", ", "),
                Orphanage = row.OrphanageName,
                Detail = DaysText(row.Days) + ", " + row.Slot.ToLower(French),
                Status = row.Status,
                ExtraLine = row.SentAt.ToString("d MMMM yyyy", French)
            });
        }

        return list;
    }

    public static async Task<int> CountDonationsAsync(string email)
    {
        await PrepareAsync();
        string mail = Clean(email);
        return await AppDb.Db.Table<DonationRow>().Where(r => r.UserEmail == mail).CountAsync();
    }

    public static async Task<int> CountMissionsAsync(string email)
    {
        await PrepareAsync();
        string mail = Clean(email);
        return await AppDb.Db.Table<VolunteerRow>().Where(r => r.UserEmail == mail).CountAsync();
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