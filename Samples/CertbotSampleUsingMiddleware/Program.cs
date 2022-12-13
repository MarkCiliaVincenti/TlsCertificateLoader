using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using System.Net;
using TlsCertificateLoader.Extensions;

var host = Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.ConfigureServices(services => services.AddTlsCertificateLoader("mydomain.tld", "www.mydomain.tld", "/etc/letsencrypt"));
    webBuilder.Configure(configureApp => configureApp.UseTlsCertificateLoader(o =>
    {
        o.UseDefaultFiles = true;
        o.UseStaticFiles = true;
        o.StaticFileOptions = new StaticFileOptions
        {
            ServeUnknownFileTypes = true,
            OnPrepareResponse = context =>
            {
                var headers = context.Context.Response.GetTypedHeaders();

                headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(30)
                };
            }
        };
        o.RedirectHttpToHttps = true;
        o.RedirectWwwSubdomainToDomain = true;
    }));
    webBuilder.UseTlsCertificateLoader(o => o.IPAddress = new IPAddress(0));
}).Build();

await host.RunAsync();