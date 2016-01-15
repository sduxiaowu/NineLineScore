using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace NineLineNotation
{
    public class DrawingLayer : ICanvasLayer, ISerialize
    {   
        string m_id;
        string m_name = "<Layer>";
        Color m_color;
        float m_width = 0.00f;
        bool m_enabled = true;
        bool m_visible = true;
        List<IDrawObject> m_objects = new List<IDrawObject>();
        Dictionary<IDrawObject, bool> m_objectMap = new Dictionary<IDrawObject, bool>();
        //djl
        public SizeF m_spacing = new SizeF(1f, 1f);
        private int m_minSize = 2;
        public Color Color
        {
            get { return m_color; }
            set { m_color = value; }
        }
        public float Width
        {
            get { return m_width; }
            set { m_width = value; }
        }
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        public DrawingLayer(string id, string name, Color color, float width)
        {
            m_id = id;
            m_name = name;
            m_color = color;
            m_width = width;
        }
        
        public void AddObject(IDrawObject drawobject)
		{
			if (m_objectMap.ContainsKey(drawobject))
				return; // this should never happen
			if (drawobject is DrawTools.DrawObjectBase)
				((DrawTools.DrawObjectBase)drawobject).Layer = this;
			m_objects.Add(drawobject);
			m_objectMap[drawobject] = true;
		}
        
		public List<IDrawObject> DeleteObjects(IEnumerable<IDrawObject> objects)
		{
			if (Enabled == false)
				return null;
			List<IDrawObject> removedobjects = new List<IDrawObject>();
			// first remove from map only
			foreach (IDrawObject obj in objects)
			{
				if (m_objectMap.ContainsKey(obj))
				{
					m_objectMap.Remove(obj);
					removedobjects.Add(obj);
				}
			}
			// need some smart algorithm here to either remove from existing list or build a new list
			// for now I will just ise the removed count;
			if (removedobjects.Count == 0)
				return null;
			if (removedobjects.Count < 10) // remove from existing list
			{
				foreach (IDrawObject obj in removedobjects)
					m_objects.Remove(obj);
			}
			else // else build new list;
			{
				List<IDrawObject> newlist = new List<IDrawObject>();
				foreach (IDrawObject obj in m_objects)
				{
					if (m_objectMap.ContainsKey(obj))
						newlist.Add(obj);
				}
				m_objects.Clear();
				m_objects = newlist;
			}
			return removedobjects;
		}
        /*
		public int Count
		{
			get { return m_objects.Count; }
		}
		public void Copy(DrawingLayer acopy, bool includeDrawObjects)
		{
			if (includeDrawObjects)
				throw new Exception("not supported yet");
			m_id = acopy.m_id;
			m_name = acopy.m_name;
			m_color = acopy.m_color;
			m_width = acopy.m_width;
			m_enabled = acopy.m_enabled;
			m_visible = acopy.m_visible;
		}
        */

        #region ICanvasLayer Members
        public void Draw(ICanvas canvas, RectangleF unitrect)
        {
            int cnt = 0;
            Console.WriteLine("Draw ing Layer");
            foreach (IDrawObject drawobject in m_objects)
            {
                DrawTools.DrawObjectBase obj = drawobject as DrawTools.DrawObjectBase;
                if (obj is IDrawObject && ((IDrawObject)obj).ObjectInRectangle(canvas, unitrect, true) == false)
                    continue;
                bool sel = obj.Selected;
                bool high = obj.Highlighted;
                obj.Selected = false;
                drawobject.Draw(canvas, unitrect);
                obj.Selected = sel;
                obj.Highlighted = high;
                cnt++;
            }
        }
        //djl
         public void Draw(ICanvas canvas, RectangleF unitrect,int start,int end,int line,int strong){
             if (Enabled == false)
                 return;

             float gridX = m_spacing.Width;
             float gridY = m_spacing.Height;
             //Console.WriteLine("gridX=" + gridX + " ; Y=" + gridY);
             float gridscreensizeX = canvas.ToScreen(gridX);
             float gridscreensizeY = canvas.ToScreen(gridY);
             //Console.WriteLine("gridscreensizeX=" + gridscreensizeX + " ; Y=" + gridscreensizeY);
             if (gridscreensizeX < m_minSize || gridscreensizeY < m_minSize)
                 return;

             PointF leftpoint = unitrect.Location;
             PointF rightpoint = ScreenUtils.RightPoint(canvas, unitrect);

             float left = (float)Math.Round(leftpoint.X / gridX) * gridX;
             float top = unitrect.Height + unitrect.Y;
             float right = rightpoint.X;
             float bottom = (float)Math.Round(leftpoint.Y / gridY) * gridY;




             Pen pen = new Pen(Color.FromArgb(255, (250 - strong), (160 - strong), (160 - strong)), 2);
             GraphicsPath path = new GraphicsPath();
             PointF p1;
             PointF p2;
             if(true)
             {
                 Console.WriteLine("left=" + left + " ; top=" + top + " ; right=" + right + " ; bottom=" + bottom
                     +"; start="+start+";end="+end+";line="+line);
                  p1= canvas.ToScreen(new UnitPoint(leftpoint.X+start,top-line*gridY));
                  p2= canvas.ToScreen(new UnitPoint(leftpoint.X+end, top-line*gridY));
                 
             }
         
             canvas.Graphics.DrawLine(pen,p1,p2);
            
         }
      
        public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj)
        {
            foreach (IDrawObject obj in m_objects)
            {
                ISnapPoint sp = obj.SnapPoint(canvas, point, otherobj, null, null);
                if (sp != null)
                    return sp;
            }
            return null;
        }
        public IEnumerable<IDrawObject> Objects
        {
            get { return m_objects; }
        }
        public bool Enabled
        {
            get { return m_enabled && m_visible; }
            set { m_enabled = value; }
        }
        public bool Visible
        {
            get { return m_visible; }
            set { m_visible = value; }
        }
        #endregion
    }
}
