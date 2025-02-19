using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlbum
{
    internal class PageStyle : BaseElement
    {
        #region properties
        public bool IsDefault { get; set; }
        public new PdfSharp.PageOrientation Orientation { get; set; }
        #endregion

        #region public methods
        //private void Draw()
        //{
        //    throw new InvalidOperationException("Invalid operation: Cannot draw a style");
        //}

        public override void Parse()
        {
            // base attributes
            ParseBaseAttributes();

            // default attribute
            ParseDefaultAttribute();
            
            // orientation
            ParseOrientationAttribute();
        }
        #endregion

        #region private methods
        private void ParseDefaultAttribute()
        {
            if (Xml.Attribute("default")?.Value.ToLower() == "true")
                this.IsDefault = true;
            else
                this.IsDefault = false;
        }
        private void ParseOrientationAttribute()
        {
            switch (Xml.Attribute("orientation")?.Value.ToLower())
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
        #endregion
    }
}