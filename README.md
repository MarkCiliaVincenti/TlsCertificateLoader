# TlsCertificateLoader
Allows loading of TLS (HTTPS) certificates for .NET 6.0 Kestrel web applications, allowing for refreshing of certificates as well as compatibility with HTTP/3. Fully compatible with certificates obtained by [Certbot](https://certbot.eff.org/) ([see sample project](https://github.com/MarkCiliaVincenti/TlsCertificateLoader/tree/master/Samples/CertbotSample)).

To use:
```c#
TlsCertificateLoader.TlsCertificateLoader tlsCertificateLoader = new(fullChainPemFilePath, privateKeyPemFilePath);
options.ListenAnyIp(433, o =>
{
     o.SetTlsHandshakeCallbackOptions(tlsCertificateLoader);
     o.SetHttpsConnectionAdapterOptions(tlsCertificateLoader);
     o.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
});
```

And to refresh (eg either on a Timer or watching a directory via PhysicalFileProvider):
```c#
tlsCertificateLoader.RefreshDefaultCertificates();
```

You may also add additional certificate collection for other hostnames (for example if you want to set up mydomain.tld as your default certificate and www.mydomain.tld as your alternate one):
```c#
tlsCertificateLoader.AddAdditionalCertificates("www.mydomain.tld", fullChainWwwPemFilePath, privateKeyWwwPemFilePath);
```

And to refresh additional certificate collections (eg either on a Timer or watching a directory via PhysicalFileProvider):
```c#
tlsCertificateLoader.RefreshAdditionalCertificates("www.mydomain.tld");
```

A [sample project using Certbot](https://github.com/MarkCiliaVincenti/TlsCertificateLoader/tree/master/Samples/CertbotSample) is available.

## Credits
[David Fowler](https://github.com/davidfowl) for [this idea](https://github.com/dotnet/aspnetcore/issues/21513#issuecomment-914370034).

Logo made by [Freepik](https://www.freepik.com) from [www.flaticon.com](https://www.flaticon.com/).