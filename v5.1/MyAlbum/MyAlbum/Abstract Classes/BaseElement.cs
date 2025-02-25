using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAlbum
{
    internal class BaseElement
    {
        #region properties
        public XElement xml { get; set; }
        #endregion  

        protected BaseElement()
        {
            xml = new XElement("none");
        }

        public virtual void Parse()
        {
            throw new NotImplementedException();
        }

    }
}
