using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using PdfSharp.Drawing;


namespace MyAlbum
{
    abstract class Container: BaseElement
    {
        private List<BaseElement> _elements;
        public List<BaseElement> Elements
        {
            get
            {
                if (_elements == null) { _elements = new List<BaseElement>(); }
                return _elements;
            }
            set { _elements = value; }
        }

        public XUnit VSpace { get; set; }
        public XUnit HSpace { get; set; }
    }
}
