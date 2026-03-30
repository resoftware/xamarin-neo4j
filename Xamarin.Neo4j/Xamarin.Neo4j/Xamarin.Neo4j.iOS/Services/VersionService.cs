//
// VersionService.cs
//
// Trevi Awater
// 02-03-2024
//
// © Xamarin.Neo4j.iOS
//

using Foundation;
using Microsoft.Maui.Controls;
using Xamarin.Neo4j.iOS.Services;
using Xamarin.Neo4j.Services.Interfaces;

[assembly: Dependency(typeof(VersionService))]
namespace Xamarin.Neo4j.iOS.Services
{
    public class VersionService : IVersionService
    {
        public string GetVersion() => NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"]?.ToString();

        public string GetBuild() => NSBundle.MainBundle.InfoDictionary["CFBundleVersion"]?.ToString();
    }
}
