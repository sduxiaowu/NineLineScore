using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Reflection;

namespace NineLineNotation
{
    public class DataModel : IModel
    {
        float m_zoom = 1;
        GridLayer m_gridLayer = new GridLayer();
        BackgroundLayer m_backgroundLayer = new BackgroundLayer();
        Dictionary<IDrawObject, bool> m_selection = new Dictionary<IDrawObject, bool>();
        List<ICanvasLayer> m_layers = new List<ICanvasLayer>();
        ICanvasLayer m_activeLayer;
        Dictionary<string, IDrawObject> m_drawObjectTypes = new Dictionary<string, IDrawObject>(); 
        UndoRedoBuffer m_undoBuffer = new UndoRedoBuffer();

        public DataModel()
        {
            m_layers.Clear();
            m_layers.Add(new DrawingLayer("layer0", "Hairline Layer", Color.Red, 0.0f));
        }
        public void AddDrawTool(string key, IDrawObject drawtool)
        {
            m_drawObjectTypes[key] = drawtool;
        }
        public void MoveNodes(UnitPoint position, IEnumerable<INodePoint> nodes)
        {
            if (m_undoBuffer.CanCapture)
                m_undoBuffer.AddCommand(new EditCommandNodeMove(nodes));
            foreach (INodePoint node in nodes)
            {
                node.SetPosition(position);
                node.Finish();
            }
        }

        public List<IDrawObject> GetHitObjects(ICanvas canvas, RectangleF selection, bool anyPoint)
        {
            List<IDrawObject> selected = new List<IDrawObject>();
            foreach (ICanvasLayer layer in m_layers)
            {
                if (layer.Visible == false)
                    continue;
                foreach (IDrawObject drawobject in layer.Objects)
                {
                    if (drawobject.ObjectInRectangle(canvas, selection, anyPoint))
                        selected.Add(drawobject);
                }
            }
            return selected;
        }
        public List<IDrawObject> GetHitObjects(ICanvas canvas, UnitPoint point)
        {
            List<IDrawObject> selected = new List<IDrawObject>();
            foreach (ICanvasLayer layer in m_layers)
            {
                if (layer.Visible == false)
                    continue;
                foreach (IDrawObject drawobject in layer.Objects)
                {
                    if (drawobject.PointInObject(canvas, point))
                        selected.Add(drawobject);
                }
            }
            return selected;
        }
        public void ClearSelectedObjects()
        {
            IEnumerable<IDrawObject> x = SelectedObjects;
            foreach (IDrawObject drawobject in x)
            {
                DrawTools.DrawObjectBase obj = drawobject as DrawTools.DrawObjectBase;
                obj.Selected = false;
            }
            m_selection.Clear();
        }
        DrawTools.DrawObjectBase CreateObject(string objecttype)
        {
            if (m_drawObjectTypes.ContainsKey(objecttype))
            {
                return m_drawObjectTypes[objecttype].Clone() as DrawTools.DrawObjectBase;
            }
            return null;
        }
        public IDrawObject CreateObject(string type, UnitPoint point, ISnapPoint snappoint)
        {
            DrawingLayer layer = ActiveLayer as DrawingLayer;
            if (layer.Enabled == false)
                return null;
            DrawTools.DrawObjectBase newobj = CreateObject(type);
            if (newobj != null)
            {
                newobj.Layer = layer;
                newobj.InitializeFromModel(point, layer, snappoint);
            }
            return newobj as IDrawObject;
        }
        public void AddObject(ICanvasLayer layer, IDrawObject drawobject)
        {
            if (drawobject is DrawTools.IObjectEditInstance)
                drawobject = ((DrawTools.IObjectEditInstance)drawobject).GetDrawObject();
            if (m_undoBuffer.CanCapture)
                m_undoBuffer.AddCommand(new EditCommandAdd(layer, drawobject));
            ((DrawingLayer)layer).AddObject(drawobject);
        }
        public void DeleteObjects(IEnumerable<IDrawObject> objects)
        {
            EditCommandRemove undocommand = null;
            if (m_undoBuffer.CanCapture)
                undocommand = new EditCommandRemove();
            foreach (ICanvasLayer layer in m_layers)
            {
                List<IDrawObject> removedobjects = ((DrawingLayer)layer).DeleteObjects(objects);
                if (removedobjects != null && undocommand != null)
                    undocommand.AddLayerObjects(layer, removedobjects);
            }
            if (undocommand != null)
                m_undoBuffer.AddCommand(undocommand);
        }

        public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, Type[] runningsnaptypes, Type usersnaptype)
        {
            List<IDrawObject> objects = GetHitObjects(canvas, point);
            if (objects.Count == 0)
                return null;

            foreach (IDrawObject obj in objects)
            {
                ISnapPoint snap = obj.SnapPoint(canvas, point, objects, runningsnaptypes, usersnaptype);
                if (snap != null)
                    return snap;
            }
            return null;
        }


        public ICanvasLayer BackgroundLayer
        {
            get { return m_backgroundLayer; }
        }
        public ICanvasLayer GridLayer
        {
            get { return m_gridLayer; }
        }
        public float Zoom
        {
            get { return m_zoom; }
            set { m_zoom = value; }
        }
        public int SelectedCount
        {
            get { return m_selection.Count; }
        }

        public IEnumerable<IDrawObject> SelectedObjects
        {
            get
            {
                return m_selection.Keys;
            }
        }
        public ICanvasLayer[] Layers
        {
            get { return m_layers.ToArray(); }
        }
        public ICanvasLayer ActiveLayer
        {
            get
            {
                m_activeLayer = m_layers[0];
                return m_activeLayer;
            }
            set
            {
                m_activeLayer = value;
            }
        }
    }
}
