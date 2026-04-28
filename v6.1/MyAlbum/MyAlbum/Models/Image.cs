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
        public XImage? XImg { get; set; }

        #region Override methods
        internal override void ParseXml(XElement xElem)
        {
            Style = Styles.Frame.GetStyle(xElem.Attribute("style")?.Value);
            base.ParseXml(xElem);

            FileName = xElem.Attribute("file-name")?.Value;
            XImg = Load(FileName);
        }
        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        { }
        internal override void Draw(XGraphics gfx)
        {
            base.Draw(gfx);
            gfx.DrawImage(XImg,)
        }
        #endregion

            #region Private Methods
        private XImage? Load(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Image's FileName is null or emplty.");
                return null;
            }

            XImage? result = null;
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

                if (XImage.ExistsFile(fullPath))
                {
                    result = XImage.FromFile(fullPath);
                }
                else
                {
                    Console.WriteLine($"Warning: Image file not found at '{fullPath}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image '{fileName}': {ex.Message}");
            }

            return result;
        }
        #endregion

    }
}
