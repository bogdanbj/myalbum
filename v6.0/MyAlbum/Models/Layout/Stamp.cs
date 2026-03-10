using MyAlbum.Models.Xml;
using MyAlbum.Models.Xml.Styles;
using MyAlbum.Services;
using MyAlbum.Utils;
using PdfSharpCore.Drawing;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAlbum.Models.Layout
{
    internal class Stamp : BaseElement
    {
        public XUnit Width { get; set; }
        public XUnit Height { get; set; }
        public Border Frame { get; set; }
        public Text Title { get; set; }
        public Image Image { get; set; }
        public Text I1 { get; set; }
        public Text I2 { get; set; }
        public Text I3 { get; set; }
        public Text F1 { get; set; }
        public Text F2 { get; set; }
        public Text F3 { get; set; }

        public Stamp()
        {
            Frame = new Border();
            Title = new Text();
            Image = new Image();
            I1 = new Text();
            I2 = new Text();
            I3 = new Text();    
            F1 = new Text();
            F2 = new Text();
            F3 = new Text();


        }

        internal void FromXml(XmlStamp xmlStamp, AlbumStyles styles)
        {

            // parse the style. The style has inner elements. parse each one's style
            StampStyle? style = StyleFactory.FindStyle<StampStyle>(xmlStamp.Style, styles.StampStyles);
            if (style == null)
            {
                throw new InvalidOperationException(
                    $"Stamp style '{xmlStamp.Style ?? "(default)"}' not found. " +
                    $"Ensure a matching StampStyle exists in the album styles or that a default StampStyle is defined.");
            }
            base.FromXml(xmlStamp, style);
            VSpace = ParseXUnit(xmlStamp.VSpace ?? style.VSpace ?? $"{VSpace.Millimeter} mm");

            Width = XUnit.FromMillimeter(xmlStamp.Width);
            Height = XUnit.FromMillimeter(xmlStamp.Height);

            Frame.Inherit(this);
            Title.Inherit(this);
            //TODO: image
            I1.Inherit(this);
            I2.Inherit(this);
            I3.Inherit(this);
            F1.Inherit(this);
            F2.Inherit(this);
            F3.Inherit(this);

            Frame.FromXml(style.BorderStyle ?? new XmlBorder(), styles);
            Title.FromXml(style.TitleStyle ?? new XmlText(), styles);
            // TODO: image
            I1.FromXml(style.Inside1Style ?? new XmlText(), styles);
            I2.FromXml(style.Inside2Style ?? new XmlText(), styles);
            I3.FromXml(style.Inside3Style ?? new XmlText(), styles);
            F1.FromXml(style.Footer1Style ?? new XmlText(), styles);
            F2.FromXml(style.Footer2Style ?? new XmlText(), styles);
            F3.FromXml(style.Footer3Style ?? new XmlText(), styles);

            //Title.Value = xmlStamp.Title;
            //// TODO image
            //I1.Value = xmlStamp.I1;
            //I2.Value = xmlStamp.I2;
            //I3.Value = xmlStamp.I3;
            //F1.Value = xmlStamp.F1;
            //F2.Value = xmlStamp.F2;
            //F3.Value = xmlStamp.F3;

            if (xmlStamp.Title != null) { Title.Value = xmlStamp.Title; }
            //TODO: image
            if (xmlStamp.I1 != null) { I1.Value = xmlStamp.I1; }
            if (xmlStamp.I2 != null) { I2.Value = xmlStamp.I2; }
            if (xmlStamp.I3 != null) { I3.Value = xmlStamp.I3; }
            if (xmlStamp.F1 != null) { F1.Value = xmlStamp.F1; }
            if (xmlStamp.F2 != null) { F2.Value = xmlStamp.F2; }
            if (xmlStamp.F3 != null) { F3.Value = xmlStamp.F3; }
        }
        internal override void CalculateSize(XGraphics gfx, XUnit w, XUnit h)
        {
            // start calculating the frame because the title and footer depend on frame width

            #region frame
            Frame.W = Width; Frame.H = Height;
            Frame.CalculateBorderWidths();
            // Left
            if (Frame.TypeLeft != BorderType.None)
                Frame.W += Frame.MarginLeft + Frame.WidthLeft + Frame.PaddingLeft;
            // Right
            if (Frame.TypeRight != BorderType.None)
                Frame.W += Frame.MarginRight + Frame.WidthRight + Frame.PaddingRight;
            // Top
            if (Frame.TypeTop != BorderType.None)
                Frame.H += Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop;
            // Bottom
            if (Frame.TypeBottom != BorderType.None)
                Frame.H += Frame.MarginBottom + Frame.WidthBottom + Frame.PaddingBottom;
            W = Frame.W; H = Frame.H;
            #endregion

            #region title
            Title.CalculateSize(gfx, Frame.W, XUnit.Zero);
            if (Title.H > XUnit.Zero) 
                H += Title.H + VSpace; 
            #endregion

            #region inside
            I1.CalculateSize(gfx, 0.9 * Width, XUnit.Zero);
            I2.CalculateSize(gfx, 0.9 * Width, XUnit.Zero);
            I3.CalculateSize(gfx, 0.9 * Width, XUnit.Zero);
            #endregion

            #region footer
            F1.W = F1.Align switch
            {
                Alignment.Left => Frame.W / 2 - F1.MarginLeft,
                Alignment.Center => Frame.W - (F1.MarginLeft + F1.MarginRight),
                Alignment.Right => Frame.W / 2 - F1.MarginRight,
                _ => Frame.W - (F1.MarginLeft + F1.MarginRight)
            };
            F2.W = F2.Align switch
            {
                Alignment.Left => Frame.W / 2 - F2.MarginLeft,
                Alignment.Center => Frame.W - (F2.MarginLeft + F2.MarginRight),
                Alignment.Right => Frame.W / 2 - F2.MarginRight,
                _ => Frame.W - (F2.MarginLeft + F2.MarginRight)
            };
            F3.W = F3.Align switch
            {
                Alignment.Left => Frame.W / 2 - F3.MarginLeft,
                Alignment.Center => Frame.W - (F3.MarginLeft + F3.MarginRight),
                Alignment.Right => Frame.W / 2 - F3.MarginRight,
                _ => Frame.W - (F3.MarginLeft + F3.MarginRight)
            };

            // footer width is already set so it won't be changed by CalculateSize
            F1.CalculateSize(gfx, XUnit.Zero, XUnit.Zero);
            F2.CalculateSize(gfx, XUnit.Zero, XUnit.Zero);
            F3.CalculateSize(gfx, XUnit.Zero, XUnit.Zero);
            XUnit footerHeight = Math.Max(F1.H, Math.Max(F2.H, F3.H));
            if (footerHeight > XUnit.Zero)
                H += footerHeight + VSpace;
            #endregion
            
            #region alignments
            TopAlign = (string.IsNullOrEmpty(Title.Value) ? XUnit.Zero : Title.H + VSpace);
            MiddleAlign = (string.IsNullOrEmpty(Title.Value) ? XUnit.Zero : Title.H + VSpace) + Frame.H / 2;
            BottomAlign = (string.IsNullOrEmpty(Title.Value) ? XUnit.Zero : Title.H + VSpace) + Frame.H;
            #endregion

        }
        internal override void CalculateInnerPositions()
        {
            #region title
            // title is always at the top and centered
            Title.X = X + (W - Title.W) / 2;
            Title.Y = Y;
            #endregion

            #region frame
            // frame is under the title and starts from the left edge of the stamp
            Frame.X = X;
            Frame.Y = Y + (Title.H > XUnit.Zero ? Title.H + VSpace : XUnit.Zero);
            #endregion

            #region inside
            // inside is always centered vertically in the frame, and its horizontal position depends on its alignment
            I1.X = I1.Align switch
            {
                Alignment.Left => X,
                Alignment.Center => X + (W - I1.W) / 2,
                Alignment.Right => X + W - I1.W,
                _ => X
            };
            I2.X = I2.Align switch
            {
                Alignment.Left => X,
                Alignment.Center => X + (W - I2.W) / 2,
                Alignment.Right => X + W - I2.W,
                _ => X
            };
            I3.X = I3.Align switch
            {
                Alignment.Left => X,
                Alignment.Center => X + (W - I3.W) / 2,
                Alignment.Right => X + W - I3.W,
                _ => X
            };

            //inside height
            XUnit h = XUnit.Zero;
            //h = (string.IsNullOrEmpty(I1.Value) ? XUnit.Zero : (I1.H + VSpace))
            //    + (string.IsNullOrEmpty(I2.Value) ? XUnit.Zero : (I2.H + VSpace))
            //    + (string.IsNullOrEmpty(I3.Value) ? XUnit.Zero : (I3.H + VSpace));
            h = (string.IsNullOrEmpty(I1.Value) ? XUnit.Zero : (I1.H))
                + (string.IsNullOrEmpty(I2.Value) ? XUnit.Zero : (I2.H))
                + (string.IsNullOrEmpty(I3.Value) ? XUnit.Zero : (I3.H));

            //if (h > XUnit.Zero)
            //    h -= VSpace; // remove the last VSpace

            XUnit yPos = Frame.Y + (Frame.H - h) / 2;
            if (!string.IsNullOrEmpty(I1.Value))
            {
                I1.Y = yPos;
                //yPos += I1.H + VSpace;
                yPos += I1.H;
            }
            if (!string.IsNullOrEmpty(I2.Value))
            {
                I2.Y = yPos;
                //yPos += I2.H + VSpace;
                yPos += I2.H;
            }
            if (!string.IsNullOrEmpty(I3.Value))
            {
                I3.Y = yPos;
            }
            #endregion

            #region footer
            // footer is always at the bottom 
            F1.X = F1.Align switch
            {
                Alignment.Left => X,
                Alignment.Center => X + (W - F1.W) / 2,
                Alignment.Right => X + W - F1.W,
                _ => X
            };
            F2.X = F2.Align switch
            {
                Alignment.Left => X,
                Alignment.Center => X + (W - F2.W) / 2,
                Alignment.Right => X + W - F2.W,
                _ => X
            };
            F3.X = F3.Align switch
            {
                Alignment.Left => X,
                Alignment.Center => X + (W - F3.W) / 2,
                Alignment.Right => X + W - F3.W,
                _ => X
            };
            F1.Y = F2.Y = F3.Y = Frame.Y + Frame.H + VSpace;
            #endregion

        }
        internal override void Draw(XGraphics gfx)
        {
            // TEST : fill Image
            if ((Name ?? "").Contains("test", StringComparison.OrdinalIgnoreCase))
                Helper.Fill(gfx, this, XColors.DeepSkyBlue);

            // assume the stamp canvas is the inside of the frame
            Canvas.W = Width;
            Canvas.H = Height;
            Canvas.H = Height; Canvas.X = Frame.X + Frame.MarginLeft + Frame.WidthLeft + Frame.PaddingLeft;
            Canvas.Y = Frame.Y + Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop;
            //Helper.FillCanvas(gfx, this);

            //Helper.WriteMe(gfx, this);


            Frame.Draw(gfx);
            Title.Draw(gfx);
            // TODO: Image.Draw(gfx);
            //I1.Value = this.Y.Millimeter.ToString("F2") + " mm";
            //I1.CalculateSize(gfx, 0.9 * Width, XUnit.Zero);
            I1.Draw(gfx);
            I2.Draw(gfx);
            I3.Draw(gfx);
            F1.Draw(gfx);
            F2.Draw(gfx);
            F3.Draw(gfx);
        }
    }
}
