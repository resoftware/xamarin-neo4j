using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Xamarin.Neo4j.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
