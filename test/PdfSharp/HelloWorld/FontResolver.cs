using PdfSharp.Fonts;
using PdfSharp.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static PdfSharp.Snippets.Font.SegoeWpFontResolver;

namespace HelloWorld
{
    internal class FontResolver : IFontResolver
    {

        public byte[] GetFont(string faceName)
        {
            string fontPath = "C:\\Git\\MyAlbum\\Fonts";
            switch (faceName)
            {
                case "Century Gothic":
                    return LoadFontData($"{fontPath}\\Century Gothic\\CenturyGothic.ttf");
                case "Century Gothic Bold":
                    return LoadFontData($"{fontPath}\\Century Gothic\\CenturyGothic_Bold.ttf");

                case "Folio":
                    return LoadFontData($"{fontPath}\\Folio Bk BT\\FolioBT_Book.ttf"); 
                case "Folio Bold":
                    return LoadFontData($"{fontPath}\\Folio Bk BT\\FolioBT_Bold.ttf");

                case "Garamond":
                    return LoadFontData($"{fontPath}\\Garamond\\Garamond Regular.ttf");
                case "Garamond Bold":
                    return LoadFontData($"{fontPath}\\Garamond\\Garamond Bold.ttf");
                case "Garamond Italic":
                    return LoadFontData($"{fontPath}\\Garamond\\Garamond Italic.ttf");
                case "Garamond Bold Italic":
                    return LoadFontData($"{fontPath}\\Garamond\\Garamond Bold Italic.ttf");

                case "Lubalin Graph":
                    return LoadFontData($"{fontPath}\\Lubalin Graph\\Lubalin Graph Regular.ttf");
                case "Lubalin Graph Bold":
                    return LoadFontData($"{fontPath}\\Lubalin Graph\\Lubalin Graph Bold.ttf");
                case "Lubalin Graph Italic":
                    return LoadFontData($"{fontPath}\\Lubalin Graph\\Lubalin Graph Italic.ttf");
                case "Lubalin Graph Bold Italic":
                    return LoadFontData($"{fontPath}\\Lubalin Graph\\Lubalin Graph Bold Italic.ttf");

                case "Stymie Becker":
                    return LoadFontData($"{fontPath}\\Stymie Becker\\stymie_becker.ttf");
                case "Stymie Becker Light":
                    return LoadFontData($"{fontPath}\\Stymie Becker\\stymie_becker_light.ttf");
            }
            return null;
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            FontResolverInfo? fontResolverInfo;
            
            string suffix = "";
            if (isBold && isItalic)
                suffix = " Bold Italic";
            else if (isBold)
                suffix = " Bold";
            else if (isItalic)
                suffix = " Italic";

            fontResolverInfo = new FontResolverInfo($"{familyName}{suffix}");


            switch (familyName)
            {
                case "Century Gothic":
                    return new FontResolverInfo($"Century Gothic{suffix}");
                case "Folio":
                    return new FontResolverInfo($"Folio{suffix}");
                case "Garamond":
                    return new FontResolverInfo($"Garamond{suffix}");
                case "Lubalin Graph":
                    return new FontResolverInfo($"Lubalin Graph{suffix}");
                case "Stymie Becker Light":
                    return new FontResolverInfo($"Stymie Becker Light{suffix}");
            }


            var font = PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);

            if (font == null)
            {
                // Fallback to a default font
                font = PlatformFontResolver.ResolveTypeface("Verdana", isBold, isItalic);
            }

            return font;
            
            //return PdfSharp.Fonts.PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);

        }


        private byte[] LoadFontData(string fontPath)
        {
            return System.IO.File.ReadAllBytes(fontPath);
        }
    }
}
