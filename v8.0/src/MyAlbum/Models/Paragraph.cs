using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAlbum.Models
{
    internal class Paragraph : Text
    {
        protected override string[] Split(XGraphics gfx)
        {
            if (string.IsNullOrEmpty(Value))
                return [""];

            var result = new List<string>();

            // Explicit line breaks
            string[] longLines = Value.Split(sep,StringSplitOptions.None);

            // Try to wrap if needed
            foreach (string longLine in longLines)
            {

                // If the line fits, just add it to the result.
                if (gfx.MeasureString(longLine, Font).Width <= Canvas.W)
                {
                    result.Add(longLine);
                    continue;
                }

                // If the line is longer than Width, wrap it.
                string shortLine = string.Empty;
                foreach (string word in longLine.Split(' '))
                {
                    string candidate = shortLine.Length == 0 ? word : $"{shortLine} {word}";
                        
                    if (gfx.MeasureString(candidate, Font).Width <= Canvas.W)
                    {
                        // Fits on the current line.
                        shortLine = candidate;
                    }
                    else
                    {
                        result.Add(shortLine);
                        shortLine = word;
                    }
                }
                result.Add(shortLine);
            }
            return result.ToArray();
        }

        // Paragraph fills the available width, so keep W rather than shrinking
        // it to the widest wrapped line (the base behavior).
        protected override XUnit CalculateWidth(XGraphics gfx) => W;

    }
}
