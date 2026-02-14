# ![TlsCertificateLoader](https://raw.githubusercontent.com/MarkCiliaVincenti/TlsCertificateLoader/master/logo32.png)&nbsp;TlsCertificateLoader
[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/MarkCiliaVincenti/TlsCertificateLoader/dotnet.yml?branch=master&logo=github&style=flat)](https://actions-badge.atrox.dev/MarkCiliaVincenti/TlsCertificateLoader/goto?ref=master) [![NuGet](https://img.shields.io/nuget/v/TlsCertificateLoader?label=NuGet&logo=nuget&style=flat)](https://www.nuget.org/packages/TlsCertificateLoader) [![NuGet](https://img.shields.io/nuget/dt/TlsCertificateLoader?logo=nuget&style=flat)](https://www.nuget.org/packages/TlsCertificateLoader) [![Codacy Grade](https://img.shields.io/codacy/grade/b40bdcdfd4314c93a0cea2ace5d5251f?style=flat)](https://app.codacy.com/gh/MarkCiliaVincenti/TlsCertificateLoader/dashboard)

A .NET library for loading of TLS/SSL (HTTPS) certificates for .NET 6.0+ Kestrel web applications, allowing for refreshing of certificates as well as compatibility with HTTP/3. Fully compatible with certificates obtained by [Certbot](https://certbot.eff.org/) ([see sample project without middleware](https://github.com/MarkCiliaVincenti/TlsCertificateLoader/tree/master/Samples/CertbotSample) or [see sample project using middleware](https://github.com/MarkCiliaVincenti/TlsCertificateLoader/tree/master/Samples/CertbotSampleUsingMiddleware)).

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

There are overloaded methdods as well that allow you to specify a password for the private key.
```csharp
tlsCertificateLoader.AddAdditionalCertificates("www.mydomain.tld", fullChainWwwPemFilePath, privateKeyWwwPemFilePath, privateKeyPassword);
```

If you want to add more than just one additional certificate you can use a different overload as well.
```csharp
services.AddTlsCertificateLoader(
     new Certificate("mydomain.tld", "/etc/letsencrypt"),
     [
          new Certificate("mydomain.tld", "/etc/letsencrypt"),
          new Certificate("db.mydomain.tld", "/etc/letsencrypt"),
          new Certificate("server.mydomain.tld", "/etc/letsencrypt")
     ]);
```

A [sample project using Certbot](https://github.com/MarkCiliaVincenti/TlsCertificateLoader/tree/master/Samples/CertbotSample) is available.

## Usage with Certbot middleware

Refer to the [sample project using Certbot using middleware](https://github.com/MarkCiliaVincenti/TlsCertificateLoader/tree/master/Samples/CertbotSampleUsingMiddleware).

## Credits
[David Fowler](https://github.com/davidfowl) for [this idea](https://github.com/dotnet/aspnetcore/issues/21513#issuecomment-914370034).
