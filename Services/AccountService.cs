using KindHome.Data;
using KindHome.Models;

namespace KindHome.Services;

// Resultat de la verification e-mail + telephone
public enum ResetCheck
{
    Ok,
    NotFound,
    SampleAccount
}

public static class AccountService
{
    static bool tableReady;

    // Cree la table des comptes la premiere fois
    static async Task PrepareAsync()
    {
        if (tableReady) return;
        await AppDb.Db.CreateTableAsync<AccountRow>();
        tableReady = true;
    }

    // Vrai si un compte existe deja avec cet e-mail
    public static async Task<bool> EmailExistsAsync(string email)
    {
        string mail = email.Trim().ToLower();

        if (MockData.Accounts.Any(a => a.Email.ToLower() == mail))
            return true;

        await PrepareAsync();
        var row = await AppDb.Db.Table<AccountRow>().Where(r => r.Email == mail).FirstOrDefaultAsync();
        return row != null;
    }

    // Enregistre un nouveau compte dans la base (mot de passe hache)
    public static async Task SaveAsync(UserAccount account)
    {
        await PrepareAsync();

        // Le calcul du hash prend un peu de temps : on le fait hors du fil de l'ecran
        string plain = account.Password;
        string hash = await Task.Run(() => PasswordHasher.Hash(plain));

        var row = new AccountRow
        {
            Email = account.Email.Trim().ToLower(),
            FullName = account.FullName,
            Phone = account.Phone,
            Password = hash,
            Role = (int)account.Role
        };

        await AppDb.Db.InsertAsync(row);

        // Le mot de passe en clair n'est pas garde en memoire
        account.Password = "";
    }

    // Cherche le compte : d'abord les comptes d'exemple, puis la base
    public static async Task<UserAccount?> FindAsync(string email, string password)
    {
        // Comptes d'exemple (mot de passe ecrit dans MockData)
        var sample = MockData.FindAccount(email, password);
        if (sample != null && sample.Password.Length > 0)
            return sample;

        await PrepareAsync();

        string mail = email.Trim().ToLower();
        var row = await AppDb.Db.Table<AccountRow>().Where(r => r.Email == mail).FirstOrDefaultAsync();
        if (row == null)
            return null;

        bool ok = await Task.Run(() => PasswordHasher.Verify(password, row.Password));
        if (!ok)
            return null;

        // Ancien compte avec mot de passe en clair : on le remplace par un hash
        if (!PasswordHasher.IsHashed(row.Password))
        {
            row.Password = await Task.Run(() => PasswordHasher.Hash(password));
            await AppDb.Db.UpdateAsync(row);
        }

        // Compte deja connu en memoire : on le reprend (pas de doublon dans la liste)
        var existing = MockData.Accounts.FirstOrDefault(a => a.Email.ToLower() == mail);
        if (existing != null)
            return existing;

        var account = new UserAccount
        {
            FullName = row.FullName,
            Email = row.Email,
            Phone = row.Phone,
            Password = "",
            Role = (UserRole)row.Role
        };

        MockData.Accounts.Add(account);
        return account;
    }

    // Garde seulement les chiffres du telephone, sans le +237 : "+237 6 12 34 56 78" donne "612345678"
    public static string NormalizePhone(string phone)
    {
        string digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.Length == 12 && digits.StartsWith("237"))
            digits = digits.Substring(3);
        return digits;
    }

    // Verifie que l'e-mail et le telephone correspondent a un meme compte
    // (meme resultat NotFound pour un e-mail inconnu et pour un mauvais telephone)
    public static async Task<ResetCheck> CheckIdentityAsync(string email, string phone)
    {
        string mail = email.Trim().ToLower();
        string digits = NormalizePhone(phone);
        if (mail.Length == 0 || digits.Length == 0)
            return ResetCheck.NotFound;

        // Comptes d'exemple (mot de passe ecrit dans le code) : on ne peut pas le changer
        var sample = MockData.Accounts.FirstOrDefault(a => a.Password.Length > 0 && a.Email.Trim().ToLower() == mail);
        if (sample != null)
            return NormalizePhone(sample.Phone) == digits ? ResetCheck.SampleAccount : ResetCheck.NotFound;

        await PrepareAsync();

        var row = await AppDb.Db.Table<AccountRow>().Where(r => r.Email == mail).FirstOrDefaultAsync();
        if (row == null)
            return ResetCheck.NotFound;

        return NormalizePhone(row.Phone) == digits ? ResetCheck.Ok : ResetCheck.NotFound;
    }

    // Change le mot de passe (l'identite est verifiee une deuxieme fois)
    public static async Task ResetPasswordAsync(string email, string phone, string newPassword)
    {
        var check = await CheckIdentityAsync(email, phone);
        if (check != ResetCheck.Ok)
            throw new InvalidOperationException("Identite non verifiee");

        string mail = email.Trim().ToLower();
        var row = await AppDb.Db.Table<AccountRow>().Where(r => r.Email == mail).FirstOrDefaultAsync();
        if (row == null)
            throw new InvalidOperationException("Compte introuvable");

        string hash = await Task.Run(() => PasswordHasher.Hash(newPassword));
        row.Password = hash;
        await AppDb.Db.UpdateAsync(row);
    }
}
