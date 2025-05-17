using System;
using System.Reflection;

namespace PatternCipher.Utilities.Common
{
    /// <summary>
    /// Provides access to the Shared-Utilities library's version information.
    /// </summary>
    /// <remarks>
    /// Allows consumers to programmatically access the version of this shared library, aiding in diagnostics and compatibility checks.
    /// Typically reads the version from an assembly attribute.
    /// </remarks>
    public static class VersionInfo
    {
        private static string? _libraryVersion;

        /// <summary>
        /// Gets the current version of the Shared-Utilities library.
        /// Reads the AssemblyInformationalVersionAttribute if available, otherwise AssemblyVersion.
        /// The value is cached after the first access.
        /// </summary>
        public static string LibraryVersion
        {
            get
            {
                if (_libraryVersion == null)
                {
                    try
                    {
                        Assembly assembly = typeof(VersionInfo).Assembly;

                        var informationalVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                        if (informationalVersionAttribute != null && !string.IsNullOrEmpty(informationalVersionAttribute.InformationalVersion))
                        {
                            _libraryVersion = informationalVersionAttribute.InformationalVersion;
                        }
                        else
                        {
                            var assemblyVersion = assembly.GetName().Version;
                            _libraryVersion = assemblyVersion?.ToString() ?? "0.0.0.0-unknown";
                        }
                    }
                    catch (Exception)
                    {
                        // In case of any error during reflection, provide a fallback.
                        _libraryVersion = "Error Retrieving Version";
                    }
                }
                return _libraryVersion;
            }
        }
    }
}