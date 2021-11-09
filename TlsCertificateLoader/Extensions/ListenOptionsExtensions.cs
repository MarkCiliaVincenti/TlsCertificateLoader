using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace TlsCertificateLoader.Extensions
{
    public static class ListenOptionsExtensions
    {
        public static ListenOptions SetTlsHandshakeCallbackOptions(this ListenOptions listenOptions, X509Certificate2Collection x509Certificate2Collection)
        {
            return listenOptions.UseHttps(new TlsHandshakeCallbackOptions
            {
                OnConnection = context => new(new SslServerAuthenticationOptions
                {
                    ServerCertificateContext = SslStreamCertificateContext.Create(x509Certificate2Collection[0], x509Certificate2Collection, offline: true)
                })
            });
        }

        public static ListenOptions SetTlsHandshakeCallbackOptions(this ListenOptions listenOptions, TlsCertificateLoader tlsCertificateLoader)
        {
            return listenOptions.SetTlsHandshakeCallbackOptions(tlsCertificateLoader.X509Certificate2Collection);
        }

        public static ListenOptions SetHttpsConnectionAdapterOptions(this ListenOptions listenOptions, TlsCertificateLoader tlsCertificateLoader)
        {
            return listenOptions.SetHttpsConnectionAdapterOptions(tlsCertificateLoader.X509Certificate2Collection);
        }

        public static ListenOptions SetHttpsConnectionAdapterOptions(this ListenOptions listenOptions, X509Certificate2Collection x509Certificate2Collection)
        {
            return listenOptions.SetHttpsConnectionAdapterOptions(x509Certificate2Collection[0]);
        }

        public static ListenOptions SetHttpsConnectionAdapterOptions(this ListenOptions listenOptions, X509Certificate2 x509Certificate2)
        {
            return listenOptions.UseHttps(new HttpsConnectionAdapterOptions
            {
                ServerCertificateSelector = (context, dnsName) => new(x509Certificate2)
            });
        }
    }
}
