# TlsCertificateLoader
Allows loading of TLS (HTTPS) certificates for .NET 6.0 Kestrel web applications, allowing for refreshing of certificates as well as compatibility with HTTP/3.

To use:
```c#
options.ListenAnyIp(433, o =>
{
     o.SetTlsHandshakeCallbackOptions(fullChainPath, privateKeyPath);
     o.SetHttpsConnectionAdapterOptions(fullChainPath, privateKeyPath);
     o.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
});
```

And to refresh (eg either on a Timer or watching a directory via PhysicalFileProvider):
```c#
TlsCertificateLoader.TlsCertificateLoader.RefreshX509Certificate2Collection();
```

## Credits
[David Fowler](https://github.com/davidfowl) for [this idea](https://github.com/dotnet/aspnetcore/issues/21513#issuecomment-914370034).

Logo made by [Freepik](https://www.freepik.com) from [www.flaticon.com](https://www.flaticon.com/).