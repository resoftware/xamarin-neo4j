//
// TrustManagerService.cs
//
// Trevi Awater
// 14-03-2022
//
// © Xamarin.Neo4j.iOS
//

using Microsoft.Maui.Controls;
using Neo4j.Driver;
using Xamarin.Neo4j.iOS.Services;
using Xamarin.Neo4j.Services.Interfaces;

[assembly: Dependency(typeof(TrustManagerService))]
namespace Xamarin.Neo4j.iOS.Services
{
    public class TrustManagerService : ITrustManagerService
    {
        public TrustManager GetNativeTrustManager()
        {
            return new NativeTrustManager();
        }
    }
}
