using System;
using System.Collections.Generic;

namespace TlsCertificateLoader.Exceptions;

/// <summary>
/// The exception that is thrown when the additional certificates requested are not found in the dictionary.
/// </summary>
[Serializable]
public sealed class AdditionalCertificatesNotFoundException : KeyNotFoundException
{
    /// <summary>
    /// The hostname provided
    /// </summary>
    public string HostName { get; internal set; }

    internal AdditionalCertificatesNotFoundException(string hostname) : base("Additional certificates not found.")
    {
        HostName = hostname;
    }
}
