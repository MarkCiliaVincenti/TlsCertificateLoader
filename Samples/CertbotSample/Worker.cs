using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CertbotSample
{
    public interface IWorker
    {
        public TlsCertificateLoader.TlsCertificateLoader TlsCertificateLoader { get; }
    }

    public class Worker : IWorker
    {
        private const string defaultHostname = "mydomain.tld";
        private const string wwwHostname = "www.mydomain.tld";
        private const string defaultCertbotPath = "/etc/letsencrypt";

        private readonly PhysicalFileProvider _fileProvider;
        private readonly PhysicalFileProvider _fileProviderWww;
        private IChangeToken _fileChangeToken;
        private IChangeToken _fileChangeTokenWww;
        public TlsCertificateLoader.TlsCertificateLoader TlsCertificateLoader { get; private set; }

        private void ChangeToken(PhysicalFileProvider physicalFileProvider, bool isWww)
        {
            var fileChangeToken = (isWww) ? _fileChangeTokenWww : _fileChangeToken;
            fileChangeToken = physicalFileProvider.Watch("privkey*.pem");
            fileChangeToken.RegisterChangeCallback(CertificatesRefreshed, isWww);
        }

        private async void CertificatesRefreshed(object state)
        {
            var isWww = Convert.ToBoolean(state);
            await Task.Delay(5000);
            if (isWww)
            {
                ChangeToken(_fileProviderWww, true);
                TlsCertificateLoader.RefreshAdditionalCertificates(wwwHostname);
            }
            else
            {
                ChangeToken(_fileProvider, false);
                TlsCertificateLoader.RefreshDefaultCertificates();
            }
        }

        public Worker()
        {
            _fileProvider = new(Path.Combine(defaultCertbotPath, "archive", defaultHostname));
            _fileProviderWww = new(Path.Combine(defaultCertbotPath, "archive", wwwHostname));
            var defaultPath = Path.Combine(defaultCertbotPath, "live", defaultHostname);
            var wwwPath = Path.Combine(defaultCertbotPath, "live", wwwHostname);
            TlsCertificateLoader = new(Path.Combine(defaultPath, "fullchain.pem"), Path.Combine(defaultPath, "privkey.pem"));
            TlsCertificateLoader.AddAdditionalCertificates(wwwHostname, Path.Combine(wwwPath, "fullchain.pem"), Path.Combine(wwwPath, "privkey.pem"));
            ChangeToken(_fileProvider, false);
            ChangeToken(_fileProviderWww, true);
        }
    }
}