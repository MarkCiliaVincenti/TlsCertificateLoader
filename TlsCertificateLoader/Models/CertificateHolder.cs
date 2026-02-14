using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TlsCertificateLoader.Extensions;

namespace TlsCertificateLoader.Models;

internal sealed class CertificateHolder
{
    internal X509Certificate2Collection X509Certificate2Collection { get; private set; }
    internal X509Certificate2 X509Certificate2 { get; private set; }
    internal SslStreamCertificateContext SslStreamCertificateContext { get; private set; }
    internal string FullChainPemFilePath { get; set; }
    internal string PrivateKeyPemFilePath { get; set; }
    internal string? Password { get; set; }

    internal CertificateHolder(string fullChainPemFilePath, string privateKeyPemFilePath, string? password)
    {
        ArgumentNullException.ThrowIfNull(fullChainPemFilePath);
        ArgumentNullException.ThrowIfNull(privateKeyPemFilePath);

        FullChainPemFilePath = fullChainPemFilePath;
        PrivateKeyPemFilePath = privateKeyPemFilePath;
        Password = password;

        X509Certificate2Collection = [];
        RefreshCertificates();
    }

    internal void RefreshCertificates()
    {
        X509Certificate2Collection = X509Certificate2Collection.ImportFullChainFromPemFiles(FullChainPemFilePath, PrivateKeyPemFilePath, Password);
        X509Certificate2 = X509Certificate2Collection[0];
        SslStreamCertificateContext = SslStreamCertificateContext.Create(X509Certificate2, X509Certificate2Collection, offline: true);
    }
}
