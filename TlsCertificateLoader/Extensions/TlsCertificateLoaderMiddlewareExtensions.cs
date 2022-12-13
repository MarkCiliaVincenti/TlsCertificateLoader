using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
#if NET6_0
using System.Runtime.Versioning;
#endif
using TlsCertificateLoader.Models;
using TlsCertificateLoader.Workers;

namespace TlsCertificateLoader.Extensions
{
    /// <summary>
    /// Extension class for TlsCertificateLoader middleware for Certbot.
    /// </summary>
#if NET6_0
    [RequiresPreviewFeatures]
#endif
    public static class TlsCertificateLoaderMiddlewareExtensions
    {
        /// <summary>
        /// Use TlsCertificateLoader middleware for Certbot.
        /// </summary>
        /// <param name="webBuilder">The extended <see cref="IWebHostBuilder"/> instance.</param>
        /// <param name="configure">A callback to configure <see cref="TlsCertificateLoaderWebOptions"/>.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseTlsCertificateLoader(this IWebHostBuilder webBuilder, Action<TlsCertificateLoaderWebOptions> configure)
        {
            TlsCertificateLoaderWebOptions tlsCertificateLoaderWebOptions = new();
            configure(tlsCertificateLoaderWebOptions);

            webBuilder.UseKestrel(k =>
            {
                var appServices = k.ApplicationServices;
                var worker = appServices.GetService<ICertbotCertificateRefresher>();

                k.Listen(tlsCertificateLoaderWebOptions.IPAddress, tlsCertificateLoaderWebOptions.HttpsPort, o =>
                {
                    o.SetTlsHandshakeCallbackOptions(worker.TlsCertificateLoader);
                    o.SetHttpsConnectionAdapterOptions(worker.TlsCertificateLoader);

                    o.Protocols = tlsCertificateLoaderWebOptions.HttpProtocols;
                });
                if (tlsCertificateLoaderWebOptions.ListenOnUnencryptedHttp)
                {
                    k.Listen(tlsCertificateLoaderWebOptions.IPAddress, tlsCertificateLoaderWebOptions.HttpPort, o =>
                    {
                        o.Protocols = HttpProtocols.Http1;
                    });
                }
            });

            return webBuilder;
        }

        /// <summary>
        /// Use TlsCertificateLoader middleware for Certbot with default options.
        /// </summary>
        /// <param name="webBuilder">The extended <see cref="IWebHostBuilder"/> instance.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder UseTlsCertificateLoader(this IWebHostBuilder webBuilder)
        {
            return UseTlsCertificateLoader(webBuilder, o => { });
        }

        /// <summary>
        /// Adds TlsCertificateLoader middleware for Certbot.
        /// </summary>
        /// <param name="services">The extended <see cref="IServiceCollection"/> instance.</param>
        /// <param name="defaultHostname">The default hostname.</param>
        /// <param name="wwwHostname">The www subdomain to redirect to the default hostname.</param>
        /// <param name="certbotCertificatesPath">The physical path to the Certbot certificates root folder (e.g. /etc/letsencrypt on Linux).</param>
        /// <param name="configure">A callback to configure <see cref="TlsCertificateLoaderHstsOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddTlsCertificateLoader(this IServiceCollection services, string defaultHostname, string wwwHostname, string certbotCertificatesPath, Action<TlsCertificateLoaderHstsOptions> configure)
        {
            TlsCertificateLoaderHstsOptions tlsCertificateLoaderHstsOptions = new();
            configure(tlsCertificateLoaderHstsOptions);

            if (tlsCertificateLoaderHstsOptions.HstsEnabled)
            {
                services.AddHsts(o => o = tlsCertificateLoaderHstsOptions);
            }

            services.AddSingleton<ICertbotCertificateRefresher>(new CertbotCertificateRefresher(defaultHostname, wwwHostname, certbotCertificatesPath));

            return services;
        }

        /// <summary>
        /// Adds TlsCertificateLoader middleware for Certbot with default options.
        /// </summary>
        /// <param name="services">The extended <see cref="IServiceCollection"/> instance.</param>
        /// <param name="defaultHostname">The default hostname.</param>
        /// <param name="wwwHostname">The www subdomain to redirect to the default hostname.</param>
        /// <param name="certbotCertificatesPath">The physical path to the Certbot certificates root folder (e.g. /etc/letsencrypt on Linux).</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddTlsCertificateLoader(this IServiceCollection services, string defaultHostname, string wwwHostname, string certbotCertificatesPath)
        {
            return AddTlsCertificateLoader(services, defaultHostname, wwwHostname, certbotCertificatesPath, o => { });
        }

        /// <summary>
        /// Configures TlsCertificateLoader middleware for Certbot.
        /// </summary>
        /// <param name="app">The extended <see cref="IApplicationBuilder"/> instance.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseTlsCertificateLoader(this IApplicationBuilder app)
        {
            return UseTlsCertificateLoader(app, o => { });
        }

        /// <summary>
        /// Configures TlsCertificateLoader middleware for Certbot.
        /// </summary>
        /// <param name="app">The extended <see cref="IApplicationBuilder"/> instance.</param>
        /// <param name="configure">A callback to configure <see cref="TlsCertificateLoaderAppOptions"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseTlsCertificateLoader(this IApplicationBuilder app, Action<TlsCertificateLoaderAppOptions> configure)
        {
            TlsCertificateLoaderAppOptions tlsCertificateLoaderAppOptions = new();
            configure(tlsCertificateLoaderAppOptions);

            app.UseHsts();

            if (tlsCertificateLoaderAppOptions.UseDefaultFiles)
            {
                if (tlsCertificateLoaderAppOptions.DefaultFilesOptions == default)
                {
                    app.UseDefaultFiles();
                }
                else
                {
                    app.UseDefaultFiles(tlsCertificateLoaderAppOptions.DefaultFilesOptions);
                }
            }
            if (tlsCertificateLoaderAppOptions.UseStaticFiles)
            {
                if (tlsCertificateLoaderAppOptions.StaticFileOptions == default)
                {
                    app.UseStaticFiles();
                }
                else
                {
                    app.UseStaticFiles(tlsCertificateLoaderAppOptions.StaticFileOptions);
                }
            }

            if (!tlsCertificateLoaderAppOptions.RedirectWwwSubdomainToDomain && !tlsCertificateLoaderAppOptions.RedirectHttpToHttps)
            {
                app.Use(async (context, next) =>
                {
                    var uri = context.Request.Path.Value ?? string.Empty;
                    if (uri.StartsWith("/.well-known"))
                    {
                        await next();
                        return;
                    }
                });
            }
            else if (!tlsCertificateLoaderAppOptions.RedirectWwwSubdomainToDomain && tlsCertificateLoaderAppOptions.RedirectHttpToHttps)
            {
                app.Use(async (context, next) =>
                {
                    var uri = context.Request.Path.Value ?? string.Empty;
                    if (uri.StartsWith("/.well-known"))
                    {
                        await next();
                        return;
                    }
                    if (!context.Request.IsHttps)
                    {
                        var withHttps = $"https://{context.Request.Host}{uri}";
                        context.Response.Redirect(withHttps, permanent: true);
                    }
                });
            }
            else if (tlsCertificateLoaderAppOptions.RedirectWwwSubdomainToDomain && !tlsCertificateLoaderAppOptions.RedirectHttpToHttps)
            {
                app.Use(async (context, next) =>
                {
                    if (context.Request.IsHttps)
                    {
                        if (context.Request.Host.Host.StartsWith("www."))
                        {
                            var uriWww = context.Request.Path.Value ?? string.Empty;
                            var finalUri = $"https://{context.Request.Host.ToString()[4..]}{uriWww}";
                            context.Response.Redirect(finalUri, permanent: true);
                            return;
                        }
                        await next();
                        return;
                    }
                    var uri = context.Request.Path.Value ?? string.Empty;
                    if (uri.StartsWith("/.well-known"))
                    {
                        await next();
                        return;
                    }
                });
            }
            else
            {
                app.Use(async (context, next) =>
                {
                    if (context.Request.IsHttps)
                    {
                        if (context.Request.Host.Host.StartsWith("www."))
                        {
                            var uriWww = context.Request.Path.Value ?? string.Empty;
                            var finalUri = $"https://{context.Request.Host.ToString()[4..]}{uriWww}";
                            context.Response.Redirect(finalUri, permanent: true);
                            return;
                        }
                        await next();
                        return;
                    }
                    var uri = context.Request.Path.Value ?? string.Empty;
                    if (uri.StartsWith("/.well-known"))
                    {
                        await next();
                        return;
                    }
                    var withHttps = $"https://{context.Request.Host}{uri}";
                    context.Response.Redirect(withHttps, permanent: true);
                });
            }

            return app;
        }
    }
}
