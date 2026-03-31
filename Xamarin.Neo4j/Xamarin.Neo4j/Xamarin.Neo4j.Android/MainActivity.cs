using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;

namespace Xamarin.Neo4j.Android
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation |
                               ConfigChanges.UiMode | ConfigChanges.ScreenLayout |
                               ConfigChanges.SmallestScreenSize | ConfigChanges.Density
    )]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnResume()
        {
            base.OnResume();

            // Re-apply theme and nav bar colours when returning to the foreground.
            // ConfigChanges.UiMode prevents Activity recreation on system theme change,
            // so RequestedThemeChanged may not fire reliably — we re-derive the theme
            // from the current configuration instead.
            if (IPlatformApplication.Current?.Application is App app)
            {
                var nightMode = Resources.Configuration.UiMode & UiMode.NightMask;
                var theme = nightMode == UiMode.NightYes
                    ? AppTheme.Dark
                    : AppTheme.Light;

                MainThread.BeginInvokeOnMainThread(() => app.UpdateTheme(theme));
            }
        }
    }
}
