using System.Security.Cryptography;
using System.Text;

namespace KindHome.Services;

// Hachage des mots de passe (PBKDF2 avec sel, inclus dans .NET)
public static class PasswordHasher
{
    const string Prefix = "pbkdf2";
    const int Iterations = 100000;
    const int MinIterations = 10000;
    const int MaxIterations = 1000000;
    const int SaltSize = 16;
    const int HashSize = 32;

    // Donne un texte du genre "pbkdf2$100000$sel$hash"
    public static string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return Prefix + "$" + Iterations + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
    }

    // Vrai si le texte enregistre est deja un hash (faux pour un ancien mot de passe en clair)
    public static bool IsHashed(string stored)
    {
        return stored.StartsWith(Prefix + "$");
    }

    public static bool Verify(string password, string stored)
    {
        // Ancien compte : mot de passe enregistre en clair
        if (!IsHashed(stored))
        {
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(password),
                Encoding.UTF8.GetBytes(stored));
        }

        string[] parts = stored.Split('$');
        if (parts.Length != 4)
            return false;

        // Nombre d'iterations dans une plage raisonnable
        if (!int.TryParse(parts[1], out int iterations) || iterations < MinIterations || iterations > MaxIterations)
            return false;

        byte[] salt;
        byte[] expected;
        try
        {
            salt = Convert.FromBase64String(parts[2]);
            expected = Convert.FromBase64String(parts[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        // Sel et hash doivent avoir la bonne taille (un hash vide est refuse)
        if (salt.Length != SaltSize || expected.Length != HashSize)
            return false;

        byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, HashSize);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
