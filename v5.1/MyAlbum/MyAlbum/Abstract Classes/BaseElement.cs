using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAlbum
{
    abstract class BaseElement
    {
        #region properties
        public XElement xml { get; set; }
        #endregion  

        protected BaseElement()
        {
            xml = new XElement("none");
        }

        public abstract void Parse();

    }
}
