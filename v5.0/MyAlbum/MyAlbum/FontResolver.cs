using PdfSharp.Fonts;
using PdfSharp.Internal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static PdfSharp.Snippets.Font.SegoeWpFontResolver;

namespace MyAlbum
{
    internal class FontResolver : IFontResolver
    {

        public byte[] GetFont(string faceName)
        {
            string fontPath = ConfigurationManager.AppSettings["FontPath"];
            switch (faceName)
            {
                case "Century Gothic":
                    return LoadFontData($"{fontPath}\\Century Gothic\\century-gothic-regular.ttf");
                case "Century Gothic Bold":
                    return LoadFontData($"{fontPath}\\Century Gothic\\century-gothic-bold.ttf");
                case "Century Gothic Italic":
                    return LoadFontData($"{fontPath}\\Century Gothic\\century-gothic-italic.ttf");
                case "Century Gothic Bold Italic":
                    return LoadFontData($"{fontPath}\\Century Gothic\\century-gothic-bold-italic.ttf");

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
                case "Stymie Becker Light Italic":
                    return LoadFontData($"{fontPath}\\Stymie Becker\\stymie_becker_light.ttf");
            }
            return null;
        }

#nullable enable
        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {

            FontResolverInfo? fontResolverInfo;

            string faceName = familyName;
            if (isBold) faceName += " Bold";
            if (isItalic) faceName += " Italic";

            fontResolverInfo = new FontResolverInfo(faceName);

            if (fontResolverInfo != null)
                return fontResolverInfo;

            // fallback to platform default resolver
            fontResolverInfo = PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);

            // fallback to "Verdana"
            if (fontResolverInfo == null)
                fontResolverInfo = PlatformFontResolver.ResolveTypeface("Verdana", isBold, isItalic);

            return fontResolverInfo!;

        }
#nullable restore

        private byte[] LoadFontData(string fontPath)
        {
            return System.IO.File.ReadAllBytes(fontPath);
        }
    }
}
