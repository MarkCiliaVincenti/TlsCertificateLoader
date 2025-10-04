using Microsoft.AspNetCore.Server.Kestrel.Core;
using NetworkPorts;
using System.Net;
#if NET6_0
using System.Runtime.Versioning;
#endif

namespace TlsCertificateLoader.Models;

/// <summary>
/// TlsCertificateLoader middleware for Certbot web options
/// </summary>
#if NET6_0
[RequiresPreviewFeatures]
#endif
public sealed class TlsCertificateLoaderWebOptions
{
    /// <summary>
    /// The <see cref="System.Net.IPAddress"/> to listen on. Defaults to <see cref="IPAddress.Any"/>.
    /// </summary>
    public IPAddress IPAddress { get; set; } = IPAddress.Any;

    /// <summary>
    /// The HTTPS port to listen on. Defaults to <see cref="NetworkPort.Https"/>.
    /// </summary>
    public int HttpsPort { get; set; } = NetworkPort.Https;

    /// <summary>
    /// The HTTP port to listen on. Defaults to <see cref="NetworkPort.Http"/>.
    /// </summary>
    public int HttpPort { get; set; } = NetworkPort.Http;

    /// <summary>
    /// HTTP protocol versions. Defaults to <see cref="HttpProtocols.Http1AndHttp2AndHttp3"/>.
    /// </summary>
    public HttpProtocols HttpProtocols { get; set; } = HttpProtocols.Http1AndHttp2AndHttp3;

    /// <summary>
    /// Listen on the HTTP port. Defaults to true.
    /// </summary>
    public bool ListenOnUnencryptedHttp { get; set; } = true;
}
