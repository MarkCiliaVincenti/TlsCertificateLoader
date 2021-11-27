using System.Net;

namespace TlsCertificateLoader.Models
{
    public class TlsCertificateLoaderWebOptions
    {
        public IPAddress IPAddress { get; set; } = IPAddress.Any;
        public int HttpsPort { get; set; } = 443;
        public int HttpPort { get; set; } = 80;
    }
}
