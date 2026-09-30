using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace MyAlbum.Utilities
{
    internal class ArgsParser
    {
        internal static Dictionary<string, string> ParseArgs(string[] args)
        {
            var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < args.Length; i++)
            {
                string key = null;
                string value = null;

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
        internal static string GetInputFileName(Dictionary<string, string> options)
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

            // Get input folder from app.config. It might be null (not set)
            string? configFolder = ConfigurationManager.AppSettings["InputFilePath"];

            // Get current directory
            string currentDirectory = Directory.GetCurrentDirectory();

            // Build the input file path.
            string fullPath = Path.Combine(
                currentDirectory,
                configFolder ?? string.Empty,
                fileName
                );
            /*
             *  Note: 
             *  If fileName is rooted, fullPath = fileName
             *  If fileName is not rooted, but configFolder is rooted, fullPath = configFolder + fileName
             *  If fileName and configFolder are not rooted, fullPath = currentDirectory + configFolder + fileName
             *  -----
             *  A rooted path is file path that is fixed to a specific drive or UNC path; it contrasts with a path 
             *  that is relative to the current drive or working directory. For example, on Windows systems, a rooted 
             *  path begins with a backslash (for example, "\Documents") or a drive letter and colon (for example, "C:Documents"). 
             *  -----
             *  Path.Combine starts from the rightmost rooted path, ignoring the preceding paths. If no rooted path is found, 
             *  the result is the combination of all paths.
            */

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
        internal static string GetOutputFileName(Dictionary<string, string> options, string inputFileName)
        {
            // Check if output option is provided
            if (options.TryGetValue("output", out string? outputFileName))
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


            // Check if the app.config has an output folder specified
            string? outputFolder = ConfigurationManager.AppSettings["OutputFilePath"];
            if (string.IsNullOrWhiteSpace(outputFolder))
            {
                outputFolder = (Path.GetDirectoryName(inputFileName) ?? "").Replace("Templates", "Output");
            }

            // Combine output path with output file name
            string fullPath = Path.Combine(
                outputFolder,
                outputFileName
                );

            // Create the output directory if is does not exist.
            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }

            // Increment the file name if a file with the same name already exists
            if (File.Exists(fullPath))
            {
                string filePath = Path.GetDirectoryName(fullPath) ?? "";
                string fileName = Path.GetFileNameWithoutExtension(fullPath);
                
                for (int i = 1; i < 10000; i++)
                {
                    if (!(File.Exists(fullPath = filePath + "\\" + fileName + "_" + i.ToString() + ".pdf")))
                        break;
                }
            }
            return fullPath;
        }
    }
}
