using MyAlbum.Utilities;
using PdfSharpCore.Drawing;
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
                        Image.FileName = xElem.Attribute("image")?.Value;
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

            // Calculate Title
            Title.Calculate(gfx, Canvas);
            this.Canvas.H = Title.H > XUnit.Zero
                ? Title.H + Title.MarginTop + Title.MarginBottom + VSpace
                : XUnit.Zero;

            // Calculate Frame
            Frame.CalculateFrameWidths();
            Frame.W = W + Frame.WidthLeft + Frame.PaddingLeft + Frame.PaddingRight + Frame.WidthRight;
            Frame.H = H + Frame.WidthTop + Frame.PaddingTop + Frame.WidthTop + Frame.PaddingBottom + Frame.WidthBottom;
            this.Canvas.H += Frame.H;
            this.Canvas.W = Frame.W;

            // Calculate Image
            Image.Calculate(gfx, Canvas);
            Image.W = this.W - Image.MarginLeft - Image.MarginRight;
            Image.H = this.H - Image.MarginTop - Image.MarginBottom;

            // Calculate Insiders (I1, I2, I3)
            I1.Calculate(gfx, Canvas);
            I2.Calculate(gfx, Canvas);
            I3.Calculate(gfx, Canvas);

            // Calculate Footers (F1, F2, F3)
            F1.Calculate(gfx, Canvas);
            F2.Calculate(gfx, Canvas);
            F3.Calculate(gfx, Canvas);
            XUnit footerHeight = Math.Max(F1.H, Math.Max(F2.H, F3.H));
            this.Canvas.H += footerHeight > XUnit.Zero
                ? VSpace + footerHeight
                : XUnit.Zero;
        }

        internal override void Draw(XGraphics gfx)
        {
            LogDraw();

            // Frame first so the content is drawn on top of it.
            Frame.Draw(gfx);
            Image.Draw(gfx);
            Title.Draw(gfx);
            I1.Draw(gfx);
            I2.Draw(gfx);
            I3.Draw(gfx);
            F1.Draw(gfx);
            F2.Draw(gfx);
            F3.Draw(gfx);
        }
        #endregion
    }
}
