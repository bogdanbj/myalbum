using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using PdfSharp.Drawing;

namespace MyAlbum3
{
    class Image : BaseElement
    {
        #region Properties
        public string FileName { get; set; }
        //public Styles.Alignments Align { get; set; }
        public bool Stretched { get; set; }
        public double Angle { get; set; }
        public new bool Color { get; set; }
        public XImage XImg { get; set; }

        private XPoint Anchor;
        #endregion

        #region Constructors
        public Image()
        {
        }
        public Image(XElement xml)
        {
            Xml = xml;
        }
        #endregion

        #region Public Methods
        public override void Parse()
        {
            //if (Parent != null) { Inherit(); }
            Image style = FindStyle();
            ApplyStyle(style);
            ParseAttributes();
        }
        public override void Calculate()
        {
            TopAlign = XUnit.Zero;
            MiddleAlign = h / 2;
            BottomAlign = h;
        }
        public void Load()
        {
            if (XImage.ExistsFile(this.FileName))
            {
                XImg = XImage.FromFile(FileName);
            }
        }
        public void Rotate()
        {
            Console.WriteLine("Image.Rotate()");
        }
        public override void Draw()
        {
            //Console.WriteLine("Image.Draw()");

            if (this.XImg == null)
            {
                this.Load();
            }

            if (XImg != null)
            {
                switch (Align)
                {
                    case Styles.Alignments.Left:
                        switch (VAlign)
                        {
                            case Styles.VerticalAlignments.NotSet:
                                break;
                            case Styles.VerticalAlignments.Top:
                                Anchor.X = x; 
                                Anchor.Y = y;
                                //gfx.DrawImage(XImg, x, y, w, h);
                                break;
                            case Styles.VerticalAlignments.Center:
                                Anchor.X = x; 
                                Anchor.Y = y - h / 2;
                                //gfx.DrawImage(XImg, x, y - h / 2, w, h);
                                break;
                            case Styles.VerticalAlignments.Bottom:
                                Anchor.X = x; 
                                Anchor.Y = y - h;
                                //gfx.DrawImage(XImg, x, y - h, w, h);
                                break;
                            default:
                                break;
                        }
                        break;
                    case Styles.Alignments.Center:
                        switch (VAlign)
                        {
                            case Styles.VerticalAlignments.NotSet:
                                break;
                            case Styles.VerticalAlignments.Top:
                                Anchor.X = x - w / 2; 
                                Anchor.Y = y;
                                //gfx.DrawImage(XImg, x - w / 2, y, w, h);
                                break;
                            case Styles.VerticalAlignments.Center:
                                Anchor.X = x - w / 2;
                                Anchor.Y = y - h / 2;
                                //gfx.DrawImage(XImg, x - w / 2, y - h / 2, w, h);
                                break;
                            case Styles.VerticalAlignments.Bottom:
                                Anchor.X = x - w / 2;
                                Anchor.Y = y - h;
                                //gfx.DrawImage(XImg, x - w / 2, y - h, w, h);
                                break;
                            default:
                                break;
                        }
                        break;
                    case Styles.Alignments.Right:
                        switch (VAlign)
                        {
                            case Styles.VerticalAlignments.NotSet:
                                break;
                            case Styles.VerticalAlignments.Top:
                                Anchor.X = x - w; 
                                Anchor.Y = y;
                                //gfx.DrawImage(XImg, x - w, y, w, h);
                                break;
                            case Styles.VerticalAlignments.Center:
                                Anchor.X = x - w;
                                Anchor.Y = y - h / 2;
                                //gfx.DrawImage(XImg, x - w, y - h / 2, w, h);
                                break;
                            case Styles.VerticalAlignments.Bottom:
                                Anchor.X = x - w;
                                Anchor.Y = y - h;
                                //gfx.DrawImage(XImg, x - w, y - h, w, h);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }

                DrawBackground();

                if (this.Stretched)
                {
                    gfx.DrawImage(XImg, Anchor.X, Anchor.Y, w, h);
                }
                else
                {
                    gfx.DrawImage(XImg, Anchor.X, Anchor.Y);
                }

            }
            //this.DrawCross(new XPoint(x, y));
            //this.DrawBox(new XPoint(x - w / 2, y), new XSize(w, h));
        }
        #endregion

        #region Private Methods
        private Image FindStyle()
        {
            Image style = null;

            // use specific style
            if (Xml.Attribute("style") != null)
            {
                style = Styles.ImageStyles.Where(t => t.Style == Xml.Attribute("style").Value).FirstOrDefault();

            }

            // use default
            if (style == null)
            {
                style = Styles.ImageStyles.Where(t => t.IsDefault == true).FirstOrDefault();
            }

            return style;

        }
        public void ApplyStyle(Image styleImage)
        {
            // copy style properties
            if (styleImage != null)
            {
                Style = styleImage.Style;
                FileName = styleImage.FileName;
                Align = styleImage.Align;
                VAlign = styleImage.VAlign;
                Stretched = styleImage.Stretched;
                Angle = styleImage.Angle;
                MarginLeft = styleImage.MarginLeft;
                MarginRight = styleImage.MarginRight;
                MarginTop = styleImage.MarginTop;
                MarginBottom = styleImage.MarginBottom;
                Color = styleImage.Color;
                Absolute = styleImage.Absolute;
                x = styleImage.x;
                y = styleImage.y;
                w = styleImage.w;
                h = styleImage.h;
            }

        }
        private void Inherit()
        { 
        }
        private void ParseAttributes()
        {
            // base element attribute
            ParseBaseAttribute();

            // color attribute
            ParseColorAttribute();

            // file_name attribute
            ParseFileNameAttribute();

            // stretched attribute
            ParseStretchedAttribute();

            // angle attribute
            ParseAngleAttribute();
        }
        private void ParseFileNameAttribute()
        {
            if (Xml.Attribute("file_name") != null)
            {
                this.FileName = Xml.Attribute("file_name").Value;
            }
        }
        private void ParseStretchedAttribute()
        {
            if (Xml.Attribute("stretched") != null)
            {
                if (Xml.Attribute("stretched").Value.ToLower() == "true")
                {
                    this.Stretched = true;
                }
            }

        }
        private void ParseAngleAttribute()
        {
            if (Xml.Attribute("angle") != null)
            {
                try
                {
                    this.Angle = double.Parse(Xml.Attribute("angle").Value);
                }
                catch (Exception)
                {
                    this.Angle = 0;
                }
            }
        }
        #endregion
    }
}
