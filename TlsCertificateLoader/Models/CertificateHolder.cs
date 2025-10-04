using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TlsCertificateLoader.Extensions;

namespace TlsCertificateLoader.Models;

internal sealed class CertificateHolder
{
    internal X509Certificate2Collection X509Certificate2Collection { get; private set; }
    internal X509Certificate2 X509Certificate2 { get; private set; }
    internal SslStreamCertificateContext SslStreamCertificateContext { get; set; }
    internal string FullChainPemFilePath { get; set; }
    internal string PrivateKeyPemFilePath { get; set; }

    internal CertificateHolder(string fullChainPemFilePath, string privateKeyPemFilePath)
    {
        ArgumentNullException.ThrowIfNull(fullChainPemFilePath);
        ArgumentNullException.ThrowIfNull(privateKeyPemFilePath);

        FullChainPemFilePath = fullChainPemFilePath;
        PrivateKeyPemFilePath = privateKeyPemFilePath;

        RefreshCertificates();
    }

    internal void RefreshCertificates()
    {
        X509Certificate2Collection = X509Certificate2Collection.ImportFullChainFromPemFiles(FullChainPemFilePath, PrivateKeyPemFilePath);
        X509Certificate2 = X509Certificate2Collection[0];
        SslStreamCertificateContext = SslStreamCertificateContext.Create(X509Certificate2, X509Certificate2Collection, offline: true);
    }
}
