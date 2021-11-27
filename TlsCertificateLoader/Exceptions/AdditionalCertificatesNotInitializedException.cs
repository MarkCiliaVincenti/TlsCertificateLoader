using System;

namespace TlsCertificateLoader.Exceptions
{
    /// <summary>
    /// The exception that is thrown when additional certificates are not initialized.
    /// </summary>
    [Serializable]
    public sealed class AdditionalCertificatesNotInitializedException : InvalidOperationException
    {
        internal AdditionalCertificatesNotInitializedException() : base("Additional certificates not initialized.")
        { }
    }
}
