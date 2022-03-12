using NetworkPorts;
using System.Net;

namespace TlsCertificateLoader.Models
{
    /// <summary>
    /// TlsCertificateLoader middleware for Certbot web options
    /// </summary>
    public class TlsCertificateLoaderWebOptions
    {
        /// <summary>
        /// The <see cref="System.Net.IPAddress"/> to listen on. Defaults to <see cref="IPAddress.Any"/>.
        /// </summary>
        public IPAddress IPAddress { get; set; } = IPAddress.Any;

        /// <summary>
        /// The HTTPS port to listen on. Defaults to <see cref="NetworkPort.Https"/>.
        /// </summary>
        public int HttpsPort { get; set; } = NetworkPort.Https;

        /// <summary>
        /// The HTTP port to listen on. Defaults to <see cref="NetworkPort.Http"/>.
        /// </summary>
        public int HttpPort { get; set; } = NetworkPort.Http;
    }
}
