using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace MyAlbum2 {
	class AlbumPage {
		static public double XLeft { get; set; }
		static public double XRight { get; set; }
		static public double MarginLeft { get; set; }
		static public double MarginRight { get; set; }
		static public double MarginTop { get; set; }
		static public double MarginBottom { get; set; }
		static public BorderPlain Border { get; set; }


		//public Border Border { get; set; }

        public static XFont GetFont(string arguments)
        {
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

            XFont font = new XFont("Arial", 12, XFontStyle.Regular, options);

            string[] args = arguments.Split(',');

            if (args.Length != 3) { throw new Exception("Incorrect number of parameters"); }

            try
            {
                XFontStyle fontStyle;

                fontStyle = XFontStyle.Regular;

                if (args[2].ToUpper().IndexOf("BOLD") >= 0)
                {
                    fontStyle = XFontStyle.Bold;
                }
                if (args[2].ToUpper().IndexOf("ITALIC") >= 0)
                {
                    fontStyle = fontStyle | XFontStyle.Italic;
                }
                if (args[2].ToUpper().IndexOf("UNDERLINE") >= 0)
                {
                    fontStyle = fontStyle | XFontStyle.Underline;
                }
                if (args[2].ToUpper().IndexOf("STRIKEOUT") >= 0)
                {
                    fontStyle = fontStyle | XFontStyle.Strikeout;
                }

                font = new XFont(args[0].Trim(), Convert.ToDouble(args[1].Trim()), fontStyle, options);
            }
            catch (Exception)
            {
                throw new Exception(String.Format("GetFont({0}): Invalid parameter", arguments));
            }

            return font;
        }


	}
}
