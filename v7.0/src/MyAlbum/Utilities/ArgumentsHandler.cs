namespace MyAlbum.Utilities;

/// <summary>
/// Parses and validates command-line arguments for the MyAlbum application.
/// </summary>
public class ArgumentsHandler
{
    /// <summary>
    /// The input album file path (positional argument).
    /// </summary>
    public string? InputFile { get; private set; }

    /// <summary>
    /// The page filter specification (--page or -p option).
    /// Examples: "1,2,6", "1-5", "5+", "1,3,5-10", "all", "*"
    /// </summary>
    public string? PageSpec { get; private set; }

    /// <summary>
    /// Whether to run in test mode (--test or -t flag).
    /// </summary>
    public bool TestMode { get; private set; }

    /// <summary>
    /// Whether to show usage help (no arguments provided).
    /// </summary>
    public bool ShowHelp { get; private set; }

    /// <summary>
    /// Error message if argument parsing or validation failed.
    /// </summary>
    public string? Error { get; private set; }

    /// <summary>
    /// Whether the arguments are valid and ready for processing.
    /// </summary>
    public bool IsValid => Error == null && !ShowHelp && !TestMode;

    /// <summary>
    /// Parse command-line arguments.
    /// </summary>
    /// <param name="args">The command-line arguments array.</param>
    /// <returns>An ArgumentsHandler instance with parsed values.</returns>
    public static ArgumentsHandler Parse(string[] args)
    {
        var handler = new ArgumentsHandler();

        // No arguments - show help
        if (args.Length == 0)
        {
            handler.ShowHelp = true;
            return handler;
        }

        // Check for test mode flag first
        if (args[0] == "--test" || args[0] == "-t")
        {
            handler.TestMode = true;
            return handler;
        }

        // Parse remaining arguments
        for (int i = 0; i < args.Length; i++)
        {
            if ((args[i] == "--page" || args[i] == "-p") && i + 1 < args.Length)
            {
                // Page filter option with value
                handler.PageSpec = args[++i];
            }
            else if (args[i].StartsWith('-'))
            {
                // Unknown option - ignore for now (could add error handling)
            }
            else
            {
                // Positional argument - first one is the input file
                handler.InputFile ??= args[i];
            }
        }

        // Validate required arguments
        if (handler.InputFile == null)
        {
            handler.Error = "No input file specified";
            return handler;
        }

        if (!File.Exists(handler.InputFile))
        {
            handler.Error = $"File not found: {handler.InputFile}";
            return handler;
        }

        return handler;
    }

    /// <summary>
    /// Print usage information to the console.
    /// </summary>
    public static void PrintUsage()
    {
        Console.WriteLine("Usage: myalbum <album-file> [options]");
        Console.WriteLine("       myalbum --test | -t");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --page | -p <pages>   Filter pages to process");
        Console.WriteLine("                        Examples: 1,2,6  |  1-5  |  5+  |  1,3,5-10  |  all  |  *");
        Console.WriteLine();
        Console.WriteLine("Supported formats: .xml, .json, .yml, .yaml");
    }
}
