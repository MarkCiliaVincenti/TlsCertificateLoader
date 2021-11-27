using Microsoft.AspNetCore.HttpsPolicy;
using System;

namespace TlsCertificateLoader.Models
{
    public class TlsCertificateLoaderHstsOptions : HstsOptions
    {
        public bool HstsEnabled { get; set; } = true;
        public new TimeSpan MaxAge { get; set; } = TimeSpan.FromDays(365 * 2);
        public new bool IncludeSubDomains { get; set; } = false;
        public new bool Preload { get; set; } = true;
    }
}

