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

public class Worker : IWorker, IDisposable
{
    private const string DefaultHostname = "mydomain.tld";
    private const string WwwHostname = "www.mydomain.tld";
    private const string CertbotCertificatesPath = "/etc/letsencrypt";

    private readonly PhysicalFileProvider _fileProvider;
    private readonly PhysicalFileProvider _fileProviderWww;
    private bool _disposedValue;

    public TlsCertificateLoader.TlsCertificateLoader TlsCertificateLoader { get; private set; }

    private void ChangeToken(PhysicalFileProvider physicalFileProvider, bool isWww)
    {
        var fileChangeToken = physicalFileProvider.Watch("privkey*.pem");
        fileChangeToken.RegisterChangeCallback(async (state) => await CertificatesRefreshed(state!).ConfigureAwait(false), isWww);
    }

    private async Task CertificatesRefreshed(object state)
    {
        var isWww = state is bool flag && flag;
        await Task.Delay(5000);
        if (isWww)
        {
            ChangeToken(_fileProviderWww, true);
            try
            {
                TlsCertificateLoader.RefreshAdditionalCertificates(WwwHostname);
                Debug.WriteLine($"Additional certificates for {WwwHostname} refresh successful");
            }
            catch (AdditionalCertificatesNotInitializedException)
            {
                Debug.WriteLine($"Additional certificates for {WwwHostname} refresh failed because initialization did not happen");
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
        _fileProvider = new(Path.Combine(CertbotCertificatesPath, "archive", DefaultHostname));
        _fileProviderWww = new(Path.Combine(CertbotCertificatesPath, "archive", WwwHostname));
        var defaultPath = Path.Combine(CertbotCertificatesPath, "live", DefaultHostname);
        var wwwPath = Path.Combine(CertbotCertificatesPath, "live", WwwHostname);
        TlsCertificateLoader = new(Path.Combine(defaultPath, "fullchain.pem"), Path.Combine(defaultPath, "privkey.pem"));
        TlsCertificateLoader.AddAdditionalCertificates(WwwHostname, Path.Combine(wwwPath, "fullchain.pem"), Path.Combine(wwwPath, "privkey.pem"));
        ChangeToken(_fileProvider, false);
        ChangeToken(_fileProviderWww, true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _fileProvider.Dispose();
                _fileProviderWww.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
