using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using PdfSharpCore.Drawing;

namespace MyAlbum
{
    class Stamp : Container
    {

        #region Properties
        private Text _title;
        private Text _i1;
        private Text _i2;
        private Text _i3;
        private Text _f1;
        private Text _f2;
        private Text _f3;
        private Border _frame;
        private Image _img;

        public Text Title
        {
            get { if (_title == null) _title = new Text(); return _title; }
            set { _title = value; }
        }
        public Text Inside1
        {
            get { if (_i1 == null) _i1 = new Text(); return _i1; }
            set { _i1 = value; }
        }
        public Text Inside2
        {
            get { if (_i2 == null) _i2 = new Text(); return _i2; }
            set { _i2 = value; }
        }
        public Text Inside3
        {
            get { if (_i3 == null) _i3 = new Text(); return _i3; }
            set { _i3 = value; }
        }
        public Text Footer1
        {
            get { if (_f1 == null) _f1 = new Text(); return _f1; }
            set { _f1 = value; }
        }
        public Text Footer2
        {
            get { if (_f2 == null) _f2 = new Text(); return _f2; }
            set { _f2 = value; }
        }
        public Text Footer3
        {
            get { if (_f3 == null) _f3 = new Text(); return _f3; }
            set { _f3 = value; }
        }
        public Border Frame
        {
            get { if (_frame == null) _frame = new Border(); return _frame; }
            set { _frame = value; }
        }
        public Image Img
        {
            get { if (_img == null) _img = new Image(); return _img; }
            set { _img = value; }
        }

        private XUnit yPos;
        private XUnit stampHeight;
        private XUnit stampWidth;
        #endregion

        #region Constructors
        public Stamp()
        {
            this.VSpace = XUnit.FromMillimeter(1);
            this.BoxColor = XColors.DeepPink;
            Text t = new Text();
            Title=t;
           
        }
        public Stamp(XElement xml) : this()
        {
            Xml = xml;
        }
        #endregion

        #region Public Methods
        public override void Parse()
        {
            if (Parent != null) Inherit();
            ApplyStyle();
            ParseAttributes();
            ParseComponents();
        }
        public override void Calculate()
        {
            Frame.w = this.stampWidth;
            if (Frame.TypeLeft != "none") { Frame.w += Frame.PaddingLeft; }
            if (Frame.TypeRight != "none") { Frame.w += Frame.PaddingRight; }
            Frame.h = this.stampHeight;
            if (Frame.TypeTop != "none") { Frame.h += Frame.PaddingTop; }
            if (Frame.TypeBottom != "none") { Frame.h += Frame.PaddingBottom; }
            Frame.gfx = gfx;
            Frame.Calculate();

            w = Frame.w;


            Title.w = Frame.w - (Title.MarginLeft+Title.MarginRight);
            Title.Align = Styles.Alignments.Center;
            Title.VAlign = Styles.VerticalAlignments.Bottom;
            Title.gfx = gfx;
            Title.Calculate();

            Inside1.gfx = gfx;
            Inside2.gfx = gfx;
            Inside3.gfx = gfx;

            Img.w = this.stampWidth - (Img.MarginLeft + Img.MarginRight);
            Img.h = this.stampHeight - (Img.MarginTop + Img.MarginBottom);
            Img.Align = Styles.Alignments.Center;
            Img.VAlign = Styles.VerticalAlignments.Center;
            Img.gfx = gfx;


            switch (Footer1.Align)
            {
                case Styles.Alignments.Left:
                    Footer1.w = Frame.w / 2 - Footer1.MarginLeft;
                    break;
                case Styles.Alignments.Center:
                    Footer1.w = Frame.w - (Footer1.MarginLeft + Footer1.MarginRight);
                    break;
                case Styles.Alignments.Right:
                    Footer1.w = Frame.w / 2 - Footer1.MarginRight;
                    break;
                default:
                    break;
            }
            Footer1.gfx = gfx;
            Footer1.Calculate();

            switch (Footer2.Align)
            {
                case Styles.Alignments.Left:
                    Footer2.w = Frame.w / 2 - Footer2.MarginLeft;
                    break;
                case Styles.Alignments.Center:
                    Footer2.w = Frame.w - (Footer2.MarginLeft + Footer2.MarginRight);
                    break;
                case Styles.Alignments.Right:
                    Footer2.w = Frame.w / 2 - Footer2.MarginRight;
                    break;
                default:
                    break;
            }
            Footer2.gfx = gfx;
            Footer2.Calculate();

            switch (Footer3.Align)
            {
                case Styles.Alignments.Left:
                    Footer3.w = Frame.w / 2 - Footer3.MarginLeft;
                    break;
                case Styles.Alignments.Center:
                    Footer3.w = Frame.w - (Footer3.MarginLeft + Footer3.MarginRight);
                    break;
                case Styles.Alignments.Right:
                    Footer3.w = Frame.w / 2 - Footer3.MarginRight;
                    break;
                default:
                    break;
            }
            Footer3.gfx = gfx;
            Footer3.Calculate();

            this.w = Frame.w;
            XUnit footerHeight = Math.Max(Footer1.h, Math.Max(Footer2.h, Footer3.h));
            this.h = (string.IsNullOrEmpty(Title.Value) ? XUnit.Zero : (XUnit)(Title.h + this.VSpace)) + Frame.h + /*2 * Frame.Height +*/ (footerHeight.Equals(XUnit.Zero) ? XUnit.Zero : (XUnit)(this.VSpace + footerHeight));

            TopAlign = MarginTop + (string.IsNullOrEmpty(Title.Value) ? XUnit.Zero : (XUnit)(Title.h + this.VSpace)); // +Frame.Height;
            MiddleAlign = MarginTop + (string.IsNullOrEmpty(Title.Value) ? XUnit.Zero : (XUnit)(Title.h + this.VSpace)) /*+ Frame.Height*/ + Frame.h / 2;
            BottomAlign = MarginTop + (string.IsNullOrEmpty(Title.Value) ? XUnit.Zero : (XUnit)(Title.h + this.VSpace)) /*+ Frame.Height*/ + Frame.h; // +Frame.Height;
        }
        public override void Draw()
        {
            yPos = this.y + MarginTop;

            DrawBackground();

            #region Draw Title
            if (!string.IsNullOrEmpty(Title.Value))
            {
                Title.x = this.x;
                Title.y = yPos;
                Title.Draw();

                yPos += Title.h + VSpace;// +Frame.Height;
            }
            #endregion

            #region Draw Frame
            Frame.x = this.x;
            Frame.y = yPos;
            Frame.Draw();

            //gfx.DrawRectangle(new XPen(XColors.Red, 0.1),
            //                  Frame.x + Frame.w / 2 - stampWidth / 2,
            //                  Frame.y + Frame.h / 2 - stampHeight / 2, 
            //                  stampWidth, 
            //                  stampHeight
            //                  );


            yPos += Frame.h + VSpace;// +Frame.Height;
            #endregion

            #region Draw Inside

            if (!string.IsNullOrEmpty(Inside2.Value))
            {
                Inside1.Value += "\\n" + Inside2.Value;
            }

            if (!string.IsNullOrEmpty(Inside3.Value))
            {
                Inside1.Value += "\\n" + Inside3.Value;
            }
            Inside1.w = stampWidth * 0.9;
            Inside1.Calculate();
            Inside1.x = Frame.x + Frame.w / 2 - Inside1.w / 2;
            Inside1.y = Frame.y + Frame.h / 2 - Inside1.h / 2;
            Inside1.Draw();
            #endregion

            #region Draw Image
            if (XImage.ExistsFile(this.Img.FileName))
            {
                Img.x = Frame.x + Frame.w / 2;
                Img.y = Frame.y + Frame.h / 2;
                Img.Draw();
            }

            #endregion

            #region Draw Footer
            if (!string.IsNullOrEmpty(Footer1.Value))
            {
                Footer1.x = this.x;
                Footer1.y = yPos;
                Footer1.Draw();
            }

            if (!string.IsNullOrEmpty(Footer2.Value))
            {
                Footer2.x = this.x;
                Footer2.y = yPos;
                Footer2.Draw();
            }

            if (!string.IsNullOrEmpty(Footer3.Value))
            {
                Footer3.x = this.x + this.w/2;
                Footer3.y = yPos;
                Footer3.Draw();
            }
            #endregion

            //DrawCross(new XPoint(this.x, this.y+this.BottomAlign), XColors.Blue);
            //            gfx.DrawRectangle(XPens.Black, x.Point, yPos.Point, w.Point, h.Point);

        }
        #endregion

        #region Private Methods
        private void Inherit()
        {
            if (Parent != null)
            {
                Align = Parent.Align;
                VAlign = Parent.VAlign;
                Color = Parent.Color;
                Title.Color = this.Color;
                Frame.Color = this.Color;
                //Img.Color = this.Color;
                Inside1.Color = this.Color;
                Inside2.Color = this.Color;
                Inside3.Color = this.Color;
                Footer1.Color = this.Color;
                Footer2.Color = this.Color;
                Footer3.Color = this.Color;
            }
        }
        private void ApplyStyle()
        {
            Stamp styleStamp = null;

            // style
            if (Xml.Attribute("style") != null)
            {
                styleStamp = Styles.StampStyles.Where(x => x.Style == Xml.Attribute("style").Value).FirstOrDefault();
            }

            // use default
            if (styleStamp == null)
            {
                styleStamp = Styles.StampStyles.Where(x => x.IsDefault == true).FirstOrDefault();
            }

            // copy style properties
            if (styleStamp != null)
            {
                Style = styleStamp.Style;
                //Title = styleStamp.Title;

                Title.ApplyStyle(styleStamp.Title);
                Frame.ApplyStyle(styleStamp.Frame);
                //Frame = styleStamp.Frame;
                Img.ApplyStyle(styleStamp.Img);
                Inside1.ApplyStyle(styleStamp.Inside1);
                Inside2.ApplyStyle(styleStamp.Inside2);
                Inside3.ApplyStyle(styleStamp.Inside3);
                Footer1.ApplyStyle(styleStamp.Footer1);
                Footer2.ApplyStyle(styleStamp.Footer2);
                Footer3.ApplyStyle(styleStamp.Footer3);
                if (!styleStamp.Color.IsEmpty)
                {
                    Color = styleStamp.Color;
                }
                if (styleStamp.VSpace != XUnit.Zero)
                {
                    VSpace = styleStamp.VSpace;
                }
            }
        }
        private void ParseAttributes()
        {
            // base element attribute
            ParseBaseAttribute();

            // give it a bit of height so it can be noticed on the page
            if (h == XUnit.Zero) { h = XUnit.FromMillimeter(10); }

            this.stampHeight = this.h;
            this.stampWidth = this.w;
            this.w = XUnit.Zero;
            this.h = XUnit.Zero;

            //image attribute
            ParseVSpaceAttribute();

            //image attribute
            ParseImageAttribute();

            //title attribute
            ParseTitleAttribute();

            //inside1 attribute
            ParseInside1Attribute();

            //inside2 attribute
            ParseInside2Attribute();

            //inside3 attribute
            ParseInside3Attribute();

            //footer1 attribute
            ParseFooter1Attribute();

            //footer2 attribute
            ParseFooter2Attribute();

            //footer3 attribute
            ParseFooter3Attrribute();

        }
        private void ParseVSpaceAttribute()
        {
            //height attribute
            if (Xml.Attribute("vspace") != null)
            {
                try
                {
                    this.VSpace = XUnit.FromMillimeter(double.Parse(Xml.Attribute("vspace").Value));
                }
                catch (Exception)
                {
                }
            }
        }
        private void ParseImageAttribute()
        {
            if (Xml.Attribute("image") != null)
            {
                this.Img.FileName = Xml.Attribute("image").Value;
            }
        }
        private void ParseTitleAttribute()
        {
            if (Xml.Attribute("title") != null)
            {
                this.Title.Value = Xml.Attribute("title").Value;
            }
        }
        private void ParseInside1Attribute()
        {
            if (Xml.Attribute("i1") != null)
            {
                this.Inside1.Value = Xml.Attribute("i1").Value;
            }
        }
        private void ParseInside2Attribute()
        {
            if (Xml.Attribute("i2") != null)
            {
                this.Inside2.Value = Xml.Attribute("i2").Value;
            }
        }
        private void ParseInside3Attribute()
        {
            if (Xml.Attribute("i3") != null)
            {
                this.Inside3.Value = Xml.Attribute("i3").Value;
            }
        }
        private void ParseFooter1Attribute()
        {
            if (Xml.Attribute("f1") != null)
            {
                this.Footer1.Value = Xml.Attribute("f1").Value;
            }
        }
        private void ParseFooter2Attribute()
        {
            if (Xml.Attribute("f2") != null)
            {
                this.Footer2.Value = Xml.Attribute("f2").Value;
            }
        }
        private void ParseFooter3Attrribute()
        {
            if (Xml.Attribute("f3") != null)
            {
                this.Footer3.Value = Xml.Attribute("f3").Value;
            }
        }
        private void ParseComponents()
        {
            IEnumerable<XElement> elements = Xml.Elements();
            foreach (XElement xElem in elements)
            {
                switch (xElem.Name.LocalName)
                {
                    case "border":
                        Frame = new Border(xElem);
                        Frame.Parent = this;
                        Frame.Parse();
                        break;
                    case "image":
                        Img = new Image(xElem);
                        Img.Parent = this;
                        Img.Parse();
                        break;
                    case "title":
                        Title = new Text(xElem);
                        Title.Parent = this;
                        Title.Parse();
                        break;
                    case "inside1":
                        Inside1 = new Text(xElem);
                        Inside1.Parent = this;
                        Inside1.Parse();
                        break;
                    case "inside2":
                        Inside2 = new Text(xElem);
                        Inside2.Parent = this;
                        Inside2.Parse();
                        break;
                    case "inside3":
                        Inside3 = new Text(xElem);
                        Inside3.Parent = this;
                        Inside3.Parse();
                        break;
                    case "footer1":
                        Footer1 = new Text(xElem);
                        Footer1.Parent = this;
                        Footer1.Parse();
                        break;
                    case "footer2":
                        Footer2 = new Text(xElem);
                        Footer2.Parent = this;
                        Footer2.Parse();
                        break;
                    case "footer3":
                        Footer3 = new Text(xElem);
                        Footer3.Parent = this;
                        Footer3.Parse();
                        break;
                    default:
                        break;
                }
            }
        }
        private XUnit GetWidth()
        {
            return Frame.w;
        }
        private XUnit GetHeight()
        {
            return XUnit.Zero;
        }
        #endregion
    }
}
