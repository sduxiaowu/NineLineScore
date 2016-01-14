using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

namespace NineLineNotation
{
    public class GridLayer : ICanvasLayer, ISerialize
    {
        private bool m_enabled = true;
        public SizeF m_spacing = new SizeF(1f, 1f); // 12"
        private int m_minSize = 2;
        private Color m_color = Color.FromArgb(50, Color.Blue);
        public Color l_color = Color.Black;
        public Color d_color = Color.Red;
        public SizeF Spacing
        {
            get { return m_spacing; }
            set { m_spacing = value; }
        }
        public int MinSize
        {
            get { return m_minSize; }
            set { m_minSize = value; }
        }
        public Color Color
        {
            get { return m_color; }
            set { m_color = value; }
        }


        #region ICanvasLayer Members
        public void Draw(ICanvas canvas, RectangleF unitrect)
        {
          
            if (Enabled == false)
                return;

            float gridX = Spacing.Width;
            float gridY = Spacing.Height;
            //Console.WriteLine("gridX=" + gridX + " ; Y=" + gridY);
            float gridscreensizeX = canvas.ToScreen(gridX);
            float gridscreensizeY = canvas.ToScreen(gridY);
            //Console.WriteLine("gridscreensizeX=" + gridscreensizeX + " ; Y=" + gridscreensizeY);
            if (gridscreensizeX < MinSize || gridscreensizeY < MinSize)
                return;

            PointF leftpoint = unitrect.Location;
            PointF rightpoint = ScreenUtils.RightPoint(canvas, unitrect);

            float left = (float)Math.Round(leftpoint.X / gridX) * gridX;
            float top = unitrect.Height + unitrect.Y;
            float right = rightpoint.X;
            float bottom = (float)Math.Round(leftpoint.Y / gridY) * gridY;


           // Console.WriteLine("left=" + left + " ; top=" + top + " ; right=" + right + " ; bottom=" + bottom);

            Pen pen = new Pen(m_color);
            GraphicsPath path = new GraphicsPath();

            // draw vertical lines
            /*
            while (left < right)
            {
                PointF p1 = canvas.ToScreen(new UnitPoint(left, leftpoint.Y));
                PointF p2 = canvas.ToScreen(new UnitPoint(left, rightpoint.Y));
                path.AddLine(p1, p2);
                path.CloseFigure();
                left += gridX;
            }
            */
            // draw horizontal lines
            
            while (bottom < top)
            {
                PointF p1 = canvas.ToScreen(new UnitPoint(leftpoint.X, bottom));
                PointF p2 = canvas.ToScreen(new UnitPoint(rightpoint.X, bottom));
                path.AddLine(p1, p2);
                path.CloseFigure();
                bottom += gridY;
            }
            canvas.Graphics.DrawPath(pen, path);

            pen = new Pen(l_color);
            path.Reset();
            left = 2;
            top = (float)Math.Round(top);
            while (left < right)
            {
                bottom = 17;
                while (bottom < top)
                {
                    if (bottom % 30 == 0)
                        bottom += 17;
                    PointF p1 = canvas.ToScreen(new UnitPoint(leftpoint.X + left, top-bottom));
                    PointF p2 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 8, top-bottom));
                    bottom += 3;
                    path.AddLine(p1, p2);
                    path.CloseFigure();
                    PointF p3 = canvas.ToScreen(new UnitPoint(leftpoint.X + left, top-bottom));
                    PointF p4 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 8, top-bottom));
                    bottom += 3;
                    path.AddLine(p3, p4);
                    path.CloseFigure();
                    PointF p5 = canvas.ToScreen(new UnitPoint(leftpoint.X + left, top-bottom));
                    PointF p6 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 8, top-bottom));
                    bottom += 4;
                    path.AddLine(p5, p6);
                    path.CloseFigure();
                    PointF p7 = canvas.ToScreen(new UnitPoint(leftpoint.X + left, top-bottom));
                    PointF p8 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 8, top-bottom));
                    bottom += 3;
                    path.AddLine(p7, p8);
                    path.CloseFigure();
                    PointF p9 = canvas.ToScreen(new UnitPoint(leftpoint.X + left, top-bottom));
                    PointF p10 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 8, top-bottom));
                    path.AddLine(p9, p10);
                    path.CloseFigure();
                }
                left += 9;
                if ((left-2)%36 == 0)
                {
                    bottom = 17;
                    while (bottom < top) {
                        PointF p1 = canvas.ToScreen(new UnitPoint(leftpoint.X + left , top - bottom));
                        PointF p2 = canvas.ToScreen(new UnitPoint(leftpoint.X + left , top - bottom-13));
                        bottom += 13;
                        path.AddLine(p1, p2);
                        path.CloseFigure();

                        if (bottom % 30 == 0)
                            bottom += 17;

                    }
                }
            }
            canvas.Graphics.DrawPath(pen, path);
        }
        public void Draw(ICanvas canvas, RectangleF unitrect,int start ,int end ,int time)
        {

            if (Enabled == false)
                return;

            float gridX = Spacing.Width;
            float gridY = Spacing.Height;
            //Console.WriteLine("gridX=" + gridX + " ; Y=" + gridY);
            float gridscreensizeX = canvas.ToScreen(gridX);
            float gridscreensizeY = canvas.ToScreen(gridY);
            //Console.WriteLine("gridscreensizeX=" + gridscreensizeX + " ; Y=" + gridscreensizeY);
            if (gridscreensizeX < MinSize || gridscreensizeY < MinSize)
                return;

            PointF leftpoint = unitrect.Location;
            PointF rightpoint = ScreenUtils.RightPoint(canvas, unitrect);

            float left = (float)Math.Round(leftpoint.X / gridX) * gridX;
            float top = unitrect.Height + unitrect.Y;
            float right = rightpoint.X;
            float bottom = (float)Math.Round(leftpoint.Y / gridY) * gridY;


             //Console.WriteLine("left=" + left + " ; top=" + top + " ; right=" + right + " ; bottom=" + bottom);

            Pen pen = new Pen(d_color);
            GraphicsPath path = new GraphicsPath();
            while (bottom < top)
            {
                PointF p1 = canvas.ToScreen(new UnitPoint(leftpoint.X, bottom));
                PointF p2 = canvas.ToScreen(new UnitPoint(rightpoint.X, bottom));
                path.AddLine(p1, p2);
                path.CloseFigure();
                bottom += gridY;
            }
            
            if(canvas!=null){canvas.Graphics.DrawPath(pen, path); }
        }
            

        public bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }
        public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj)
        {
            if (Enabled == false)
                return null;
            UnitPoint snappoint = new UnitPoint();
            UnitPoint mousepoint = point;
            float gridX = Spacing.Width;
            float gridY = Spacing.Height;
            snappoint.X = (float)(Math.Round(mousepoint.X / gridX)) * gridX;
            snappoint.Y = (float)(Math.Round(mousepoint.Y / gridY)) * gridY;
            double threshold = canvas.ToUnit(/*ThresholdPixel*/6);
            if ((snappoint.X < point.X - threshold) || (snappoint.X > point.X + threshold))
                return null;
            if ((snappoint.Y < point.Y - threshold) || (snappoint.Y > point.Y + threshold))
                return null;
            return new DrawTools.GridSnapPoint(canvas, snappoint);
        }
        public IEnumerable<IDrawObject> Objects
        {
            get { return null; }
        }
        public bool Visible
        {
            get { return true; }
        }
        #endregion
    }
}
