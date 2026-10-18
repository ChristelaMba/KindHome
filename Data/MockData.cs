using KindHome.Models;

namespace KindHome.Data;

public static class MockData
{
    // ---------- Comptes d'exemple ----------
    // Les comptes crees a l'inscription s'ajoutent ici pendant que l'application est ouverte.
    // (remplie dans le constructeur statique, en bas du fichier, une fois les chiffres du profil prets)
    public static readonly List<UserAccount> Accounts = new();

    public static UserAccount? FindAccount(string email, string password)
    {
        return Accounts.FirstOrDefault(a =>
            string.Equals(a.Email, email.Trim(), StringComparison.OrdinalIgnoreCase) && a.Password == password);
    }

    // ---------- Orphelinats ----------
    // Seul le Centre d'Espoir a des valeurs venant de la maquette (ecran 07). Les autres orphelinats,
    // leurs textes, categories, notes, avis et actualites sont des exemples inventes.
    public static readonly Orphanage CentreEspoir = new()
    {
        Name = "Centre d'Espoir", District = "Bonapriso", DistanceKm = 3.2, ChildrenCount = 32,
        Categories = new() { "Éducation", "Hébergement", "Santé" },
        IsVerified = true, Rating = 4.6, ReviewCount = 12,
        About = "Le Centre d'Espoir accueille et accompagne des enfants orphelins et en situation difficile dans un cadre familial et sécurisé.",
        Photo = "orphanage_centre_espoir.jpg", CoverPhoto = "orphanage_centre_espoir_cover.jpg",
        MapX = 172, MapY = 118,
        Phone = "+237 6 90 00 00 01", Email = "contact@centre-espoir.example", Latitude = 4.0235, Longitude = 9.6960,
        News = new()
        {
            new NewsItem { Title = "Rentrée scolaire des enfants", Date = "12 septembre" },
            new NewsItem { Title = "Journée portes ouvertes", Date = "28 août" }
        }
    };

    public static readonly Orphanage PetitsCoeurs = new()
    {
        Name = "Les Petits Cœurs", District = "Akwa", DistanceKm = 4.7, ChildrenCount = 28,
        Categories = new() { "Éducation", "Santé" },
        IsVerified = true, Rating = 4.4, ReviewCount = 9,
        About = "Les Petits Cœurs offrent un accompagnement scolaire et un suivi médical régulier à des enfants de tous âges. L'équipe veille à ce que chacun grandisse dans la confiance et le respect.",
        Photo = "orphanage_petits_coeurs.jpg", CoverPhoto = "orphanage_petits_coeurs.jpg",
        MapX = 190, MapY = 44,
        Phone = "+237 6 90 00 00 02", Email = "contact@petits-coeurs.example", Latitude = 4.0510, Longitude = 9.7010,
        News = new()
        {
            new NewsItem { Title = "Campagne de vaccination terminée", Date = "5 septembre" },
            new NewsItem { Title = "Nouveaux livres pour la bibliothèque", Date = "19 août" }
        }
    };

    public static readonly Orphanage MaisonSourire = new()
    {
        Name = "La Maison du Sourire", District = "Bepanda", DistanceKm = 5.6, ChildrenCount = 16,
        Categories = new() { "Hébergement", "Santé" },
        IsVerified = true, Rating = 4.2, ReviewCount = 7,
        About = "La Maison du Sourire héberge une quinzaine d'enfants dans une grande maison de quartier. Les repas, les soins et les jeux y rythment des journées simples et chaleureuses.",
        Photo = "orphanage_maison_sourire.jpg", CoverPhoto = "orphanage_maison_sourire.jpg",
        MapX = 110, MapY = 160,
        Phone = "+237 6 90 00 00 03", Email = "contact@maison-sourire.example", Latitude = 4.0720, Longitude = 9.7330,
        News = new()
        {
            new NewsItem { Title = "Fête de fin d'année", Date = "30 août" },
            new NewsItem { Title = "Un grand merci à nos donateurs", Date = "14 août" }
        }
    };

    public static readonly Orphanage MaisonAnges = new()
    {
        Name = "Maison des Anges", District = "Bonapriso", DistanceKm = 1.8, ChildrenCount = 24,
        Categories = new() { "Éducation", "Hébergement" },
        IsVerified = true, Rating = 4.8, ReviewCount = 21,
        About = "La Maison des Anges accompagne vingt-quatre enfants, de l'école primaire au collège. Des bénévoles viennent chaque semaine pour le soutien scolaire et les activités du week-end.",
        Photo = "orphanage_maison_anges.jpg", CoverPhoto = "orphanage_maison_anges.jpg",
        MapX = 240, MapY = 150,
        Phone = "+237 6 90 00 00 04", Email = "contact@maison-anges.example", Latitude = 4.0210, Longitude = 9.7000,
        News = new()
        {
            new NewsItem { Title = "Atelier de lecture du samedi", Date = "9 septembre" },
            new NewsItem { Title = "Remise des cahiers de la rentrée", Date = "2 septembre" }
        }
    };

    public static readonly Orphanage EspoirDemain = new()
    {
        Name = "Espoir de Demain", District = "Akwa", DistanceKm = 4.5, ChildrenCount = 18,
        Categories = new() { "Éducation" },
        IsVerified = true, Rating = 4.5, ReviewCount = 15,
        About = "Espoir de Demain prépare les enfants à l'avenir grâce à l'école et à des ateliers de formation. L'orphelinat travaille main dans la main avec les enseignants du quartier.",
        Photo = "orphanage_espoir_demain.jpg", CoverPhoto = "orphanage_espoir_demain.jpg",
        MapX = 190, MapY = 196,
        Phone = "+237 6 90 00 00 05", Email = "contact@espoir-demain.example", Latitude = 4.0480, Longitude = 9.6980,
        News = new()
        {
            new NewsItem { Title = "Résultats d'examens : bravo aux élèves", Date = "7 septembre" },
            new NewsItem { Title = "Visite de l'inspecteur scolaire", Date = "23 août" }
        }
    };

    // Sixieme repere de la carte du PNG 06 : orphelinat invente
    public static readonly Orphanage EnfantsSoleil = new()
    {
        Name = "Les Enfants du Soleil", District = "Deido", DistanceKm = 6.3, ChildrenCount = 21,
        Categories = new() { "Hébergement" },
        IsVerified = true, Rating = 4.3, ReviewCount = 6,
        About = "Les Enfants du Soleil accueillent des enfants de 3 à 15 ans dans un cadre calme près du fleuve. La vie de groupe, le partage des tâches et le jeu y tiennent une grande place.",
        Photo = "orphanage_enfants_soleil.jpg", CoverPhoto = "orphanage_enfants_soleil.jpg",
        MapX = 66, MapY = 70,
        Phone = "+237 6 90 00 00 06", Email = "contact@enfants-soleil.example", Latitude = 4.0620, Longitude = 9.7190,
        News = new()
        {
            new NewsItem { Title = "Un nouveau jardin potager", Date = "1 septembre" },
            new NewsItem { Title = "Sortie à la plage", Date = "16 août" }
        }
    };

    // Ordre de la liste de l'annuaire (ecran 06) : les trois premiers comme sur le PNG, puis les autres
    public static readonly List<Orphanage> Orphanages = new()
    {
        CentreEspoir, PetitsCoeurs, MaisonSourire, MaisonAnges, EspoirDemain, EnfantsSoleil
    };
    // ---------- Besoins ----------
    // Le texte "Pourquoi c'est important ?" n'existe dans la maquette que pour le lait en poudre.
    // Pour les deux autres besoins, c'est une phrase de remplacement (a confirmer).
    public static readonly Need Milk = new()
    {
        Title = "Lait en poudre", Orphanage = CentreEspoir, Collected = 12, Total = 20, Unit = "unités",
        Category = "Alimentation",
        WhyImportant = "Le lait est essentiel pour la santé et la croissance des plus petits. Il aide à renforcer leur système immunitaire et leur apporte les nutriments dont ils ont besoin.",
        Photo = "need_milk.jpg", CoverPhoto = "need_milk_cover.jpg"
    };

    public static readonly Need SchoolSupplies = new()
    {
        Title = "Fournitures scolaires", Orphanage = MaisonAnges, Collected = 8, Total = 30, Unit = "kits",
        Category = "Scolarité",
        WhyImportant = "Les fournitures permettent aux enfants de suivre leurs cours dans de bonnes conditions et de réussir à l'école.",
        Photo = "need_notebook.jpg", CoverTile = "TileRose"
    };

    public static readonly Need Rice = new()
    {
        Title = "Riz", Orphanage = MaisonSourire, Collected = 3, Total = 5, Unit = "sacs",
        Category = "Alimentation",
        WhyImportant = "Le riz est la base des repas quotidiens des enfants accueillis.",
        Photo = "need_rice.jpg", CoverTile = "TileBeige"
    };

    // Ordre de la liste "Besoins prioritaires" (ecran 08)
    public static readonly List<Need> Needs = new() { Milk, SchoolSupplies, Rice };

    // ---------- Accueil donateur (ecran 05) ----------
    public static Need FeaturedNeed => Milk;
    public static Orphanage[] NearbyOnHome => new[] { MaisonAnges, EspoirDemain };

    // ---------- Profil de Christela Mba (ecran 15) ----------
    public static readonly ProfileSummary ChristelaProfile = new()
    {
        RoleText = "Donatrice & Bénévole", City = "Douala, Cameroun",
        DonationsCount = 3, MissionsCount = 2, FollowedCount = 4, ChildrenHelped = 48
    };

    // ---------- Tableau de bord de la Maison des Anges (ecran 16) ----------
    public static readonly OrganizationSummary MaisonAngesDashboard = new()
    {
        Name = "Maison des Anges", Children = 24, Volunteers = 5, DonationsThisMonth = 8,
        FeaturedNeed = SchoolSupplies,
        VolunteerRequestTitle = "Accompagnement scolaire", VolunteerRequestCount = 3,
        LastDonationNeed = Milk, LastDonationQuantity = 12
    };

    // ---------- Historique de Christela (page Mon historique) ----------
    static List<HistoryEntry> ChristelaDonations() => new()
    {
        new HistoryEntry { Title = "Lait en poudre - 4 unités", Orphanage = "Centre d'Espoir", Detail = "12 mars 2026", Status = "Remis" },
        new HistoryEntry { Title = "Fournitures scolaires - 10 kits", Orphanage = "Maison des Anges", Detail = "2 février 2026", Status = "Confirmé" },
        new HistoryEntry { Title = "Riz - 2 sacs", Orphanage = "La Maison du Sourire", Detail = "15 janvier 2026", Status = "Remis" }
    };

    static List<HistoryEntry> ChristelaMissions() => new()
    {
        new HistoryEntry { Title = "Soutien scolaire", Orphanage = "Centre d'Espoir", Detail = "Samedi, matin", Status = "Acceptée" },
        new HistoryEntry { Title = "Animation", Orphanage = "Maison des Anges", Detail = "Dimanche, après-midi", Status = "En attente" }
    };

    // ---------- Notifications d'exemple ----------
    static List<AppNotification> DonorNotifications() => new()
    {
        new AppNotification { Title = "Don confirmé", Text = "Le Centre d'Espoir a confirmé la remise de vos 4 unités de lait en poudre.", TimeText = "Il y a 2 h", Icon = "icon_check_circle.png" },
        new AppNotification { Title = "Candidature acceptée", Text = "La Maison des Anges a accepté votre candidature d'animation.", TimeText = "Il y a 5 h", Icon = "icon_people_20.png" },
        new AppNotification { Title = "Nouveau besoin près de vous", Text = "La Maison du Sourire a besoin de riz : il manque 2 sacs.", TimeText = "Il y a 1 j", Icon = "icon_bag_empty.png", IsRead = true },
        new AppNotification { Title = "Merci pour votre engagement", Text = "Grâce à vous, 48 enfants ont reçu une aide cette année.", TimeText = "Il y a 3 j", Icon = "icon_heart_impact.png", IsRead = true }
    };

    static List<AppNotification> OrganizationNotifications() => new()
    {
        new AppNotification { Title = "Nouvelle demande de bénévolat", Text = "Une personne souhaite proposer du soutien scolaire.", TimeText = "Il y a 2 h", Icon = "icon_people_20.png" },
        new AppNotification { Title = "Nouveau don reçu", Text = "4 unités de lait en poudre proposées.", TimeText = "Il y a 6 h", Icon = "icon_archive_plus.png" },
        new AppNotification { Title = "Besoin presque couvert", Text = "Fournitures scolaires : 8 kits sur 30 collectés.", TimeText = "Il y a 1 j", Icon = "icon_news.png", IsRead = true }
    };
    // ---------- Espace organisation de la Maison des Anges ----------
    static List<OrgRequest> MaisonAngesRequests() => new()
    {
        new OrgRequest { Name = "Aïcha Nkoulou", Domain = "Soutien scolaire", Schedule = "Samedi, matin" },
        new OrgRequest { Name = "Paul Essomba", Domain = "Soutien scolaire", Schedule = "Dimanche, matin" },
        new OrgRequest { Name = "Linda Fotso", Domain = "Soutien scolaire", Schedule = "Samedi, après-midi" }
    };

    static List<OrgDonation> MaisonAngesDonations() => new()
    {
        new OrgDonation { Need = Milk, Title = "Lait en poudre - 12 unités", Donor = "Don de Christela Mba", Date = "12 mars 2026", Status = "Remis" },
        new OrgDonation { Need = SchoolSupplies, Title = "Fournitures scolaires - 5 kits", Donor = "Don de Marc Tchoumi", Date = "20 septembre 2026", Status = "À confirmer" },
        new OrgDonation { Need = SchoolSupplies, Title = "Fournitures scolaires - 3 kits", Donor = "Don de Sandrine Eboa", Date = "4 octobre 2026", Status = "À confirmer" }
    };
    static MockData()
    {
        Accounts.Add(new UserAccount
        {
            FullName = "Christela Mba", Email = "christela@mail.com", Password = "kind1234",
            Phone = "6 99 99 99 99",
            Role = UserRole.Donor, Photo = "avatar_christela.jpg",
            Profile = ChristelaProfile,
            Donations = ChristelaDonations(),
            Missions = ChristelaMissions(),
            Notifications = DonorNotifications()
        });
        Accounts.Add(new UserAccount
        {
            FullName = "Maison des Anges", Email = "maison@mail.com", Password = "kind1234",
            Phone = "6 98 76 54 32",
            Role = UserRole.Organization,
            Organization = MaisonAngesDashboard,
            Notifications = OrganizationNotifications(),
            OrgRequests = MaisonAngesRequests(),
            OrgDonations = MaisonAngesDonations()
        });

        // Chaque besoin est lie a son orphelinat dans les deux sens
        foreach (var need in Needs)
            need.Orphanage.Needs.Add(need);
    }
}
