using PdfSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace MyAlbum
{
    internal class PageStyle : StyleElement
    {
        #region fields
        //private PageOrientation _orientation;
        //private Image? _banner;
        private Border? _frame;
        //private List<Row>? _rows;
        #endregion

        #region properties
        public PageOrientation Orientation { get; set; }
        public PageSize Size { get; set; }


        //public Image Banner
        //{
        //    get
        //    {
        //        if (_banner == null) { _banner = new Image(); }
        //        return _banner;
        //    }
        //    set { _banner = value; }
        //}
        public Border Frame
        {
            get
            {
                if (_frame == null) { _frame = new Border(); }
                return _frame;
            }
            set { _frame = value; }
        }
        //public List<Row> Rows
        //{
        //    get
        //    {
        //        if (_rows == null) { _rows = new List<Row>(); }
        //        return _rows;
        //    }
        //    set { _rows = value; }
        //}
        #endregion

        #region constructors
        public PageStyle()
        { }
        public PageStyle(XElement xml) : this()
        {
            this.xml = xml;
        }
        #endregion

        #region public methods
        public override void Parse()
        {
            ParseAttributes();
            ParseComponents();
        }
        #endregion

        #region private methods
        private void ParseAttributes()
        {
            // common style attributes
            ParseBaseAttributes();
            
            // orientation attribute
            ParseOrientationAttribute();
            
            // size attribute
            ParseSizeAttribute();
        }
        private void ParseComponents()
        {
            IEnumerable<XElement> elements = xml.Elements();
            foreach (XElement xElem in elements)
            {
                switch (xElem.Name.LocalName)
                {
                    //case "banner":
                    //    Banner = new Image(xElem);
                    //    Banner.Parent = this;
                    //    Banner.Parse();
                    //    break;
                    case "border":
                        Frame = new Border(xElem);
                        Frame.Parent = this;
                        Frame.Parse();
                        break;
                    //case "row":
                    //    Row newRow = new Row(xElem);
                    //    newRow.Parent = this;
                    //    newRow.Parse();
                    //    this.Rows.Add(newRow);
                    //    break;
                    default:
                        break;
                }
            }
        }
        private void ParseOrientationAttribute()
        {
            if (xml.Attribute("orientation") != null)
            {
                switch (xml.Attribute("orientation").Value.ToLower())
                {
                    case "portrait":
                        this.Orientation = PdfSharp.PageOrientation.Portrait;
                        break;
                    case "landscape":
                        this.Orientation = PdfSharp.PageOrientation.Landscape;
                        break;
                    default:
                        this.Orientation = PdfSharp.PageOrientation.Portrait;
                        break;
                }
            }
            //else
            //    this.Orientation = PdfSharp.PageOrientation.Portrait;
        }
        private void ParseSizeAttribute()
        {
            if (xml.Attribute("size") != null)
            {
                switch (xml.Attribute("size").Value.ToLower())
                {
                    case "letter":
                        this.Size = PdfSharp.PageSize.Letter;
                        break;
                    case "a4":
                        this.Size = PdfSharp.PageSize.A4;
                        break;
                    default:
                        this.Size = PdfSharp.PageSize.Letter;
                        break;
                }
            }
            else
                this.Size = PdfSharp.PageSize.Letter;
        }

        #endregion
    }

}
