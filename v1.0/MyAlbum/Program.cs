using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;




namespace MyAlbum
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName;
            int i = 0;
            Album album = new Album();


            try
            {
                fileName = args[0];
                
                System.IO.StreamReader reader = new System.IO.StreamReader(fileName, System.Text.Encoding.GetEncoding(0), true);
                string command;

                while ((command = reader.ReadLine()) != null)
                {
                    i++;
                    if (command.Trim().Length == 0) { continue; }
                    if (command.Trim().Substring(0, 1) == "#") { continue; }
                    if (command.Trim().ToUpper() == "END ALBUM") { break; }
                    album.Draw(command);
                }

                album.Save(fileName = fileName.Replace(Path.GetExtension(fileName), ".pdf"));
                // ...and start a viewer.
                Process.Start(fileName);

                reader.Close();
                //Console.WriteLine("\nPress any key to continue:");
                //Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string[] ParseArguments(string commandLine)
        {
            char[] parmChars = commandLine.ToCharArray();
            bool escaped = false;
            for (int index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '\\')
                    escaped = true;
                else 
                {
                    if (!escaped && parmChars[index] == ',')
                        parmChars[index] = '\n';
                    escaped = false;
                }
            }

            string[] ret = (new string(parmChars)).Split('\n');
            for (int i = 0; i < ret.Length; i++)
                ret[i] = ret[i].Trim().Replace("\\,", ",");
            return ret;
        }
        public static string TrimMatchingQuotes(string input, char quote)
        {
            if ((input.Length >= 2) &&
                (input[0] == quote) && (input[input.Length - 1] == quote))
                return input.Substring(1, input.Length - 2);

            return input;
        }
        public static string TrimMatchingBrackets(string input, char startBracket, char endBracket)
        {
            if ((input.Length >= 2) &&
                (input[0] == startBracket) && (input[input.Length - 1] == endBracket))
                return input.Substring(1, input.Length - 2);

            return input;
        }
    }
}
