using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using PdfSharp;
using PdfSharp.Drawing;
//using PdfSharp.Pdf;
//using PdfSharp.Pdf.IO;


namespace MyAlbum2 
{

    class StampRow 
	{
        public enum HAligns
        {
            FS, ES, JS
        }
        public enum VAligns
        {
            TOP, BOTTOM, CENTRE
        }

		XGraphics gfx;
		public List<Stamp> Stamps { get; set; }
		public HAligns HAlign { get; set; }
		public VAligns VAlign { get; set; }
		public double HSpace { get; set; }
		public double YPos { get; set; }
		
		
		public StampRow(XGraphics gfx, string hAlign, string vAlign, double y)
		{
			this.gfx = gfx;
			Stamps = new List<Stamp>();
			
			#region switch (hAlign)
			switch (hAlign.Trim().Substring(0, 2))
			{
				case "FS":
					HAlign = HAligns.FS;
					break;
				case "ES":
					HAlign = HAligns.ES;
					break;
				case "JS":
					HAlign = HAligns.JS;
					break;
				default:
					throw new Exception("Invalid hAlign parameter. Valid values are FS, ES, JS.");
			}
			#endregion
			#region switch(vAlign)
			switch (vAlign.Trim().Substring(0, 3))
			{
				case "TOP":
					VAlign = VAligns.TOP;
					break;
				case "BOT":
					VAlign = VAligns.BOTTOM;
					break;
				case "CEN":
					VAlign = VAligns.CENTRE;
					break;
				default:
					throw new Exception("Invalid vAlign parameter. Valid values are TOP, BOT(tom), CEN(tre).");
			}
			#endregion

			YPos = y;

		}

		public void Draw()
		{
            /*
			double rowWidth=0; //, rowHeight=0;

			#region Horizontal Alignment
			switch (HAlign)
			{
				#region HAligns.ES
				case StampRow.HAligns.ES:
					// get rowWidth
					foreach (Stamp stamp in Stamps)
					{
						rowWidth += stamp.Width;
					}

					HSpace = (AlbumPage.XRight - AlbumPage.XLeft - rowWidth) / (Stamps.Count + 1);

					double XPos = AlbumPage.XLeft + HSpace;

					foreach (Stamp stamp in Stamps)
					{
						stamp.XPos = XPos;
						XPos += stamp.Width + HSpace;
					}
					break;
				#endregion
				#region HAligns.FS
				case StampRow.HAligns.FS:
					foreach (StampDisplay stamp in Stamps)
					{
						rowWidth += stamp.Width;
					}

					rowWidth += (Stamps.Count - 1) * HSpace;

					XPos = (AlbumPage.XLeft + AlbumPage.XRight - rowWidth) / 2;

					foreach (StampDisplay stamp in Stamps)
					{
						stamp.XPos = XPos;
						XPos += stamp.Width + HSpace;
					}
					break;
				#endregion
				#region HAligns.JS
				case StampRow.HAligns.JS:
					if (Stamps.Count == 1)
					{
						Stamps[0].XPos = (AlbumPage.XRight + AlbumPage.XLeft - Stamps[0].Width) / 2;
					}
					else
					{
						foreach (StampDisplay stamp in Stamps)
						{
							rowWidth += stamp.Width;
						}

						HSpace = (AlbumPage.XRight - AlbumPage.XLeft - rowWidth) / (Stamps.Count - 1);

						XPos = AlbumPage.XLeft;

						foreach (StampDisplay stamp in Stamps)
						{
							stamp.XPos = XPos;
							XPos += stamp.Width + HSpace;
						}
					}
					break;
				#endregion

			}
			#endregion

			#region Vertical Alignment
			double max = 0;
			switch (VAlign)
			{
				#region VAligns.TOP
				case StampRow.VAligns.TOP:
					// get max title height
					foreach (StampDisplay stamp in Stamps)
					{
						double h = stamp.Title.Height + StampDisplay.VSpace;
						max = (h > max ? h : max);
					}

					foreach (StampDisplay stamp in Stamps)
					{
						stamp.YPos = this.YPos + max;
					}
					break;
				#endregion
				#region VAligns.BOTTOM
				case StampRow.VAligns.BOTTOM:
					foreach (StampDisplay stamp in Stamps)
					{
						double h = stamp.Title.Height + StampDisplay.VSpace + stamp.Frame.Height;
						max = (h > max ? h : max);
					}

					foreach (StampDisplay stamp in Stamps)
					{
						stamp.YPos = this.YPos + max - stamp.Frame.Height;
					}
					break;
				#endregion
				#region VAligns.CENTRE
				case StampRow.VAligns.CENTRE:
					foreach (StampDisplay stamp in Stamps)
					{
						double h = stamp.Title.Height + StampDisplay.VSpace + stamp.Frame.Height / 2;
						max = (h > max ? h : max);
					}

					foreach (StampDisplay stamp in Stamps)
					{
						stamp.YPos = this.YPos + max - stamp.Frame.Height / 2;
					}

					break;
				#endregion
			}
			#endregion

			foreach (Stamp stamp in Stamps)
			{
				stamp.Draw();
			}
             */
		}
	}
}
