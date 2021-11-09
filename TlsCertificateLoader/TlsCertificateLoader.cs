using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TlsCertificateLoader.Extensions;

namespace TlsCertificateLoader
{
    public sealed class TlsCertificateLoader
    {
        public X509Certificate2Collection X509Certificate2Collection { get; private set; }
        internal SslStreamCertificateContext SslStreamCertificateContext { get; set; }
        private string _fullChainPemFilePath;
        private string _privateKeyPemFilePath;

        public TlsCertificateLoader(string fullChainPemFilePath, string privateKeyPemFilePath)
        {
            RefreshCertificates(fullChainPemFilePath, privateKeyPemFilePath);
        }

        public void RefreshCertificates(string fullChainPemFilePath, string privateKeyPemFilePath)
        {
            _fullChainPemFilePath = fullChainPemFilePath;
            _privateKeyPemFilePath = privateKeyPemFilePath;
            RefreshCertificates();
        }

        public void RefreshCertificates()
        {
            X509Certificate2Collection = X509Certificate2Collection.ImportFullChainFromPemFiles(_fullChainPemFilePath, _privateKeyPemFilePath);
            SslStreamCertificateContext = SslStreamCertificateContext.Create(X509Certificate2Collection[0], X509Certificate2Collection, offline: true);
        }
    }
}
