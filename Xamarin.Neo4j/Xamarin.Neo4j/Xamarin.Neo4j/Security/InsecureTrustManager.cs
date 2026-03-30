using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Neo4j.Driver;

namespace Xamarin.Neo4j.Security
{
    public class InsecureTrustManager : TrustManager
    {
        public override bool ValidateServerCertificate(Uri uri, X509Certificate2 certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
