# <img src="https://github.com/MarkCiliaVincenti/TlsCertificateLoader/raw/master/logo.png" width="32" height="32"> TlsCertificateLoader
[![GitHub branch checks state](https://img.shields.io/github/checks-status/MarkCiliaVincenti/TlsCertificateLoader/master?label=build&logo=github&style=for-the-badge)](https://actions-badge.atrox.dev/MarkCiliaVincenti/TlsCertificateLoader/goto?ref=master) [![Nuget](https://img.shields.io/nuget/v/TlsCertificateLoader?label=TlsCertificateLoader&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/TlsCertificateLoader) [![Nuget](https://img.shields.io/nuget/dt/TlsCertificateLoader?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/TlsCertificateLoader)

Allows loading of TLS (HTTPS) certificates for .NET 6.0 Kestrel web applications, allowing for refreshing of certificates as well as compatibility with HTTP/3. Fully compatible with certificates obtained by [Certbot](https://certbot.eff.org/) ([see sample project without middleware](https://github.com/MarkCiliaVincenti/TlsCertificateLoader/tree/master/Samples/CertbotSample) or [see sample project using middleware](https://github.com/MarkCiliaVincenti/TlsCertificateLoader/tree/master/Samples/CertbotSampleUsingMiddleware)).

## Installation
The recommended means is to use [NuGet](https://www.nuget.org/packages/TlsCertificateLoader), but you could also download the source code from [here](https://github.com/MarkCiliaVincenti/TlsCertificateLoader/releases).

## Usage without middleware
```csharp
TlsCertificateLoader.TlsCertificateLoader tlsCertificateLoader = new(fullChainPemFilePath, privateKeyPemFilePath);
options.ListenAnyIp(433, o =>
{
     o.SetTlsHandshakeCallbackOptions(tlsCertificateLoader);
     o.SetHttpsConnectionAdapterOptions(tlsCertificateLoader);
     o.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
});
```

And to refresh (eg either on a Timer or watching a directory via PhysicalFileProvider):
```csharp
tlsCertificateLoader.RefreshDefaultCertificates();
```

You may also add additional certificate collection for other hostnames (for example if you want to set up mydomain.tld as your default certificate and www.mydomain.tld as your alternate one):
```csharp
tlsCertificateLoader.AddAdditionalCertificates("www.mydomain.tld", fullChainWwwPemFilePath, privateKeyWwwPemFilePath);
```

And to refresh additional certificate collections (eg either on a Timer or watching a directory via PhysicalFileProvider):
```csharp
tlsCertificateLoader.RefreshAdditionalCertificates("www.mydomain.tld");
```

A [sample project using Certbot](https://github.com/MarkCiliaVincenti/TlsCertificateLoader/tree/master/Samples/CertbotSample) is available.

## Usage with Certbot middleware

Refer to the [sample project using Certbot using middleware](https://github.com/MarkCiliaVincenti/TlsCertificateLoader/tree/master/Samples/CertbotSampleUsingMiddleware).

## Credits
[David Fowler](https://github.com/davidfowl) for [this idea](https://github.com/dotnet/aspnetcore/issues/21513#issuecomment-914370034).
