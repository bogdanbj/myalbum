using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfSharp.Drawing;

namespace MyAlbum2
{
    public abstract class BasicType : IDrawable
    {
        #region Fields
        protected XGraphics gfx;
        protected bool needsProcess;
        static XPen UsePen = XPens.Red;
        #endregion

        #region Properties
       
        public virtual XUnit X { get; set; }
        public virtual XUnit Y { get; set; }
        public virtual XUnit H { get; set; }
        public virtual XUnit W { get; set; }
        public virtual XUnit Padding {get; set; }
        public Alignments Alignment { get; set; }
        public bool CanGrow { get; set; }
        public bool HasBorder { get; set; }
        #endregion

        #region Constructors
        protected BasicType(XGraphics gfx)
        {
            this.gfx = gfx;
            CanGrow = true;
            HasBorder = false;
            needsProcess = true;
        }
        protected BasicType(XGraphics gfx, double x, double y, double w, double h)
            : this(gfx)
        {
            X = XUnit.FromMillimeter(x);
            Y = XUnit.FromMillimeter(y);
            W = XUnit.FromMillimeter(w);
            H = XUnit.FromMillimeter(h);
        }
        protected BasicType(XGraphics gfx, double x, double y, double w, double h, Alignments align, bool canGrow)
            : this(gfx, x, y, w, h)
        {
            Alignment = align;
            CanGrow = canGrow;
        }
        #endregion

        #region Methods
        public abstract void Draw();
        public static void DrawCross(XGraphics gfx, XPoint point)
        {
            gfx.DrawLine(UsePen, point.X - 10, point.Y, point.X + 10, point.Y);
            gfx.DrawLine(UsePen, point.X, point.Y - 10, point.X, point.Y + 10);
        }
        public static void DrawCross(XGraphics gfx, XPoint point, XPen pen)
        {
            gfx.DrawLine(pen, point.X - 10, point.Y, point.X + 10, point.Y);
            gfx.DrawLine(pen, point.X, point.Y - 10, point.X, point.Y + 10);
        }
        #endregion
    }

    public abstract class AlbumElement : BasicType
    {
        #region Properties
        // constituents
        public virtual Text Title { get; set; }
        public virtual BasicType Body { get; set; }
        private List<Text> footNotes;
        public virtual  List<Text> FootNotes 
        { 
            get
            {
                if (footNotes == null) footNotes = new List<Text>();
                return footNotes;
            } 
            set { footNotes = value; } 
        }
        public XUnit VSpace { get; set; }
        #endregion

        #region Constructors
        protected AlbumElement(XGraphics gfx) : base(gfx)
        {
            this.gfx = gfx;
            CanGrow = true;
            HasBorder = false;
            needsProcess = true;
        }
        protected AlbumElement(XGraphics gfx, double x, double y, double w, double h)
            : this(gfx)
        {
            X = XUnit.FromMillimeter(x);
            Y = XUnit.FromMillimeter(y);
            W = XUnit.FromMillimeter(w);
            H = XUnit.FromMillimeter(h);
        }
        protected AlbumElement(XGraphics gfx, double x, double y, double w, double h, Alignments align, bool canGrow)
            : this(gfx, x, y, w, h)
        {
            Alignment = align;
            CanGrow = canGrow;
        }
        #endregion
    }

    public static class FootntesExtensions
    {
        public static XUnit TextHeight(this List<Text> Items)
        {
            XUnit max = XUnit.Zero;

            foreach (Text item in Items)
            {
                XUnit h = item.TextHeight;
                max = max > h ? max : h;
            }

            return max;
        }
    }

}