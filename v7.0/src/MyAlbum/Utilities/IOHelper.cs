using System.Configuration;

namespace MyAlbum.Utilities;

/// <summary>
/// Helper methods for resolving paths from configuration.
/// </summary>
public static class IOHelper
{
    /// <summary>
    /// Get the working directory from config, or fall back to executable directory.
    /// </summary>
    public static string GetWorkingDirectory()
    {
        string? workingDir = ConfigurationManager.AppSettings["WorkingDirectory"];
        if (string.IsNullOrWhiteSpace(workingDir))
            return AppContext.BaseDirectory;
        return Path.IsPathFullyQualified(workingDir)
            ? workingDir
            : Path.Combine(AppContext.BaseDirectory, workingDir);
    }

    /// <summary>
    /// Resolve a path from config, relative to the working directory.
    /// </summary>
    public static string ResolvePath(string configKey, string defaultValue)
    {
        // string? configPath = ConfigurationManager.AppSettings[configKey] ?? defaultValue;
        string configPath = ConfigurationManager.AppSettings[configKey];
        if (string.IsNullOrWhiteSpace(configPath))
            configPath = defaultValue;
        
        return Path.IsPathFullyQualified(configPath)
            ? configPath
            : Path.Combine(GetWorkingDirectory(), configPath);
    }

    /// <summary>
    /// Get the input folder path.
    /// </summary>
    public static string GetInputFolder() => ResolvePath("InputFolder", "Templates");

    /// <summary>
    /// Get the output folder path.
    /// </summary>
    public static string GetOutputFolder() => ResolvePath("OutputFolder", "Output");

    /// <summary>
    /// Get the fonts folder path.
    /// </summary>
    public static string GetFontsFolder() => ResolvePath("FontsFolder", "Fonts");

    /// <summary>
    /// Get the images folder path.
    /// </summary>
    public static string GetImagesFolder() => ResolvePath("ImagesFolder", "Images");
}
