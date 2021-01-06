using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfSharp.Drawing;

namespace MyAlbum2
{
/*    abstract class Entity : IDrawable
    {
        #region Fields
        protected XGraphics gfx;
        protected bool needsProcess;
        #endregion

        #region Properties

        // constituents
        public virtual Entity Title { get; set; }
        public Entity Body { get; set; }
        public List<Entity> Footnotes { get; set; }

        //dimensions
        public XUnit X { get; set; }
        public XUnit Y { get; set; }
        public virtual XUnit H { get; set; }
        public virtual XUnit W { get; set; }
        public XUnit VSpace { get; set; }
        public Alignments Alignment { get; set; }
        public bool CanGrow { get; set; }
        public bool HasBorder { get; set; }
        #endregion

        #region Constructors
        protected Entity(XGraphics gfx)
        {
            this.gfx = gfx;
            CanGrow = true;
            HasBorder = false;
            needsProcess = true;
        }
        protected Entity(XGraphics gfx, double x, double y, double w, double h)
            : this(gfx)
        {
            Title = null;
            Footnotes = new List<Entity>();
            X = XUnit.FromMillimeter(x);
            Y = XUnit.FromMillimeter(y);
            W = XUnit.FromMillimeter(w);
            H = XUnit.FromMillimeter(h);
        }
        protected Entity(XGraphics gfx, double x, double y, double w, double h, Alignments align, bool canGrow)
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
            gfx.DrawLine(XPens.Red, point.X - 10, point.Y, point.X + 10, point.Y);
            gfx.DrawLine(XPens.Red, point.X, point.Y - 10, point.X, point.Y + 10);
        }
        #endregion
    }
*/
}
