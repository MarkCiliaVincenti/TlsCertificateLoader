using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Net.Security;

namespace TlsCertificateLoader.Extensions
{
    /// <summary>
    /// Extension method for <see cref="ListenOptions"/>.
    /// </summary>
    public static class ListenOptionsExtensions
    {
        /// <summary>
        /// Sets the TlsHandshakeCallbackOptions using a <see cref="SslStreamCertificateContext"/> provided by <see cref="TlsCertificateLoader"/>.
        /// </summary>
        /// <param name="listenOptions">The extended <see cref="ListenOptions"/> instance.</param>
        /// <param name="tlsCertificateLoader">The instance of <see cref="TlsCertificateLoader"/> to use.</param>
        /// <returns>The <see cref="ListenOptions"/>.</returns>
        public static ListenOptions SetTlsHandshakeCallbackOptions(this ListenOptions listenOptions, TlsCertificateLoader tlsCertificateLoader)
        {
            return listenOptions.UseHttps(new TlsHandshakeCallbackOptions
            {
                OnConnection = context => new(new SslServerAuthenticationOptions
                {
                    ServerCertificateContext = tlsCertificateLoader.GetCertificateHolder(context.ClientHelloInfo.ServerName).SslStreamCertificateContext
                })
            });
        }

        /// <summary>
        /// Sets the HttpsConnectionAdapterOptions using a <see cref="System.Security.Cryptography.X509Certificates.X509Certificate2"/> provided by <see cref="TlsCertificateLoader"/>.
        /// </summary>
        /// <param name="listenOptions">The extended <see cref="ListenOptions"/> instance.</param>
        /// <param name="tlsCertificateLoader">The instance of <see cref="TlsCertificateLoader"/> to use.</param>
        /// <returns>The <see cref="ListenOptions"/>.</returns>
        public static ListenOptions SetHttpsConnectionAdapterOptions(this ListenOptions listenOptions, TlsCertificateLoader tlsCertificateLoader)
        {
            return listenOptions.UseHttps(new HttpsConnectionAdapterOptions
            {
                ServerCertificateSelector = (_, hostname) => tlsCertificateLoader.GetCertificateHolder(hostname).X509Certificate2
            });
        }
    }
}
