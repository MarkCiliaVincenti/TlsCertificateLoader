using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace TlsCertificateLoader
{
    public static class TlsCertificateLoader
    {
        private class SslCertificateCollection
        {
            public string FullChainPath { get; set; }
            public string PrivateKeyPath { get; set; }

            private X509Certificate2Collection _collection;

            internal X509Certificate2Collection X509Certificate2Collection
            {
                get { return _collection; }
                private set { _collection = value; }
            }

            internal X509Certificate2Collection GetX509Certificate2Collection()
            {
                var leaf = X509Certificate2.CreateFromPemFile(FullChainPath, PrivateKeyPath);
                var chain = new X509Certificate2Collection(leaf);
                chain.ImportFromPemFile(FullChainPath);
                return chain;
            }

            internal void RefreshX509Certificate2Collection()
            {
                X509Certificate2Collection = GetX509Certificate2Collection();
            }
        }

        private static SslCertificateCollection _sslCertificate;

        public static void RefreshX509Certificate2Collection()
        {
            _sslCertificate.RefreshX509Certificate2Collection();
        }

        public static X509Certificate2Collection X509Certificate2Collection()
        {
            return _sslCertificate.X509Certificate2Collection;
        }

        private static SslCertificateCollection Initialize(string fullChainPath, string privateKeyPath)
        {
            ArgumentNullException.ThrowIfNull(fullChainPath);
            ArgumentNullException.ThrowIfNull(privateKeyPath);

            if (_sslCertificate == null)
            {
                _sslCertificate = new SslCertificateCollection()
                {
                    FullChainPath = fullChainPath,
                    PrivateKeyPath = privateKeyPath
                };
                _sslCertificate.RefreshX509Certificate2Collection();
            }
            return _sslCertificate;
        }

        public static ListenOptions SetTlsHandshakeCallbackOptions(this ListenOptions listenOptions, string fullChainPath, string privateKeyPath)
        {
            var x509Certificate2Collection = Initialize(fullChainPath, privateKeyPath).X509Certificate2Collection;
            return listenOptions.UseHttps(new TlsHandshakeCallbackOptions
            {
                OnConnection = context => new(new SslServerAuthenticationOptions
                {
                    ServerCertificateContext = SslStreamCertificateContext.Create(x509Certificate2Collection[0], x509Certificate2Collection, offline: true)
                })
            });
        }

        public static ListenOptions SetHttpsConnectionAdapterOptions(this ListenOptions listenOptions, string fullChainPath, string privateKeyPath)
        {
            var x509Certificate2Collection = Initialize(fullChainPath, privateKeyPath).X509Certificate2Collection;

            return listenOptions.UseHttps(new HttpsConnectionAdapterOptions
            {
                ServerCertificateSelector = (context, dnsName) => new(x509Certificate2Collection[0])
            });
        }
    }
}