﻿using System.Security.Cryptography.X509Certificates;

namespace TlsCertificateLoader.Extensions
{
    /// <summary>
    /// Extension method for <see cref="X509Certificate2Collection"/>.
    /// </summary>
    public static class X509Certificate2CollectionExtensions
    {
        /// <summary>
        /// Imports the full chain certificate collection from .pem files.
        /// </summary>
        /// <param name="x509Certificate2Collection">The extended <see cref="X509Certificate2Collection"/> instance.</param>
        /// <param name="fullChainPemFilePath">The full path to the full chain .pem file (e.g. the fullchain.pem generated by Certbot)</param>
        /// <param name="privateKeyPemFilePath">The full path to the private key .pem file (e.g. the privkey.pem generated by Certbot)</param>
        /// <returns>The <see cref="X509Certificate2Collection"/>.</returns>
        public static X509Certificate2Collection ImportFullChainFromPemFiles(this X509Certificate2Collection x509Certificate2Collection, string fullChainPemFilePath, string privateKeyPemFilePath)
        {
            var leaf = X509Certificate2.CreateFromPemFile(fullChainPemFilePath, privateKeyPemFilePath);
            var chain = new X509Certificate2Collection(leaf);
            chain.ImportFromPemFile(fullChainPemFilePath);
            return chain;
        }
    }
}
