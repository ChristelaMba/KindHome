using SQLite;

namespace KindHome.Data;

// Connexion unique a la base SQLite (comme MockData, c'est une classe statique)
public static class AppDb
{
    static SQLiteAsyncConnection? _db;

    public static SQLiteAsyncConnection Db
    {
        get
        {
            if (_db == null)
            {
                string path = Path.Combine(FileSystem.AppDataDirectory, "kindhome.db3");
                _db = new SQLiteAsyncConnection(path);
            }

            return _db;
        }
    }
}