using Microsoft.Extensions.Logging;

namespace KindHome
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    // InterRegular est la police par defaut (voir les styles Label, Entry, Button... dans Styles.xaml)
                    fonts.AddFont("Inter_18pt-Regular.ttf", "InterRegular");
                    fonts.AddFont("Inter_18pt-Medium.ttf", "InterMedium");
                    fonts.AddFont("Inter_18pt-SemiBold.ttf", "InterSemiBold");
                    fonts.AddFont("Inter_18pt-Bold.ttf", "InterBold");
                });

            RemoveEntryBorder();
            RemoveEditorBorder();
            UseExactLineHeight();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        // Sur Android, LineHeight multiplie la hauteur naturelle de la police : les lignes sont trop espacees.
        // On fixe donc la hauteur de ligne en pixels, comme le line-height de la maquette.
        static void UseExactLineHeight()
        {
            // Repris aussi quand le texte, la police ou la hauteur de ligne changent apres la creation
            foreach (var key in new[] { "ExactLineHeight", "LineHeight", "Font", "Text" })
                Microsoft.Maui.Handlers.LabelHandler.Mapper.AppendToMapping(key, SetExactLineHeight);
        }

        static void SetExactLineHeight(Microsoft.Maui.Handlers.ILabelHandler handler, ILabel label)
        {
#if ANDROID
            var textView = (Android.Widget.TextView)handler.PlatformView;
            // Pas de marge ajoutee autour de la police, comme sur le web
            textView.SetIncludeFontPadding(false);
            if (label.LineHeight < 0 || label.Font.Size <= 0) return;
            float density = textView.Context!.Resources!.DisplayMetrics!.Density;
            float wanted = (float)(density * label.Font.Size * label.LineHeight);
            textView.SetLineSpacing(0f, 1f);
            float natural = textView.LineHeight;
            textView.SetLineSpacing(wanted - natural, 1f);
#endif
        }

        // Meme chose pour les zones de texte multiligne (TextArea)
        static void RemoveEditorBorder()
        {
            Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("NoNativeBorder", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
                handler.PlatformView.SetPadding(0, 0, 0, 0);
#elif WINDOWS
                var box = handler.PlatformView;
                box.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                var clear = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                box.Resources["TextControlBackground"] = clear;
                box.Resources["TextControlBackgroundPointerOver"] = clear;
                box.Resources["TextControlBackgroundFocused"] = clear;
                box.Resources["TextControlBackgroundDisabled"] = clear;
                box.Resources["TextControlBorderBrush"] = clear;
                box.Resources["TextControlBorderBrushPointerOver"] = clear;
                box.Resources["TextControlBorderBrushFocused"] = clear;
                box.Resources["TextControlBorderBrushDisabled"] = clear;
                box.Padding = new Microsoft.UI.Xaml.Thickness(0);
#endif
            });
        }

        // Les champs de saisie sont dessines par FormField : on enleve le trait et le fond natifs de l'Entry
        static void RemoveEntryBorder()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoNativeBorder", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
                handler.PlatformView.SetPadding(0, 0, 0, 0);
#elif WINDOWS
                var box = handler.PlatformView;
                box.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                var clear = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                box.Resources["TextControlBackground"] = clear;
                box.Resources["TextControlBackgroundPointerOver"] = clear;
                box.Resources["TextControlBackgroundFocused"] = clear;
                box.Resources["TextControlBackgroundDisabled"] = clear;
                box.Resources["TextControlBorderBrush"] = clear;
                box.Resources["TextControlBorderBrushPointerOver"] = clear;
                box.Resources["TextControlBorderBrushFocused"] = clear;
                box.Resources["TextControlBorderBrushDisabled"] = clear;
                box.Padding = new Microsoft.UI.Xaml.Thickness(0);
#endif
            });
        }
    }
}
