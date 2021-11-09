using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Net.Security;

namespace TlsCertificateLoader.Extensions
{
    public static class ListenOptionsExtensions
    {
        public static ListenOptions SetTlsHandshakeCallbackOptions(this ListenOptions listenOptions, TlsCertificateLoader tlsCertificateLoader)
        {
            return listenOptions.UseHttps(new TlsHandshakeCallbackOptions
            {
                OnConnection = context => new(new SslServerAuthenticationOptions
                {
                    ServerCertificateContext = tlsCertificateLoader.SslStreamCertificateContext
                })
            });
        }

        public static ListenOptions SetHttpsConnectionAdapterOptions(this ListenOptions listenOptions, TlsCertificateLoader tlsCertificateLoader)
        {
            return listenOptions.UseHttps(new HttpsConnectionAdapterOptions
            {
                ServerCertificateSelector = (context, dnsName) => tlsCertificateLoader.X509Certificate2Collection[0]
            });
        }
    }
}
