using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TlsCertificateLoader.Models;

namespace TlsCertificateLoader.Workers;

internal interface ICertbotCertificateRefresher
{
    public TlsCertificateLoader TlsCertificateLoader { get; }
}

internal sealed class CertbotCertificateRefresher : ICertbotCertificateRefresher, IDisposable
{
    private readonly Dictionary<string, PhysicalFileProvider> _additionalPhysicalFileProviders;
    private readonly PhysicalFileProvider _defaultPhysicalFileProvider;
    private bool _disposedValue;

    public TlsCertificateLoader TlsCertificateLoader { get; private set; }

    private void ChangeToken(PhysicalFileProvider physicalFileProvider, string? hostname)
    {
        var fileChangeToken = physicalFileProvider.Watch("privkey*.pem");
        fileChangeToken.RegisterChangeCallback(async (hostname) => await CertificatesRefreshed(hostname).ConfigureAwait(false), hostname);
    }

    private async Task CertificatesRefreshed(object? hostname)
    {
        if (hostname == null)
        {
            ChangeToken(_defaultPhysicalFileProvider, null);
            TlsCertificateLoader.RefreshDefaultCertificates();
        }
        else
        {
            var hn = hostname.ToString()!;
            ChangeToken(_additionalPhysicalFileProviders[hn], hn);
            TlsCertificateLoader.RefreshAdditionalCertificates(hn);
        }
    }

    public CertbotCertificateRefresher(Certificate defaultCertificate, IEnumerable<Certificate>? additionalCertificates = null)
    {
        _defaultPhysicalFileProvider = new PhysicalFileProvider(Path.GetDirectoryName(defaultCertificate.PrivateKeyPemFilePath.Replace($"{Path.DirectorySeparatorChar}live{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}archive{Path.DirectorySeparatorChar}", System.StringComparison.Ordinal))!);
        TlsCertificateLoader = new(defaultCertificate.FullChainPemFilePath, defaultCertificate.PrivateKeyPemFilePath, defaultCertificate.Password);
        ChangeToken(_defaultPhysicalFileProvider, null);

        if (additionalCertificates != null)
        {
            _additionalPhysicalFileProviders = [];
            foreach (var cert in additionalCertificates)
            {
                var additionalFileProvider = new PhysicalFileProvider(Path.GetDirectoryName(cert.PrivateKeyPemFilePath.Replace($"{Path.DirectorySeparatorChar}live{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}archive{Path.DirectorySeparatorChar}", System.StringComparison.Ordinal))!);
                _additionalPhysicalFileProviders.Add(cert.Hostname, additionalFileProvider);
                TlsCertificateLoader.AddAdditionalCertificates(cert.Hostname, cert.FullChainPemFilePath, cert.PrivateKeyPemFilePath, cert.Password);
                ChangeToken(additionalFileProvider, cert.Hostname);
            }
        }
    }

    public CertbotCertificateRefresher(string defaultHostname, string wwwHostname, string certbotCertificatesPath)
        : this(new Certificate(defaultHostname, certbotCertificatesPath),
               [new Certificate(wwwHostname, certbotCertificatesPath)])
    {
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                foreach (var item in _additionalPhysicalFileProviders.Values)
                {
                    item.Dispose();
                }
                _additionalPhysicalFileProviders.Clear();
                _defaultPhysicalFileProvider.Dispose();
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
