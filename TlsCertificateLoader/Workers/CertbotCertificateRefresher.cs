using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TlsCertificateLoader.Workers;

internal interface ICertbotCertificateRefresher
{
    public TlsCertificateLoader TlsCertificateLoader { get; }
}

internal sealed class CertbotCertificateRefresher : ICertbotCertificateRefresher
{
    private readonly string _wwwHostname;

    private readonly PhysicalFileProvider _fileProvider;
    private readonly PhysicalFileProvider _fileProviderWww;
    public TlsCertificateLoader TlsCertificateLoader { get; private set; }

    private void ChangeToken(PhysicalFileProvider physicalFileProvider, bool isWww)
    {
        var fileChangeToken = physicalFileProvider.Watch("privkey*.pem");
        fileChangeToken.RegisterChangeCallback(async (state) => await CertificatesRefreshed(state).ConfigureAwait(false), isWww);
    }

    private async Task CertificatesRefreshed(object state)
    {
        var isWww = Convert.ToBoolean(state);
        await Task.Delay(5000);
        if (isWww)
        {
            ChangeToken(_fileProviderWww, true);
            TlsCertificateLoader.RefreshAdditionalCertificates(_wwwHostname);
        }
        else
        {
            ChangeToken(_fileProvider, false);
            TlsCertificateLoader.RefreshDefaultCertificates();
        }
    }

    public CertbotCertificateRefresher(string defaultHostname, string wwwHostname, string certbotCertificatesPath)
    {
        _wwwHostname = wwwHostname;

        _fileProvider = new(Path.Combine(certbotCertificatesPath, "archive", defaultHostname));
        _fileProviderWww = new(Path.Combine(certbotCertificatesPath, "archive", wwwHostname));
        var defaultPath = Path.Combine(certbotCertificatesPath, "live", defaultHostname);
        var wwwPath = Path.Combine(certbotCertificatesPath, "live", wwwHostname);
        TlsCertificateLoader = new(Path.Combine(defaultPath, "fullchain.pem"), Path.Combine(defaultPath, "privkey.pem"));
        TlsCertificateLoader.AddAdditionalCertificates(wwwHostname, Path.Combine(wwwPath, "fullchain.pem"), Path.Combine(wwwPath, "privkey.pem"));
        ChangeToken(_fileProvider, false);
        ChangeToken(_fileProviderWww, true);
    }
}