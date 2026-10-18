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
        #endregion

        #region Other properties
        public XImage? XImg { get; set; }
        public double AspectRatio { get; set; }
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
            if (Absolute)
            {
                X = XUnit.FromMillimeter(XmlParser.ParseDouble(xElem.Attribute("x")?.Value) ?? 0);
                Y = XUnit.FromMillimeter(XmlParser.ParseDouble(xElem.Attribute("y")?.Value) ?? 0);
            }
        }
        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            /*
             * Image Width and Height set the printed image sizes - which is the object Canvas.
             * W and H are calculated by adding the MArgins.
             *
             * Canvas W and H are always calculated here from the "width" and "height" attributes:
             * - If neither width nor height is provided, we use the image's original dimensions.
             * - If only width is provided, we infer height from the image's aspect ratio.
             * - If only height is provided, we infer width from the image's aspect ratio.
             * - If both are provided, the image is stretched to cover the whole defined area.
             * Width may be expressed as a percentage relative to the parent canvas width.
             * At the end, both W and H hold valid XUnit values greater than XUnit.Zero.
             * 
             * X and Y:
             * - If Absolute is true, we keep the X, Y values parsed from the "x", "y" attributes.
             * - Otherwise they are calculated relative to the parent canvas.
             */

            XUnit absoluteX = X, absoluteY = Y;
            base.Calculate(gfx, parentCanvas);
            if (Absolute)
            {
                X = absoluteX;
                Y = absoluteY;
            }

            // Determine is width and height attributes are provided.
            bool hasWidth = !string.IsNullOrEmpty(Width);
            bool hasHeight = !string.IsNullOrEmpty(Height);

            AspectRatio = (XImg != null && XImg.PixelHeight != 0)
                ? (double)XImg.PixelWidth / XImg.PixelHeight
                : 1.0;

            // For image, the Width and Height attribute refer to the actual drawn image, which in this case is the canvas.
            // Margins are added to the canvas to calculate the space taken on paper by the object.
            if (hasWidth && hasHeight)
            {
                Canvas.W = CalculateWidth(parentCanvas.W);
                Canvas.H = CalculateHeight(parentCanvas.H);
                W = Canvas.W + (MarginLeft + MarginRight);
                H = Canvas.H + (MarginTop + MarginBottom);
            }
            else if (hasWidth)
            {
                // Only width provided: infer height from aspect ratio.
                Canvas.W = CalculateWidth(parentCanvas.W);
                Canvas.H = XUnit.FromPoint(Canvas.W.Point / AspectRatio);
                W = Canvas.W + (MarginLeft + MarginRight);
                H = Canvas.H + (MarginTop + MarginBottom);
            }
            else if (hasHeight)
            {
                // Only height provided: infer width from aspect ratio.
                Canvas.H = CalculateHeight(parentCanvas.H);
                Canvas.W = XUnit.FromPoint(Canvas.H.Point * AspectRatio);
                W = Canvas.W + (MarginLeft + MarginRight);
                H = Canvas.H + (MarginTop + MarginBottom);
            }
            else
            {
                // Neither provided: use the image's original dimensions.
                Canvas.W = XUnit.FromPoint(XImg?.PointWidth  ?? 0);
                Canvas.H = XUnit.FromPoint(XImg?.PointHeight ?? 0);
                W = Canvas.W + (MarginLeft + MarginRight);
                H = Canvas.H + (MarginTop + MarginBottom);
            }

        }
        internal override void Draw(XGraphics gfx)
        {
            base.Draw(gfx);
            if (XImg != null)
            {
                gfx.DrawImage(XImg, MarginLeft.Point, MarginTop.Point, Canvas.W.Point, Canvas.H.Point);
            }
        }
        #endregion

        #region Private Methods
        internal XImage? Load(string? fileName)
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
