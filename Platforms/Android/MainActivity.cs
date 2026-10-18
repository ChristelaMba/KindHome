using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;

namespace KindHome
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Plein ecran : le contenu passe sous les barres systeme, les pages gerent les marges avec SafeAreaEdges
            if (Window != null)
            {
                WindowCompat.SetDecorFitsSystemWindows(Window, false);
#pragma warning disable CA1422
                Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
                Window.SetNavigationBarColor(Android.Graphics.Color.Transparent);
#pragma warning restore CA1422
            }

            SetSystemBarIcons(true);
        }

        // true : icones sombres (fond clair). false : icones blanches (fond sombre ou photo).
        public static void SetSystemBarIcons(bool darkIcons)
        {
            var window = Platform.CurrentActivity?.Window;
            if (window == null) return;

            var controller = WindowCompat.GetInsetsController(window, window.DecorView);
            if (controller == null) return;

            controller.AppearanceLightStatusBars = darkIcons;
            controller.AppearanceLightNavigationBars = darkIcons;
        }
    }
}
