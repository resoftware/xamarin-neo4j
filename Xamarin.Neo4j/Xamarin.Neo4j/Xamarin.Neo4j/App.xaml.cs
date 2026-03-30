using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Xamarin.Neo4j.Pages;
using Xamarin.Neo4j.Themes;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Xamarin.Neo4j
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            SetTheme(Current.RequestedTheme);

            MainPage = new NavigationPage(new RootPage());
        }

        private void SetTheme(AppTheme theme)
        {
            Resources = theme switch
            {
                AppTheme.Dark => new DarkTheme(),
                AppTheme.Light => new LightTheme(),

                _ => new LightTheme()
            };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
