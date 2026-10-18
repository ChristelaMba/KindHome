using ObjCRuntime;
using UIKit;

namespace KindHome
{
    public class Program
    {
        // Point d'entree de l'application
        static void Main(string[] args)
        {
            // Classe AppDelegate de l'application
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
    }
}
