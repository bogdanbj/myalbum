using System.Configuration;

namespace MyAlbum.Utilities
{
    internal static class IOHelper
    {
        /// <summary>
        /// Reads a folder path from App.config by key, returning <paramref name="defaultValue"/>
        /// when the key is missing. Relative values are anchored to the executable's
        /// base directory so resolution is independent of the current working directory.
        /// </summary>
        internal static string ResolvePath(string configKey, string defaultValue)
        {
            var configPath = ConfigurationManager.AppSettings[configKey] ?? defaultValue;
            return Path.IsPathFullyQualified(configPath)
                ? configPath
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configPath));
        }
    }
}
