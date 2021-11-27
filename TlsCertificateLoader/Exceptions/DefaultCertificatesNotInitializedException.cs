using System;

namespace TlsCertificateLoader.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the default certificates are not initialized.
    /// </summary>
    [Serializable]
    public sealed class DefaultCertificatesNotInitializedException : InvalidOperationException
    {
        internal DefaultCertificatesNotInitializedException() : base("Default certificates not initialized.")
        { }
    }
}
