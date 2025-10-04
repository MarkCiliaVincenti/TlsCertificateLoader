using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;

namespace TlsCertificateLoader.Models;

/// <summary>
/// TlsCertificateLoader middleware for Certbot app options
/// </summary>
public sealed class TlsCertificateLoaderAppOptions
{
    /// <summary>
    /// <inheritdoc cref="DefaultFilesExtensions.UseDefaultFiles(IApplicationBuilder, DefaultFilesOptions)"/>
    /// </summary>
    public bool UseDefaultFiles { get; set; } = true;

    /// <summary>
    /// <inheritdoc cref="Microsoft.AspNetCore.Builder.DefaultFilesOptions" />
    /// </summary>
    public DefaultFilesOptions DefaultFilesOptions { get; set; }

    /// <summary>
    /// <inheritdoc cref="StaticFileExtensions.UseStaticFiles(IApplicationBuilder, StaticFileOptions)"/>
    /// </summary>
    public bool UseStaticFiles { get; set; } = true;

    /// <summary>
    /// <inheritdoc cref="Microsoft.AspNetCore.Builder.StaticFileOptions" />
    /// </summary>
    public StaticFileOptions StaticFileOptions { get; set; } = new StaticFileOptions
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

    /// <summary>
    /// Redirects www.mydomain.tld to mydomain.tld
    /// </summary>
    public bool RedirectWwwSubdomainToDomain { get; set; } = true;

    /// <summary>
    /// Redirects unencrypted HTTP traffic to HTTPS
    /// </summary>
    public bool RedirectHttpToHttps { get; set; } = true;
}
