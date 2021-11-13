using CertbotSample;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using TlsCertificateLoader.Extensions;

var host = Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.UseStartup<Startup>();
    webBuilder.PreferHostingUrls(false);
    webBuilder.UseKestrel(k =>
    {
        var appServices = k.ApplicationServices;
        var worker = appServices.GetService<IWorker>();

        k.Listen(new IPAddress(0), 443, o =>
        {
            o.SetTlsHandshakeCallbackOptions(worker.TlsCertificateLoader);
            o.SetHttpsConnectionAdapterOptions(worker.TlsCertificateLoader);

            o.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        });
        k.Listen(new IPAddress(0), 80, o =>
        {
            o.Protocols = HttpProtocols.Http1;
        });
    });
}).Build();

await host.RunAsync();