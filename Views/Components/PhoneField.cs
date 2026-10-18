namespace KindHome.Views.Components;

// Champ telephone : prefixe fixe "+237" suivi d'un separateur vertical, clavier numerique.
// Il reprend tous les etats de FormField (Normal, Actif, Valide, Desactive, Erreur).
public class PhoneField : FormField
{
    public PhoneField()
    {
        Prefix = "+237";
        Keyboard = Keyboard.Telephone;
    }
}
