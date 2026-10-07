using MyAlbum.Utilities;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class Image : BaseElement<ImageStyle>
    {
        #region Fields
        protected bool? _absolute;
        //protected bool? _rotate;
        #endregion

        #region Style properties
        public string? FileName { get; set; }
        public bool Absolute
        {
            get => _absolute ?? Style.Absolute ?? false;
            set => _absolute = value;
        }
        //public bool Rotate
        //{
        //    get => _rotate ?? Style.Rotate ?? false;
        //    set => _rotate = value;
        //}
        #endregion

        #region Other properties
        public XImage? XImg { get; set; }
        #endregion

        #region Constructors
        public Image()
        {
            Style = Styles.Image.GetStyle("default") ?? new ImageStyle();
        }
        #endregion

        #region Override methods
        internal override void ParseXml(XElement xElem)
        {
            Style = Styles.Image.GetStyle(xElem.Attribute("style")?.Value);
            base.ParseXml(xElem);

            FileName = xElem.Attribute("file-name")?.Value;
            XImg = Load(FileName);
            _absolute = XmlParser.ParseBool(xElem.Attribute("absolute")?.Value);
            //_rotate = XmlParser.ParseBool(xElem.Attribute("rotate")?.Value);
            if (Absolute)
            {
                X = XUnit.FromMillimeter(XmlParser.ParseDouble(xElem.Attribute("x")?.Value) ?? 0);
                Y = XUnit.FromMillimeter(XmlParser.ParseDouble(xElem.Attribute("y")?.Value) ?? 0);
            }

        }
        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            /*
             W and H are always calculated here from the "width" and "height" attributes:
             - If neither width nor height is provided, we use the image's original dimensions.
             - If only width is provided, we infer height from the image's aspect ratio.
             - If only height is provided, we infer width from the image's aspect ratio.
             - If both are provided, the image is stretched to cover the whole defined area.
             width may be expressed as a percentage relative to the parent canvas width.
             At the end, both W and H hold valid XUnit values greater than XUnit.Zero.

             X and Y:
             - If Absolute is true, we keep the X, Y values parsed from the "x", "y" attributes.
             - Otherwise they are calculated relative to the parent canvas.
             */

            // Determine is width and height attributes are provided.
            bool hasWidth = !string.IsNullOrEmpty(Width);
            bool hasHeight = !string.IsNullOrEmpty(Height);

            // Store the original X, Y values before calling base.Calculate
            XUnit absoluteX = X;
            XUnit absoluteY = Y;

            base.Calculate(gfx, parentCanvas);

            // If Absolute is true, we use the original X, Y values
            if (Absolute)
            {
                // Use the absolute X, Y values as is
                X = absoluteX;
                Y = absoluteY;
            }

            double aspectRatio = (XImg != null && XImg.PixelHeight != 0)
                ? (double)XImg.PixelWidth / XImg.PixelHeight
                : 1.0;

            if (hasWidth && hasHeight)
            {
                // Both provided: base.Calculate already resolved W and H
                // (absolute or %). Stretch to cover the defined area.
            }
            else if (hasWidth)
            {
                // Only width provided: W is resolved by base.Calculate;
                // infer height from aspect ratio.
                H = XUnit.FromPoint(W.Point / aspectRatio);
            }
            else if (hasHeight)
            {
                // Only height provided: H is resolved by base.Calculate;
                // infer width from aspect ratio.
                W = XUnit.FromPoint(H.Point * aspectRatio);
            }
            else
            {
                // Neither provided: use the image's original dimensions.
                W = XImg != null ? XUnit.FromPoint(XImg.PointWidth) : XUnit.Zero;
                H = XImg != null ? XUnit.FromPoint(XImg.PointHeight) : XUnit.Zero;
            }

        }
        internal override void Draw(XGraphics gfx)
        {
            base.Draw(gfx);
            if (XImg != null)
            {
                gfx.DrawImage(XImg, 0, 0, W.Point, H.Point);
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
                string? imagePath = ConfigurationManager.AppSettings["ImagesFolder"];

                if (!string.IsNullOrWhiteSpace(imagePath))
                {
                    imagePath = Path.GetFullPath(Path.IsPathFullyQualified(imagePath)
                        ? imagePath
                        : Path.Combine(Directory.GetCurrentDirectory(), imagePath));
                }
                else
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Images");
                }

                if (!Directory.Exists(imagePath))
                    throw new DirectoryNotFoundException($"Error: Image directory not found: {imagePath}");

                string fullPath = Path.Combine(imagePath, fileName);

                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"? File not found: {fullPath}");
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
