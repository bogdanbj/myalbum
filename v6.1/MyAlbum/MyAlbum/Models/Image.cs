using MyAlbum.Utilities;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf.IO.enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class Image : BaseElement
    {
        public string? FileName { get; set; }
        public bool Absolute { get; set; }
        public bool Stretched { get; set; }
        public XImage? XImg { get; set; }

        #region Override methods
        internal override void ParseXml(XElement xElem)
        {
            Style = Styles.Frame.GetStyle(xElem.Attribute("style")?.Value);
            base.ParseXml(xElem);

            FileName = xElem.Attribute("file-name")?.Value;
            XImg = Load(FileName);
            Absolute = XmlParser.ParseBool(xElem.Attribute("absolute")?.Value);
            Stretched = XmlParser.ParseBool(xElem.Attribute("stretched")?.Value);
            if (Absolute)
            {
                X = XUnit.FromMillimeter(XmlParser.ParseDouble(xElem.Attribute("x")?.Value) ?? 0);
                Y = XUnit.FromMillimeter(XmlParser.ParseDouble(xElem.Attribute("y")?.Value) ?? 0);
                H = XUnit.FromMillimeter(XmlParser.ParseDouble(xElem.Attribute("height")?.Value) ?? 0);
                W = XUnit.FromMillimeter(XmlParser.ParseDouble(xElem.Attribute("width")?.Value) ?? 0);
            }

        }
        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        { }
        internal override void Draw(XGraphics gfx)
        {
            base.Draw(gfx);
            if (XImg != null)
            {
                if (this.Stretched)
                {
                    gfx.DrawImage(XImg, X, Y, W, H);
                }
                else
                {
                    gfx.DrawImage(XImg, X, Y);
                }
            }
        }
        #endregion

        #region Private Methods
        private XImage? Load(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Image's FileName is null or empty.");
                return null;
            }

            //XImage? result = null;
            try
            {
                string? imagePath = ConfigurationManager.AppSettings["ImagePath"];

                if (!string.IsNullOrWhiteSpace(imagePath))
                {
                    if (!Path.IsPathRooted(imagePath))
                    {
                        imagePath = Path.Combine(Directory.GetCurrentDirectory(), imagePath);
                    }
                }
                else
                {
                    imagePath = Path.Combine(Environment.CurrentDirectory, "Images");
                }

                if (!Directory.Exists(imagePath))
                    throw new DirectoryNotFoundException($"Error: Image directory not found: {imagePath}");

                string fullPath = Path.Combine(imagePath, fileName);

                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"✗ File not found: {fullPath}");
                    return null;
                }

                // Now safely call FromFile
                XImage result = XImage.FromFile(fullPath);
                //string jpegSamplePath = "C:\\Git\\myalbum\\Images\\0284.jpg";
                //XImage image = XImage.FromFile(jpegSamplePath);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image '{fileName}': {ex.Message}");
            }

            return null;
        }
        #endregion

    }
}
