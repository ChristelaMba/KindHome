using KindHome.Data;
using KindHome.Models;

namespace KindHome.Services;

public static class NeedService
{
    // Appelee une seule fois, au demarrage de l'app
    public static async Task LoadAsync()
    {
        await AppDb.Db.CreateTableAsync<NeedRow>();

        var rows = await AppDb.Db.Table<NeedRow>().OrderBy(r => r.Id).ToListAsync();

        // Les besoins d'exemple (copie de la liste avant d'y ajouter les besoins publies)
        var samples = MockData.Needs.ToList();
        var matched = new HashSet<Need>();

        // Pour les organisations creees a l'inscription (elles ne sont pas dans la liste des orphelinats)
        var extraOrphanages = new Dictionary<string, Orphanage>();

        foreach (var row in rows)
        {
            // Un besoin d'exemple n'est reconnu qu'une seule fois : on reprend son numero et son compteur
            var known = samples.FirstOrDefault(n => !matched.Contains(n) && n.Title == row.Title && n.Orphanage.Name == row.OrphanageName);
            if (known != null)
            {
                matched.Add(known);
                known.Id = row.Id;
                known.Collected = row.Collected;
                continue;
            }

            // Sinon c'est un besoin publie : on le recree
            var orphanage = MockData.Orphanages.FirstOrDefault(o => o.Name == row.OrphanageName);
            if (orphanage == null)
            {
                if (!extraOrphanages.ContainsKey(row.OrphanageName))
                    extraOrphanages[row.OrphanageName] = new Orphanage { Name = row.OrphanageName };
                orphanage = extraOrphanages[row.OrphanageName];
            }

            var need = new Need
            {
                Id = row.Id,
                Title = row.Title,
                Orphanage = orphanage,
                Collected = row.Collected,
                Total = row.Total,
                Unit = row.Unit,
                Category = row.Category,
                WhyImportant = row.WhyImportant,
                Photo = row.Photo,
                CoverPhoto = row.CoverPhoto,
                CoverTile = row.CoverTile
            };

            MockData.Needs.Add(need);
            orphanage.Needs.Add(need);
        }

        // Les besoins d'exemple absents de la base (premier lancement, ou lancement interrompu) sont inseres
        foreach (var sample in samples)
        {
            if (matched.Contains(sample)) continue;

            var newRow = new NeedRow
            {
                Title = sample.Title,
                OrphanageName = sample.Orphanage.Name,
                Collected = sample.Collected,
                Total = sample.Total,
                Unit = sample.Unit,
                Category = sample.Category,
                WhyImportant = sample.WhyImportant,
                Photo = sample.Photo,
                CoverPhoto = sample.CoverPhoto,
                CoverTile = sample.CoverTile
            };

            await AppDb.Db.InsertAsync(newRow);
            sample.Id = newRow.Id;
        }
    }

    // Enregistre un nouveau besoin dans la base, puis l'ajoute aux listes en memoire
    public static async Task AddAsync(Need need)
    {
        var row = new NeedRow
        {
            Title = need.Title,
            OrphanageName = need.Orphanage.Name,
            Collected = need.Collected,
            Total = need.Total,
            Unit = need.Unit,
            Category = need.Category,
            WhyImportant = need.WhyImportant,
            Photo = need.Photo,
            CoverPhoto = need.CoverPhoto,
            CoverTile = need.CoverTile
        };

        await AppDb.Db.InsertAsync(row);
        need.Id = row.Id;

        MockData.Needs.Add(need);
        need.Orphanage.Needs.Add(need);
    }
}
