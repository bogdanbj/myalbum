using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace HelloWorld
{
    internal class BaseElement
    {
        #region fields
        public XGraphics? gfx;
        public XUnitPt x;
        public XUnitPt y;
        public XUnitPt w;
        public XUnitPt h;
        #endregion

        #region constructors
        public BaseElement()
        {
        }
        public BaseElement(XGraphics gfx) : this() 
        {
            this.gfx = gfx;
        }
        #endregion
    }
}
