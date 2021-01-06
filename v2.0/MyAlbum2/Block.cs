using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfSharp.Drawing;

namespace MyAlbum2
{
    class Block : AlbumElement
    {

//        public enum HAligns
//        {
//            FS, ES, JS
//        }
//        public enum VAligns
//        {
//            TOP, BOTTOM, CENTRE
//        }

        #region Properties
        public XUnit HSpace { get; set; }
        private List<AlbumElement> _items;
        public List<AlbumElement> Items
        {
            get 
            {
                if (_items == null) { _items = new List<AlbumElement>(); }
                return _items; 
            }
        }
        #endregion

        #region Constructors
        public Block(XGraphics gfx) : base(gfx)
        {}
        public Block(XGraphics gfx, double x, double y, double w, double h)
            : base(gfx, x, y, w, h)
        {}
        public Block(XGraphics gfx, double x, double y, double w, double h, MyAlbum2.Alignments align, double hSpace, bool canGrow)
            : base(gfx, x, y, w, h, align, canGrow)
        {
            HSpace = XUnit.FromMillimeter(hSpace);
        }
        #endregion

        #region Methods
        public void Add(AlbumElement item) 
        { 
            this.Items.Add(item); 
        }
        public void Clear()
        {
            this.Items.Clear();
        }
        public void Count()
        {
            this.Items.Count();
        }
        public void Insert(int index, AlbumElement item)
        {
            this.Items.Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            this.Items.RemoveAt(index);
        }
        
        public override void Draw()
        {

//            foreach (Entity item in Items)
//            {
//                string t;
//                t = item.GetType().Name;
//            }
            
            // Set X,Y for each item
            double currentX = 0;
            double itemsWidth = 0;  // XUnit.Point
            double maxHeight = 0;   // XUnit.Point
            double max = 0;         // XUnit.Point
            foreach (AlbumElement item in Items)
            {
                maxHeight = item.H.Point > maxHeight ? item.H.Point : maxHeight;
                itemsWidth += item.W.Point;
            }
            if (maxHeight > this.H.Point)
                if (CanGrow)
                    this.H = XUnit.FromPoint(maxHeight);

            // Determine the spacing between items, in case is auto-calculated
            switch (this.Alignment)
            {
                case Alignments.TopLeft:
                case Alignments.CenterLeft:
                case Alignments.BottomLeft:
                    currentX = this.X.Point;
                    break;
                case Alignments.TopCenter:
                case Alignments.CenterCenter:
                case Alignments.BottomCenter:
                    currentX = this.X.Point + (this.W.Point - itemsWidth - (Items.Count - 1) * HSpace.Point) / 2;
                    break;
                case Alignments.TopRight:
                case Alignments.CenterRight:
                case Alignments.BottomRight:
                    currentX = this.X.Point + (this.W.Point - itemsWidth - (Items.Count - 1) * HSpace.Point);
                    break;
                default:
                    break;
            }

            switch (this.Alignment)
            {
                case Alignments.TopLeft:
                case Alignments.TopCenter:
                case Alignments.TopRight:
                    foreach (AlbumElement item in Items)
                    {
                        double h = ((XUnit)((item.Title != null ? item.Title.TextHeight : XUnit.Zero) + item.VSpace)).Point;
                        max = (h > max ? h : max);
                    }
                    foreach (AlbumElement item in Items)
                    {
                        item.Y = this.Y + XUnit.FromPoint(max);
                    }
                    break;
                case Alignments.CenterLeft:
                case Alignments.CenterCenter:
                case Alignments.CenterRight:
                    foreach (AlbumElement item in Items)
                    {
                        double h = ((XUnit)((item.Title != null ? item.Title.TextHeight : XUnit.Zero) + item.VSpace + item.Body.H / 2)).Point;
                        max = (h > max ? h : max);
                    }
                    foreach (AlbumElement item in Items)
                    {
                        item.Y = this.Y + XUnit.FromPoint(max) - item.Body.H / 2;
                    }
                    break;
                case Alignments.BottomLeft:
                case Alignments.BottomCenter:
                case Alignments.BottomRight:
                    foreach (AlbumElement item in Items)
                    {
                        double h = ((XUnit)((item.Title != null ? item.Title.H : XUnit.Zero) + item.VSpace + item.Body.H)).Point;
                        max = (h > max ? h : max);
                    }
                    foreach (AlbumElement item in Items)
                    {
                        item.Y = this.Y + XUnit.FromPoint(max) - item.Body.H;
                    }
                    break;
                default:
                    break;
            }


            foreach (AlbumElement item in Items)
            {
                item.X = XUnit.FromPoint(currentX);


                //switch (this.Alignment)
                //{
                //    case Alignments.TopLeft:
                //    case Alignments.TopCenter:
                //    case Alignments.TopRight:
                //        //item.Y = this.Y;
                //        break;
                //    case Alignments.CenterLeft:
                //    case Alignments.CenterCenter:
                //    case Alignments.CenterRight:
                //        //item.Y = this.Y + ((CanGrow ? maxHeight : this.H.Point) - item.H.Point) / 2;
                //        //item.Y = this.Y + (this.H.Point - item.H.Point) / 2;
                //        break;
                //    case Alignments.BottomLeft:
                //    case Alignments.BottomCenter:
                //    case Alignments.BottomRight:
                //        //item.Y = this.Y + (CanGrow ? maxHeight : this.H.Point) - item.H.Point;
                //        //item.Y = this.Y + this.H.Point - item.H.Point;
                //        break;
                //    default:
                //        break;
                //}

                //if ((item.X >= this.X) & (item.X + item.W <= this.X + this.W))
                item.Draw();

                currentX += item.W.Point + HSpace.Point;

                if (HasBorder)
                    gfx.DrawRectangle(XPens.Firebrick, this.X.Point, this.Y.Point, this.W.Point, this.H.Point);
            }
    
        }
        #endregion
    }
}
