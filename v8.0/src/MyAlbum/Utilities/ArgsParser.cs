using System.Configuration;

namespace MyAlbum.Utilities
{
    internal class ArgsParser
    {
        internal static Dictionary<string, string?> ParseArgs(string[] args)
        {
            var options = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < args.Length; i++)
            {
                string? key = null;
                string? value = null;

                if (args[i].StartsWith("--"))
                {
                    key = args[i].TrimStart('-');
                }
                else if (args[i].StartsWith("-"))
                {
                    // Map short option to long key
                    switch (args[i][1]) // First character after the single dash
                    {
                        case 'i':
                            key = "input";
                            break;
                        case 'o':
                            key = "output";
                            break;
                        case 'p':
                            key = "page";
                            break;
                        case 't':
                            key = "test";
                            break;
                    }
                }
                if (key != null)
                {
                    // If next arg exists and is not another option, treat as value
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        value = args[i + 1];
                        i++;
                    }
                    options[key] = value;
                }
            }
            return options;
        }

        internal static PageSelection ParsePageSelection(string pageArg)
        {
            PageSelection result = new PageSelection();

            if (string.IsNullOrWhiteSpace(pageArg) || pageArg.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                // Empty or "all" means no filtering - return empty PageSelection
                return result;
            }

            // Split argument into distinct parts by comma
            string[] parts = pageArg.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (string? part in parts)
            {
                string? trimmed = part.Trim();

                // Handle "10+" pattern (page 10 and all subsequent pages)
                if (trimmed.EndsWith('+'))
                {
                    string numberPart = trimmed.TrimEnd('+');
                    if (int.TryParse(numberPart, out int startPage))
                    {
                        result.FromPageOnwards = startPage;
                    }
                }

                if (trimmed.Contains('-'))
                {
                    string[] range = trimmed.Split('-', StringSplitOptions.RemoveEmptyEntries);
                    if (range.Length == 2 && int.TryParse(range[0], out int start) && int.TryParse(range[1], out int end))
                    {
                        for (int i = start; i <= end; i++)
                            result.SpecificPages.Add(i);
                    }
                }
                else if (int.TryParse(trimmed, out int page))
                {
                    result.SpecificPages.Add(page);
                }
            }

            // Deduplicate and sort specific pages
            result.SpecificPages = result.SpecificPages.Distinct().OrderBy(x => x).ToList();
            return result;
        }

        internal static string GetInputFileName(Dictionary<string, string?> options)
        {
            // Input option is required
            if (!options.TryGetValue("input", out string? fileName))
            {
                throw new ArgumentException("The '--input' argument is required.");
            }

            // Input argument must be not null or empty
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("input", "The '--input' argument cannot be empty.");
            }

            string configFolder = ConfigurationManager.AppSettings["InputFolder"] ?? string.Empty;
            string inputFolder = ResolveConfiguredPath(configFolder);
            string fullPath = Path.GetFullPath(Path.Combine(inputFolder, fileName));

            // Check if the directory exists
            string directoryPath = Path.GetDirectoryName(fullPath) ?? "";
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");
            }

            // Check if the file exists
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"The specified input file '{fullPath}' does not exist.");
            }

            // Return the full path to the input file
            return fullPath;
        }

        internal static string GetOutputFileName(Dictionary<string, string?> options, string inputFileName)
        {
            // Check if output option is provided
            if (options.TryGetValue("output", out string? outputFileName) &&
                !string.IsNullOrWhiteSpace(outputFileName))
            {
                // Output does not end with .pdf
                if (!outputFileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    outputFileName += ".pdf";
                }
            }
            else
            {
                outputFileName = Path.GetFileName(inputFileName);
                outputFileName = Path.ChangeExtension(outputFileName, ".pdf");
            }

            string? outputFolder = ConfigurationManager.AppSettings["OutputFolder"];
            if (string.IsNullOrWhiteSpace(outputFolder))
            {
                outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "Output");
            }
            else
            {
                outputFolder = ResolveConfiguredPath(outputFolder);
            }

            // Combine output path with output file name
            string fullPath = Path.Combine(outputFolder, outputFileName);

            // Create the output directory if it does not exist.
            string? outputDir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Increment the file name if a file with the same name already exists
            if (File.Exists(fullPath))
            {
                string filePath = Path.GetDirectoryName(fullPath) ?? "";
                string fileName = Path.GetFileNameWithoutExtension(fullPath);

                for (int i = 1; i < 10000; i++)
                {
                    if (!(File.Exists(fullPath = Path.Combine(filePath, fileName + "_" + i.ToString() + ".pdf"))))
                        break;
                }
            }
            return fullPath;
        }

        private static string ResolveConfiguredPath(string path)
        {
            return Path.GetFullPath(Path.IsPathFullyQualified(path)
                ? path
                : Path.Combine(Directory.GetCurrentDirectory(), path));
        }
    }
}
