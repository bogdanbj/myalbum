using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfSharp.Drawing;

namespace MyAlbum
{
    public static class Alignments
    {
        public static XStringFormat Default
        {
            get
            {
                XStringFormat format = new XStringFormat();
                format.LineAlignment = XLineAlignment.BaseLine;
                return format;
            }
        }
        public static XStringFormat TopLeft
        {
            get
            {
                XStringFormat format = new XStringFormat();
                format.Alignment = XStringAlignment.Near;
                format.LineAlignment = XLineAlignment.Near;
                return format;
            }
        }
        public static XStringFormat TopCenter
        {
            get
            {
                XStringFormat format = new XStringFormat();
                format.Alignment = XStringAlignment.Center;
                format.LineAlignment = XLineAlignment.Near;
                return format;
            }
        }
        public static XStringFormat TopRight
        {
            get
            {
                XStringFormat format = new XStringFormat();
                format.Alignment = XStringAlignment.Far;
                format.LineAlignment = XLineAlignment.Near;
                return format;
            }
        }
        public static XStringFormat CenterLeft
        {
            get
            {
                XStringFormat format = new XStringFormat();
                format.Alignment = XStringAlignment.Near;
                format.LineAlignment = XLineAlignment.Center;
                return format;
            }
        }
        public static XStringFormat CenterCenter
        {
            get
            {
                XStringFormat format = new XStringFormat();
                format.Alignment = XStringAlignment.Center;
                format.LineAlignment = XLineAlignment.Center;
                return format;
            }
        }
        public static XStringFormat CenterRight
        {
            get
            {
                XStringFormat format = new XStringFormat();
                format.Alignment = XStringAlignment.Far;
                format.LineAlignment = XLineAlignment.Center;
                return format;
            }
        }
        public static XStringFormat BottomLeft
        {
            get
            {
                XStringFormat format = new XStringFormat();
                format.Alignment = XStringAlignment.Near;
                format.LineAlignment = XLineAlignment.Far;
                return format;
            }
        }
        public static XStringFormat BottomCenter
        {
            get
            {
                XStringFormat format = new XStringFormat();
                format.Alignment = XStringAlignment.Center;
                format.LineAlignment = XLineAlignment.Far;
                return format;
            }
        }
        public static XStringFormat BottomRight
        {
            get
            {
                XStringFormat format = new XStringFormat();
                format.Alignment = XStringAlignment.Far;
                format.LineAlignment = XLineAlignment.Far;
                return format;
            }
        }
    }
}
