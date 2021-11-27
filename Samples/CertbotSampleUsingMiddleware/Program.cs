using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Net;
using TlsCertificateLoader.Extensions;

var host = Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.ConfigureServices(services => services.AddTlsCertificateLoader("mydomain.tld", "www.mydomain.tld", "/etc/letsencrypt"));
    webBuilder.Configure(configureApp => configureApp.UseTlsCertificateLoader());
    webBuilder.UseTlsCertificateLoader(o => o.IPAddress = new IPAddress(0));
}).Build();

await host.RunAsync();