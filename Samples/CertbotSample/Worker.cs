using Microsoft.Extensions.FileProviders;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TlsCertificateLoader.Exceptions;

namespace CertbotSample;

public interface IWorker
{
    public TlsCertificateLoader.TlsCertificateLoader TlsCertificateLoader { get; }
}

public class Worker : IWorker
{
    private const string defaultHostname = "mydomain.tld";
    private const string wwwHostname = "www.mydomain.tld";
    private const string certbotCertificatesPath = "/etc/letsencrypt";

    private readonly PhysicalFileProvider _fileProvider;
    private readonly PhysicalFileProvider _fileProviderWww;
    public TlsCertificateLoader.TlsCertificateLoader TlsCertificateLoader { get; private set; }

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
            try
            {
                TlsCertificateLoader.RefreshAdditionalCertificates(wwwHostname);
                Debug.WriteLine($"Additional certificates for {wwwHostname} refresh successful");
            }
            catch (AdditionalCertificatesNotInitializedException)
            {
                Debug.WriteLine($"Additional certificates for {wwwHostname} refresh failed because initialization did not happen");
            }
            catch (AdditionalCertificatesNotFoundException ex)
            {
                Debug.WriteLine($"Additional certificates for {ex.HostName} refresh failed because the certificate was not added");
            }
        }
        else
        {
            ChangeToken(_fileProvider, false);
            try
            {
                TlsCertificateLoader.RefreshDefaultCertificates();
                Debug.WriteLine($"Default certificates refresh successful");
            }
            catch (DefaultCertificatesNotInitializedException)
            {
                Debug.WriteLine($"Default certificates refresh failed because initialization did not happen");
            }
        }
    }

    public Worker()
    {
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