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


            // 蓝色底格
            Pen pen = new Pen(m_color);
            GraphicsPath path = new GraphicsPath();
            while (bottom < top)
            {
                PointF p1 = canvas.ToScreen(new UnitPoint(-20, bottom));
                PointF p2 = canvas.ToScreen(new UnitPoint(180, bottom));
                path.AddLine(p1, p2);
                path.CloseFigure();
                bottom += gridY;
            }
            canvas.Graphics.DrawPath(pen, path);


            //画基架括号
      //      Image newImage = Image.FromFile("d:\\CSARP\\1.jpg");//建立要绘制的Image图像




            right = 174;
            pen = new Pen(l_color);
            path.Reset();
            left = 10;
            int pailine = 12;
            top = (float)Math.Round(top);
            while (left < right)
            {
                bottom = 10;
                while (bottom < top)
                {
                    
                      
                    PointF p1 = canvas.ToScreen(new UnitPoint(leftpoint.X + left, top - bottom));
                    PointF p2 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + pailine, top - bottom));
                    bottom += 1;
                    path.AddLine(p1, p2);
                    path.CloseFigure();
                    for (int i = 0; i < 4; i++)
                    {
                        PointF p3 = canvas.ToScreen(new UnitPoint(leftpoint.X + left+3*i, top - bottom));
                        PointF p4 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 1+3*i, top - bottom));
                        path.AddLine(p3, p4);
                        path.CloseFigure();
                    }
                    bottom += 2;
                  
                    PointF p5 = canvas.ToScreen(new UnitPoint(leftpoint.X + left, top - bottom));
                    PointF p6 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + pailine, top - bottom));
                    bottom += 2;
                    path.AddLine(p5, p6);
                    path.CloseFigure();
                    for (int i = 0; i < 4; i++)
                    {
                        PointF p7 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 3 * i, top - bottom));
                        PointF p8 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 1+3*i, top - bottom));
                        path.AddLine(p7, p8);
                        path.CloseFigure();
                    }
                    bottom += 1;
                
                    PointF p9 = canvas.ToScreen(new UnitPoint(leftpoint.X + left, top - bottom));
                    PointF p10 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + pailine, top - bottom));
                    bottom += 2;
                    path.AddLine(p9, p10);
                    path.CloseFigure();
                    for (int i = 0; i < 4; i++)
                    {
                        PointF p11 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 3 * i, top - bottom));
                        PointF p12 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 3 * i+1, top - bottom));
                       
                        path.AddLine(p11, p12);
                        path.CloseFigure();
                    }
                    bottom += 2;
                    PointF p13 = canvas.ToScreen(new UnitPoint(leftpoint.X + left, top - bottom));
                    PointF p14 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + pailine, top - bottom));
                    bottom += 2;
                    path.AddLine(p13, p14);
                    path.CloseFigure();
                    for (int i = 0; i < 4; i++)
                    {
                        PointF p15 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 3 * i, top - bottom));
                        PointF p16 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 3 * i+1, top - bottom));
                        
                        path.AddLine(p15, p16);
                        path.CloseFigure();
                    }
                    bottom += 1;
                    PointF p17 = canvas.ToScreen(new UnitPoint(leftpoint.X + left, top - bottom));
                    PointF p18 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + pailine, top - bottom));
                    bottom += 7;
                    path.AddLine(p17, p18);
                    path.CloseFigure();



                    PointF p19 = canvas.ToScreen(new UnitPoint(leftpoint.X + left, top - bottom));
                    PointF p20 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + pailine, top - bottom));
                    bottom += 2;
                    path.AddLine(p19, p20);
                    path.CloseFigure();
                    for (int i = 0; i < 4; i++)
                    {
                        PointF p21 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 3 * i, top - bottom));
                        PointF p22 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 3 * i+1, top - bottom));
                        
                        path.AddLine(p21, p22);
                        path.CloseFigure();
                    }
                    bottom += 2;
                    PointF p23 = canvas.ToScreen(new UnitPoint(leftpoint.X + left, top - bottom));
                    PointF p24 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + pailine, top - bottom));
                    bottom += 1;
                    path.AddLine(p23, p24);
                    path.CloseFigure();
                    for (int i = 0; i < 4; i++)
                    {
                        PointF p25 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 3 * i, top - bottom));
                        PointF p26 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 3 * i+1, top - bottom));
                       
                        path.AddLine(p25, p26);
                        path.CloseFigure();
                    }
                    bottom += 2;
                    PointF p27 = canvas.ToScreen(new UnitPoint(leftpoint.X + left, top - bottom));
                    PointF p28 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + pailine, top - bottom));
                    bottom += 2;
                    path.AddLine(p27, p28);
                    path.CloseFigure();
                    for (int i = 0; i < 4; i++)
                    {
                        PointF p29 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 3 * i, top - bottom));
                        PointF p30 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 3 * i+1, top - bottom));
                       
                        path.AddLine(p29, p30);
                        path.CloseFigure();
                    }
                    bottom += 1;
                    PointF p31 = canvas.ToScreen(new UnitPoint(leftpoint.X + left, top - bottom));
                    PointF p32 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + pailine, top - bottom));
                    bottom += 2;
                    path.AddLine(p31, p32);
                    for (int i = 0; i < 4; i++)
                    {
                        path.CloseFigure();
                        PointF p33 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 3 * i, top - bottom));
                        PointF p34 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + 3 * i+1, top - bottom));
                        
                        path.AddLine(p33, p34);
                        path.CloseFigure();
                    }
                    bottom += 2;
                    PointF p35 = canvas.ToScreen(new UnitPoint(leftpoint.X + left, top - bottom));
                    PointF p36 = canvas.ToScreen(new UnitPoint(leftpoint.X + left + pailine, top - bottom));
                    path.AddLine(p35, p36);
                    path.CloseFigure();
                    bottom += 20;
                }
                left += 14;
               
            }
            canvas.Graphics.DrawPath(pen, path);

            canvas.Graphics.DrawString("{", new Font("方正兰亭超细黑简体", 105, FontStyle.Regular),  
                                    new SolidBrush(Color.Black),-37,25);
            canvas.Graphics.DrawString("{", new Font("方正兰亭超细黑简体", 105, FontStyle.Regular),
                                    new SolidBrush(Color.Black), -37, 240);
            canvas.Graphics.DrawString("{", new Font("方正兰亭超细黑简体", 105, FontStyle.Regular),
                                    new SolidBrush(Color.Black), -37, 455);
            canvas.Graphics.DrawString("{", new Font("方正兰亭超细黑简体", 105, FontStyle.Regular),
                                    new SolidBrush(Color.Black), -37, 670);
                                       
        }
        public void Draw(ICanvas canvas, RectangleF unitrect,int start ,int end ,int time,int strong)
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
