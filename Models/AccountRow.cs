using SQLite;

namespace KindHome.Models;

// Une ligne de la table des comptes dans SQLite
public class AccountRow
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Email { get; set; } = "";

    public string FullName { get; set; } = "";

    public string Phone { get; set; } = "";

    public string Password { get; set; } = "";

    // Le role est garde sous forme de nombre (0 Guest, 1 Donor, 2 Volunteer, 3 Organization)
    public int Role { get; set; }
}