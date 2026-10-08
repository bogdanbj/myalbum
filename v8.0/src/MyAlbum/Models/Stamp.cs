using MyAlbum.Utilities;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class Stamp : BaseElement<StampStyle>
    {
        #region Fields
        protected XUnit? _vSpace;
        #endregion

        #region Style properties
        public XUnit VSpace
        {
            get => _vSpace ?? Style?.VSpace ?? XUnit.Zero;
            set => _vSpace = value;
        }
        #endregion

        #region Other properties
        // The stamp has exactly these predefined parts: 1 title, 1 frame, 1 image,
        // 3 insiders (I1/I2/I3) and 3 footers (F1/F2/F3). They are always created,
        // even when their content value is empty.
        public Text Title { get; set; } = new Text();
        public Frame Frame { get; set; } = new Frame();
        public Image Image { get; set; } = new Image();
        public Text I1 { get; set; } = new Text();
        public Text I2 { get; set; } = new Text();
        public Text I3 { get; set; } = new Text();
        public Text F1 { get; set; } = new Text();
        public Text F2 { get; set; } = new Text();
        public Text F3 { get; set; } = new Text();
        public XUnit StampWidth { get; set; }
        public XUnit StampHeight { get; set; }
        #endregion

        #region Constructors
        public Stamp()
        {
            Style = Styles.Stamp.GetStyle("default") ?? new StampStyle();
        }
        #endregion

        #region Override methods
        internal override void ParseXml(XElement xElem)
        {
            Style = Styles.Stamp.GetStyle(xElem.Attribute("style")?.Value) ?? new StampStyle();
            base.ParseXml(xElem);
            if (string.IsNullOrEmpty(Width) || string.IsNullOrEmpty(Height))
            {
                throw new InvalidOperationException(
                    $"A <stamp> element requires both 'width' and 'height' attributes. (Page {PageNo}: {xElem})");
            }

            _vSpace = XmlParser.ParseXUnit(xElem.Attribute("vspace")?.Value);

            // Appearance comes from the stamp STYLE's composition children; the
            // content comes from the <stamp> element attributes. The user-facing
            // style tags are title/frame/image/inside1..3/footer1..3, while the
            // content attributes are title/image/i1..i3/f1..f3.
            foreach (XElement styleElement in Style.ChildElements)
            {
                switch (styleElement.Name.LocalName)
                {
                    case "title":
                        //ConfigureText(Title, styleElement, xElem.Attribute("title")?.Value);
                        Title.Inherit(this);
                        Title.ParseXml(styleElement);
                        Title.Value = xElem.Attribute("title")?.Value;
                        break;
                    case "frame":
                        Frame.Inherit(this);
                        Frame.ParseXml(styleElement);
                        break;
                    case "image":
                        Image.Inherit(this);
                        Image.ParseXml(styleElement);
                        Image.XImg = Image.Load(xElem.Attribute("image")?.Value);
                        break;
                    case "inside1":
                        I1.Inherit(this);
                        I1.ParseXml(styleElement);
                        I1.Value = xElem.Attribute("i1")?.Value;
                        break;
                    case "inside2":
                        I2.Inherit(this);
                        I2.ParseXml(styleElement);
                        I2.Value = xElem.Attribute("i2")?.Value;
                        break;
                    case "inside3":
                        I3.Inherit(this);
                        I3.ParseXml(styleElement);
                        I3.Value = xElem.Attribute("i3")?.Value;
                        break;
                    case "footer1":
                        F1.Inherit(this);
                        F1.ParseXml(styleElement);
                        F1.Value = xElem.Attribute("f1")?.Value;
                        break;
                    case "footer2":
                        F2.Inherit(this);
                        F2.ParseXml(styleElement);
                        F2.Value = xElem.Attribute("f2")?.Value;
                        break;
                    case "footer3":
                        F3.Inherit(this);
                        F3.ParseXml(styleElement);
                        F3.Value = xElem.Attribute("f3")?.Value;
                        break;
                    default:
                        break;
                }
            }
        }

        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            base.Calculate(gfx, parentCanvas);

            StampWidth = XmlParser.ParseXUnit(Width) ?? XUnit.Zero;
            StampHeight = XmlParser.ParseXUnit(Height) ?? XUnit.Zero;
            if (StampWidth == XUnit.Zero || StampHeight == XUnit.Zero)
            {
                throw new InvalidOperationException(
                    $"Invalid 'width' and 'height' attributes. (Page {PageNo})");
            }


            // Calculate Title
            Title.Calculate(gfx, Canvas);
            Title.X = XUnit.Zero; // will be centered after the Frame is calculated
            Title.Y = Title.MarginTop;
            Canvas.Y += Title.H > XUnit.Zero
                ? Title.H + Title.MarginTop + Title.MarginBottom + VSpace
                : XUnit.Zero;

            // Calculate Frame
            Frame.CalculateFrameSidesWidth();
            Frame.W = Frame.MarginLeft + Frame.WidthLeft + Frame.PaddingLeft + StampWidth + Frame.PaddingRight + Frame.WidthRight + Frame.MarginRight;
            Frame.H = Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + StampHeight + Frame.PaddingBottom + Frame.WidthBottom + Frame.MarginBottom;
            Frame.X = XUnit.Zero; 
            Frame.Y = Canvas.Y;
            this.Canvas.W = Frame.W;

            Title.X = (Frame.W - Title.W) / 2; // center the title relative to the frame

            // Calculate Image
            Image.Calculate(gfx, Canvas);
            Image.W = this.W - Image.MarginLeft - Image.MarginRight;
            Image.H = this.H - Image.MarginTop - Image.MarginBottom;
            Image.X = (Frame.W - Image.W) / 2; // center the insiders relative to the frame
            Image.Y = Canvas.Y + Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + StampHeight / 2 - Image.H / 2;

            // Calculate Insiders (I1, I2, I3)
            Text I = new Text();
            I.Value = string.Join("\n",
                new[] { I1.Value, I2.Value, I3.Value }
                    .Where(v => !string.IsNullOrEmpty(v)));
            I.Calculate(gfx, Canvas);
            I1.Calculate(gfx, Canvas);
            I2.Calculate(gfx, Canvas);
            I3.Calculate(gfx, Canvas);

            I1.X = (Frame.W - I1.W) / 2;
            I2.X = (Frame.W - I2.W) / 2;
            I3.X = (Frame.W - I3.W) / 2;

            XUnit insiderHeight = 
                (string.IsNullOrEmpty(I1.Value) ? XUnit.Zero : I1.H + VSpace)
                + (string.IsNullOrEmpty(I2.Value) ? XUnit.Zero : I2.H + VSpace)
                + (string.IsNullOrEmpty(I3.Value) ? XUnit.Zero : I3.H + VSpace);
            insiderHeight -= insiderHeight > XUnit.Zero ? VSpace : XUnit.Zero;

            I1.Y = Canvas.Y + Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + StampHeight / 2 - insiderHeight / 2;
            I2.Y = I1.H > XUnit.Zero ? I1.Y + I1.H + VSpace : I1.Y;
            I3.Y = I2.H > XUnit.Zero ? I2.Y + I2.H + VSpace : I2.Y;

            //I.X = (Frame.W - I.W) / 2; // center the insiders relative to the frame
            //I.Y = Canvas.Y + Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + StampHeight / 2 - I.H / 2;

            this.Canvas.Y += Frame.H;

            // Calculate Footers (F1, F2, F3)
            F1.Calculate(gfx, Canvas);
            F2.Calculate(gfx, Canvas);
            F3.Calculate(gfx, Canvas);

            XUnit h1 = F1.MarginTop + F1.H + F1.MarginBottom;
            XUnit h2 = F2.MarginTop + F2.H + F2.MarginBottom;
            XUnit h3 = F3.MarginTop + F3.H + F3.MarginBottom;
            XUnit footerHeight = XUnit.FromPoint(Math.Max(F1.H.Point, Math.Max(F2.H.Point, F3.H.Point)));
            F1.X = XUnit.Zero;
            F2.X = (Frame.W - F2.W) / 2; 
            F3.X = Frame.W - F3.W;
            F1.Y = F2.Y = F3.Y = footerHeight > XUnit.Zero
                ? Canvas.Y + VSpace
                : XUnit.Zero;
            //F2.Y = footerHeight > XUnit.Zero
            //    ? Canvas.Y + VSpace
            //    : XUnit.Zero;
            //F3.Y = footerHeight > XUnit.Zero
            //    ? Canvas.Y + VSpace
            //    : XUnit.Zero;

            this.Canvas.Y += footerHeight > XUnit.Zero
                ? VSpace + footerHeight
                : XUnit.Zero;



            this.H = Canvas.Y + this.PaddingBottom;
        }

        internal override void Draw(XGraphics gfx)
        {
            LogDraw();

            // Frame first so the content is drawn on top of it.
            DrawElement(gfx, Title);
            DrawElement(gfx, Frame);
            DrawElement(gfx, Image);
            DrawElement(gfx, I1);
            DrawElement(gfx, I2);
            DrawElement(gfx, I3);
            DrawElement(gfx, F1);
            DrawElement(gfx, F2);
            DrawElement(gfx, F3);
        }
        #endregion
    }
}
