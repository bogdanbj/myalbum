using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PdfSharpCore;

namespace MyAlbum.Models.Layout
{
    internal class LayoutPage : LayoutElement
    {
        private PageOrientation _pdfOrientation;
        public string Orientation
        {
            get
            {
                return PdfOrientation.ToString().ToLower();
            }
            set
            {
                if (value != null && Enum.TryParse(value, ignoreCase: true, out PageOrientation result))
                {
                    _pdfOrientation = result;
                }
            }
        }
        public PageOrientation PdfOrientation
        {
            get
            {
                return _pdfOrientation;
            }
            set
            {
                _pdfOrientation = value;
            }
        }


        public LayoutPage()
        {
            _pdfOrientation = PageOrientation.Portrait;
            X = 0;
            Y = 0;
        }
    }
}
