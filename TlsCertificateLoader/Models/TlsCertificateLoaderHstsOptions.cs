using Microsoft.AspNetCore.HttpsPolicy;
using System;

namespace TlsCertificateLoader.Models
{
    /// <summary>
    /// TlsCertificateLoader middleware for Certbot HSTS options
    /// </summary>
    public class TlsCertificateLoaderHstsOptions : HstsOptions
    {
        /// <summary>
        /// Enable or disable HSTS (Strict-Transport-Security header). Defaults to <see langword="true"/>.
        /// </summary>
        public bool HstsEnabled { get; set; } = true;
        /// <summary>
        /// Sets the max-age parameter of the Strict-Transport-Security header. Defaults to 2 years.
        /// </summary>
        public new TimeSpan MaxAge { get; set; } = TimeSpan.FromDays(365 * 2);
        /// <summary>
        /// <inheritdoc cref="HstsOptions.IncludeSubDomains"/>
        /// </summary>
        public new bool IncludeSubDomains { get; set; }
        /// <summary>
        /// <inheritdoc cref="HstsOptions.Preload"/> Defaults to <see langword="true"/>.
        /// </summary>
        public new bool Preload { get; set; } = true;
    }
}

