using System;
using System.Security.Cryptography.X509Certificates;

namespace TlsCertificateLoader.Extensions
{
    public static class X509Certificate2CollectionExtensions
    {
        public static X509Certificate2Collection ImportFullChainFromPemFiles(this X509Certificate2Collection x509Certificate2Collection, string fullChainPemFilePath, string privateKeyPemFilePath)
        {
            ArgumentNullException.ThrowIfNull(fullChainPemFilePath);
            ArgumentNullException.ThrowIfNull(privateKeyPemFilePath);

            var leaf = X509Certificate2.CreateFromPemFile(fullChainPemFilePath, privateKeyPemFilePath);
            var chain = new X509Certificate2Collection(leaf);
            chain.ImportFromPemFile(fullChainPemFilePath);
            return chain;
        }
    }
}
