using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAlbum
{
    internal abstract class BaseElement
    {
        #region properties
        //public bool IsDefault { get; set; }
        public string? Style { get; set; }

        private XElement? _xml;

        public XElement Xml
        {
            get { return _xml ?? new XElement ("none"); }
            set { _xml = value; }
        }

        public XRect Canvas { get; set; }
        public XUnitPt X { get; set; }
        public XUnitPt Y { get; set; }
        public XUnitPt W { get; set; }
        public XUnitPt H { get; set; }

        #endregion

        #region constructors
        public BaseElement()
        {
            Xml = new XElement("none");
            X = XUnitPt.Zero;
            Y = XUnitPt.Zero;
            W = XUnitPt.Zero;
            H = XUnitPt.Zero;
            
        }
        //public BaseElement(XGraphics gfx) : this()
        //{
        //    Gfx = gfx;
        //}
        #endregion

        #region public abstract methods
        public abstract void Parse();
        //public abstract void Draw();
        #endregion

        #region public methods
        public void ParseBaseAttributes()
        {
            // style attribute
            ParseStyleAttribute();
        }
        #endregion

        #region private methods
        private void ParseStyleAttribute()
        {
            Style = Xml.Attribute("style")?.Value.ToLower(); // null if "style" attribue does not exists
        }
        #endregion
    }
}
