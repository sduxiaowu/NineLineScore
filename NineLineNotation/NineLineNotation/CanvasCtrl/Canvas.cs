using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace NineLineNotation
{

    public struct CanvasWrapper : ICanvas
    {
        CanvasCtrl m_canvas;
        Graphics m_graphics;
        Rectangle m_rect;
        public CanvasWrapper(CanvasCtrl canvas)
        {
            m_canvas = canvas;
            m_graphics = null;
            m_rect = new Rectangle();
        }
        public CanvasWrapper(CanvasCtrl canvas, Graphics graphics, Rectangle clientrect)
        {
            m_canvas = canvas;
            m_graphics = graphics;
            m_rect = clientrect;
        }
        public IModel Model
        {
            get { return m_canvas.Model; }
        }
        public CanvasCtrl CanvasCtrl
        {
            get { return m_canvas; }
        }
        
        public void Dispose()
        {
            //graphics置空
            m_graphics = null;
        }


        #region ICanvas
        public IModel DataModel
        {
            get { return m_canvas.Model; }
        }

        public PointF ToScreen(UnitPoint unitpoint)
        {
            return m_canvas.ToScreen(unitpoint);
        }
        public float ToScreen(double unitvalue)
        {
            return m_canvas.ToScreen(unitvalue);
        }
        public double ToUnit(float screenvalue)
        {
            return m_canvas.ToUnit(screenvalue);
        }
        public UnitPoint ToUnit(PointF screenpoint)
        {
            return m_canvas.ToUnit(screenpoint);
        }
        public Graphics Graphics
        {
            get { return m_graphics; }
        }

        public Rectangle ClientRectangle
        {
            get { return m_rect; }
            set { m_rect = value; }
        }

        public Pen CreatePen(Color color, float unitWidth)
        {
            return m_canvas.CreatePen(color, unitWidth);
        }
        public void DrawLine(ICanvas canvas, Pen pen, UnitPoint p1, UnitPoint p2)
        {
            m_canvas.DrawLine(canvas, pen, p1, p2);
        }
        public void DrawSymbol(ICanvas canvas,  int p1, int p2, Bitmap bmp, double scale)
        {
            m_canvas.DrawSymbol(canvas, p1, p2, bmp, scale);
        }
        public void Invalidate()
        {
            m_canvas.DoInvalidate(false);
        }
        #endregion
        
    }

    public partial class CanvasCtrl : UserControl
    {
        public static CanvasCtrl M_canvas;

        enum eCommandType
        {
            select,
            pan,
            move,
            draw,
            edit,
            editNode,
            symbol
        }

        public IModel m_model;
        PointF m_panOffset = new PointF(25, -25);
        PointF m_dragOffset = new PointF(0, 0);
        float m_screenResolution = 4;
        Bitmap m_staticImage = null;
        bool m_staticDirty = true;
        SelectionRectangle m_selection = null;
        string m_drawObjectId = string.Empty;
        PointF m_mousedownPoint;
        eCommandType m_commandType = eCommandType.select;
        bool m_runningSnaps = true;
        Type[] m_runningSnapTypes = null;
        ISnapPoint m_snappoint = null;
        NodeMoveHelper m_nodeMoveHelper = null;
        public IDrawObject m_newObject = null;
        public CanvasWrapper m_canvaswrapper;
        System.Drawing.Drawing2D.SmoothingMode m_smoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
        Dictionary<float, Dictionary<Color, Pen>> m_penCache = new Dictionary<float, Dictionary<Color, Pen>>();
        //djl
        public CanvasWrapper dcanvaswrapper;
        public RectangleF drf;


        public CanvasCtrl(ICanvasOwner owner, IModel datamodel)
        {
            M_canvas = this;

            m_canvaswrapper = new CanvasWrapper(this);
            m_model = datamodel;

            ((DataModel)m_model).AddDrawTool("Pen", new DrawTools.LineEdit(false));
            //////////////////////////////////////////
            //??????????????????????????????????
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            InitializeComponent();
            m_commandType = eCommandType.select;

            BorderStyle = BorderStyle.None;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            m_nodeMoveHelper = new NodeMoveHelper(m_canvaswrapper);
        }

        public UnitPoint GetMousePoint()
        {
            Point point = this.PointToClient(Control.MousePosition);
            return ToUnit(point);
        }
        public bool RunningSnapsEnabled
        {
            get { return m_runningSnaps; }
            set { m_runningSnaps = value; }
        }

        public Type[] RunningSnaps
        {
            get { return m_runningSnapTypes; }
            set { m_runningSnapTypes = value; }
        }

        protected void SetCenterScreen(PointF screenPoint, bool setCursor)
        {
            float centerX = ClientRectangle.Width / (float)1.5;
            m_panOffset.X += centerX - screenPoint.X;

            float centerY = ClientRectangle.Height / (float)1.5;
            m_panOffset.Y += centerY - screenPoint.Y;

            //if (setCursor)
            //    Cursor.Position = this.PointToScreen(new Point((int)centerX, (int)centerY));
            DoInvalidate(true);
        }
        public void testpaint(){
           
        
        
        
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            //CommonTools.Tracing.StartTrack(Program.TracePaint);
            //ClearPens();
            e.Graphics.SmoothingMode = m_smoothingMode;
            CanvasWrapper dc = new CanvasWrapper(this, e.Graphics, ClientRectangle);
            Rectangle cliprectangle = e.ClipRectangle;
            if (m_staticImage == null)
            {
                cliprectangle = ClientRectangle;
                m_staticImage = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
                m_staticDirty = true;
            }
            RectangleF r = ScreenUtils.ToUnitNormalized(dc, cliprectangle);
            if (float.IsNaN(r.Width) || float.IsInfinity(r.Width))
            {
                r = ScreenUtils.ToUnitNormalized(dc, cliprectangle);
            }
            if (m_staticDirty)
            {
                m_staticDirty = false;
                CanvasWrapper dcStatic = new CanvasWrapper(this, Graphics.FromImage(m_staticImage), ClientRectangle);
                dcStatic.Graphics.SmoothingMode = m_smoothingMode;
                m_model.BackgroundLayer.Draw(dcStatic, r);
                //九线谱底谱
                if (m_model.GridLayer.Enabled)
                    m_model.GridLayer.Draw(dcStatic, r);

                //djl
                dcanvaswrapper = dcStatic;
                drf = r;
                /*
                PointF nullPoint = ToScreen(new UnitPoint(0, 0));
                dcStatic.Graphics.DrawLine(Pens.Blue, nullPoint.X - 10, nullPoint.Y, nullPoint.X + 10, nullPoint.Y);
                dcStatic.Graphics.DrawLine(Pens.Blue, nullPoint.X, nullPoint.Y - 10, nullPoint.X, nullPoint.Y + 10);
                */
                ICanvasLayer[] layers = m_model.Layers;
                for (int layerindex = layers.Length - 1; layerindex >= 0; layerindex--)
                {
                    if (layers[layerindex] != m_model.ActiveLayer && layers[layerindex].Visible)
                        layers[layerindex].Draw(dcStatic, r);
                }
                if (m_model.ActiveLayer != null)
                    m_model.ActiveLayer.Draw(dcStatic, r);
                
                dcStatic.Dispose();
            }
            e.Graphics.DrawImage(m_staticImage, cliprectangle, cliprectangle, GraphicsUnit.Pixel);
            /*
            foreach (IDrawObject drawobject in m_model.SelectedObjects)
                drawobject.Draw(dc, r);

            if (m_newObject != null)
                m_newObject.Draw(dc, r);

            if (m_snappoint != null)
                m_snappoint.Draw(dc);

            if (m_selection != null)
            {
                m_selection.Reset();
                m_selection.SetMousePoint(e.Graphics, this.PointToClient(Control.MousePosition));
            }
            if (m_moveHelper.IsEmpty == false)
                m_moveHelper.DrawObjects(dc, r);

            if (m_nodeMoveHelper.IsEmpty == false)
                m_nodeMoveHelper.DrawObjects(dc, r);
            dc.Dispose();
            ClearPens();
            CommonTools.Tracing.EndTrack(Program.TracePaint, "OnPaint complete");*/
        }

        void RepaintStatic(Rectangle r)
        {
            if (m_staticImage == null)
                return;
            Graphics dc = Graphics.FromHwnd(Handle);
            if (r.X < 0) r.X = 0;
            if (r.X > m_staticImage.Width) r.X = 0;
            if (r.Y < 0) r.Y = 0;
            if (r.Y > m_staticImage.Height) r.Y = 0;

            if (r.Width > m_staticImage.Width || r.Width < 0)
                r.Width = m_staticImage.Width;
            if (r.Height > m_staticImage.Height || r.Height < 0)
                r.Height = m_staticImage.Height;
            dc.DrawImage(m_staticImage, r, r, GraphicsUnit.Pixel);
            dc.Dispose();
        }

        void RepaintSnappoint(ISnapPoint snappoint)
        {
            if (snappoint == null)
                return;
            CanvasWrapper dc = new CanvasWrapper(this, Graphics.FromHwnd(Handle), ClientRectangle);
            snappoint.Draw(dc);
            dc.Graphics.Dispose();
            dc.Dispose();
        }

        PointF Translate(UnitPoint point)
        {
            return point.Point;
        }
        float ScreenHeight()
        {
            return (float)(ToUnit(this.ClientRectangle.Height) / m_model.Zoom);
        }
        public IModel Model
		{
			get { return m_model; }
			set { m_model = value; }
        }
        public PointF ToScreen(UnitPoint point)
        {
            PointF transformedPoint = Translate(point);
            transformedPoint.Y = ScreenHeight() - transformedPoint.Y;
            transformedPoint.Y *= m_screenResolution * m_model.Zoom;
            transformedPoint.X *= m_screenResolution * m_model.Zoom;

            transformedPoint.X += m_panOffset.X + m_dragOffset.X;
            transformedPoint.Y += m_panOffset.Y + m_dragOffset.Y;
            return transformedPoint;
        }
        public float ToScreen(double value)
        {
            return (float)(value * m_screenResolution * m_model.Zoom);
        }
        public double ToUnit(float screenvalue)
        {
            return (double)screenvalue / (double)(m_screenResolution * m_model.Zoom);
        }
        public UnitPoint ToUnit(PointF screenpoint)
        {
            float panoffsetX = m_panOffset.X + m_dragOffset.X;
            float panoffsetY = m_panOffset.Y + m_dragOffset.Y;
            float xpos = (screenpoint.X - panoffsetX) / (m_screenResolution * m_model.Zoom);
            float ypos = ScreenHeight() - ((screenpoint.Y - panoffsetY)) / (m_screenResolution * m_model.Zoom);
            return new UnitPoint(xpos, ypos);
        }

        public Pen CreatePen(Color color, float unitWidth)
        {
            return GetPen(color, ToScreen(unitWidth));
        }

        Pen GetPen(Color color, float width)
        {
            if (m_penCache.ContainsKey(width) == false)
                m_penCache[width] = new Dictionary<Color, Pen>();
            if (m_penCache[width].ContainsKey(color) == false)
                m_penCache[width][color] = new Pen(color, width);
            //return m_penCache[width][color];
            return new Pen(Color.DarkSlateGray, width+3);
        }

        public void DrawLine(ICanvas canvas, Pen pen, UnitPoint p1, UnitPoint p2)
        {
            PointF tmpp1 = ToScreen(p1);
            PointF tmpp2 = ToScreen(p2);
            //Console.WriteLine("pen:(" + pen.Color + "," + pen.Width + ")    tmpp1:(" + tmpp1.X + "," + tmpp1.Y + ")    tmpp2:(" + tmpp2.X + "," + tmpp2.Y+")");
            PointF tp = new PointF(tmpp2.X, tmpp1.Y);
            canvas.Graphics.DrawLine(pen, tmpp1, tp);
           
        }
        public void DrawSymbol(ICanvas canvas, int X, int Y, Image img, double scale)
        {
           // PointF tmpp1 = ToScreen(p1);
          //  PointF tmpp2 = ToScreen(p2);
            //Console.WriteLine("pen:(" + pen.Color + "," + pen.Width + ")    tmpp1:(" + tmpp1.X + "," + tmpp1.Y + ")    tmpp2:(" + tmpp2.X + "," + tmpp2.Y+")");
          //  PointF tp = new PointF(tmpp2.X, tmpp1.Y);
            
            //    MessageBox.Show("test  test " + X + "   " + Y);

            canvas.Graphics.DrawImage(img, new Rectangle(X - (int)(0.5 * scale * (img.Width)), Y - (int)(0.5 * scale * (img.Height)), (int)(scale * (img.Width)), (int)(scale * (img.Height))));
            
          //  canvas.Graphics.DrawImage(img, X, Y);

        }
        /*
        public void DoInvalidate(bool dostatic, RectangleF rect)
        {
            if (dostatic)
                m_staticDirty = true;
            Invalidate(ScreenUtils.ConvertRect(rect));
        }*/

        void RepaintObject(IDrawObject obj)
        {
            if (obj == null)
                return;
            CanvasWrapper dc = new CanvasWrapper(this, Graphics.FromHwnd(Handle), ClientRectangle);
            RectangleF invalidaterect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(dc, obj.GetBoundingRect(dc)));
            obj.Draw(dc, invalidaterect);
            dc.Graphics.Dispose();
            dc.Dispose();
        }

        public void DoInvalidate(bool dostatic)
        {
            if (dostatic)
                m_staticDirty = true;
            Invalidate();
        }
        public void DoInvalidate(bool dostatic, RectangleF rect)
        {
            if (dostatic)
                m_staticDirty = true;
            Invalidate(ScreenUtils.ConvertRect(rect));
        }

        public virtual void HandleMouseDownWhenDrawing(UnitPoint mouseunitpoint, ISnapPoint snappoint)
        {
            if (m_commandType == eCommandType.draw)
            {
                if (m_newObject == null)
                {
                    m_newObject = m_model.CreateObject(m_drawObjectId, mouseunitpoint, snappoint);
                    DoInvalidate(false, m_newObject.GetBoundingRect(m_canvaswrapper));
                }
                else
                {
                    if (m_newObject != null)
                    {
                        eDrawObjectMouseDown result = m_newObject.OnMouseDown(m_canvaswrapper, mouseunitpoint, snappoint);
                        switch (result)
                        {
                            case eDrawObjectMouseDown.Done:
                                m_model.AddObject(m_model.ActiveLayer, m_newObject);
                                m_newObject = null;
                                DoInvalidate(true);
                                break;
                            case eDrawObjectMouseDown.DoneRepeat:
                                m_model.AddObject(m_model.ActiveLayer, m_newObject);
                                m_newObject = m_model.CreateObject(m_newObject.Id, m_newObject.RepeatStartingPoint, null);
                                DoInvalidate(true);
                                break;
                            case eDrawObjectMouseDown.Continue:
                                break;
                        }
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_mousedownPoint = new PointF(e.X, e.Y); // used when panning
            m_dragOffset = new PointF(0, 0);

           

            UnitPoint mousepoint = ToUnit(m_mousedownPoint);
            if (m_snappoint != null)
                mousepoint = m_snappoint.SnapPoint;
/*
            if (m_commandType == eCommandType.editNode)
            {
                bool handled = false;
                if (m_nodeMoveHelper.HandleMouseDown(mousepoint, ref handled))
                {
                    FinishNodeEdit();
                    base.OnMouseDown(e);
                    return;
                }
            }
 * 
 * */
            if (m_commandType == eCommandType.select)
            {
                bool handled = false;
                if (m_nodeMoveHelper.HandleMouseDown(mousepoint, ref handled))
                {
                    m_commandType = eCommandType.editNode;
                    m_snappoint = null;
                    base.OnMouseDown(e);
                    return;
                }
                m_selection = new SelectionRectangle(m_mousedownPoint);
            }
            /*
            if (m_commandType == eCommandType.move)
            {
                m_moveHelper.HandleMouseDownForMove(mousepoint, m_snappoint);
            }
             * */
            if (m_commandType == eCommandType.draw)
            {
                HandleMouseDownWhenDrawing(mousepoint, null);
                DoInvalidate(true);
            }

            else if (m_commandType == eCommandType.symbol) {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestForm));
                CanvasWrapper canvastest = new CanvasWrapper(this, Graphics.FromImage(m_staticImage), ClientRectangle);
                canvastest.Graphics.SmoothingMode = m_smoothingMode;
                if (m_drawObjectId == "clef1" || m_drawObjectId == "clef2" || m_drawObjectId == "clef3" || m_drawObjectId == "clef4" || m_drawObjectId == "clef6" || m_drawObjectId == "N2"
                    || m_drawObjectId == "N3" || m_drawObjectId == "N4" || m_drawObjectId == "N5" || m_drawObjectId == "N6" || m_drawObjectId == "N7" || m_drawObjectId == "N8" || m_drawObjectId == "N9"
                    || m_drawObjectId == "N10" || m_drawObjectId == "N11" || m_drawObjectId == "N12" || m_drawObjectId == "s10" || m_drawObjectId == "s11" || m_drawObjectId == "s12" ||
                    m_drawObjectId == "N12" || m_drawObjectId == "N13" )
                {
                  //  MessageBox.Show("test  test " + m_drawObjectId);

                    DrawSymbol(canvastest, e.X, e.Y, ((System.Drawing.Image)(resources.GetObject("ribbonButton_" + m_drawObjectId + ".Image"))), 1);
                }
                else if (m_drawObjectId == "bm1" || m_drawObjectId == "bm2" || m_drawObjectId == "bm3" || m_drawObjectId == "bm4"
                   || m_drawObjectId == "bm5" || m_drawObjectId == "bm6" || m_drawObjectId == "bm7" || m_drawObjectId == "bm8" ||
                    m_drawObjectId == "bm18" || m_drawObjectId == "o11")
                {
                    DrawSymbol(canvastest, e.X, e.Y, ((System.Drawing.Image)(resources.GetObject("ribbonButton_" + m_drawObjectId + ".Image"))), 0.7);

                }
                else
                {
                    //  MessageBox.Show("test  test " + m_drawObjectId);
                    DrawSymbol(canvastest, e.X, e.Y, ((System.Drawing.Image)(resources.GetObject("ribbonButton_" + m_drawObjectId + ".Image"))), 0.4);
                }
                canvastest.CanvasCtrl.Refresh();
                
            }
            /*
            if (m_commandType == eCommandType.edit)
            {
                if (m_editTool == null)
                    m_editTool = m_model.GetEditTool(m_editToolId);
                if (m_editTool != null)
                {
                    if (m_editTool.SupportSelection)
                        m_selection = new SelectionRectangle(m_mousedownPoint);

                    eDrawObjectMouseDown mouseresult = m_editTool.OnMouseDown(m_canvaswrapper, mousepoint, m_snappoint);
//                    /*
//                    if (mouseresult == eDrawObjectMouseDown.Continue)
//                    {
//                        if (m_editTool.SupportSelection)
//                            m_selection = new SelectionRectangle(m_mousedownPoint);
//                    }
//                     * * /
//
                    if (mouseresult == eDrawObjectMouseDown.Done)
                    {
                        m_editTool.Finished();
                        m_editTool = m_model.GetEditTool(m_editToolId); // continue with new tool
                        //m_editTool = null;

                        if (m_editTool.SupportSelection)
                            m_selection = new SelectionRectangle(m_mousedownPoint);
                    }
                }
                DoInvalidate(true);
                UpdateCursor();
            }
           */
            base.OnMouseDown(e);
        }

        
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            /*
            if (m_commandType == eCommandType.pan)
            {
                m_panOffset.X += m_dragOffset.X;
                m_panOffset.Y += m_dragOffset.Y;
                m_dragOffset = new PointF(0, 0);
            }
            */
            List<IDrawObject> hitlist = null;
            Rectangle screenSelRect = Rectangle.Empty;
            if (m_selection != null)
            {
                screenSelRect = m_selection.ScreenRect();
                RectangleF selectionRect = m_selection.Selection(m_canvaswrapper);
                if (selectionRect != RectangleF.Empty)
                {
                    // is any selection rectangle. use it for selection
                    hitlist = m_model.GetHitObjects(m_canvaswrapper, selectionRect, m_selection.AnyPoint());
                    DoInvalidate(true);
                }
                else
                {
                    // else use mouse point
                    UnitPoint mousepoint = ToUnit(new PointF(e.X, e.Y));
                    hitlist = m_model.GetHitObjects(m_canvaswrapper, mousepoint);
                }
                m_selection = null;
            }
            /*
            if (m_commandType == eCommandType.select)
            {
                if (hitlist != null)
                    HandleSelection(hitlist);
            }
            if (m_commandType == eCommandType.edit && m_editTool != null)
            {
                UnitPoint mousepoint = ToUnit(m_mousedownPoint);
                if (m_snappoint != null)
                    mousepoint = m_snappoint.SnapPoint;
                if (screenSelRect != Rectangle.Empty)
                    m_editTool.SetHitObjects(mousepoint, hitlist);
                m_editTool.OnMouseUp(m_canvaswrapper, mousepoint, m_snappoint);
            }
             * */
            if (m_commandType == eCommandType.draw && m_newObject != null)
            {
                UnitPoint mousepoint = ToUnit(m_mousedownPoint);
                if (m_snappoint != null)
                    mousepoint = m_snappoint.SnapPoint;
                m_newObject.OnMouseUp(m_canvaswrapper, mousepoint, m_snappoint);
            }
            
        }
        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (m_selection != null)
            {
                Graphics dc = Graphics.FromHwnd(Handle);
                m_selection.SetMousePoint(dc, new PointF(e.X, e.Y));
                dc.Dispose();
                return;
            }
            UnitPoint mousepoint;
            if (m_commandType == eCommandType.draw || m_commandType == eCommandType.move || m_nodeMoveHelper.IsEmpty == false)
            {
                Rectangle invalidaterect = Rectangle.Empty;
                ISnapPoint newsnap = null;
                mousepoint = GetMousePoint();
                if (RunningSnapsEnabled)
                    newsnap = m_model.SnapPoint(m_canvaswrapper, mousepoint, m_runningSnapTypes, null);
                if (newsnap == null)
                    newsnap = m_model.GridLayer.SnapPoint(m_canvaswrapper, mousepoint, null);
                if ((m_snappoint != null) && ((newsnap == null) || (newsnap.SnapPoint != m_snappoint.SnapPoint) || m_snappoint.GetType() != newsnap.GetType()))
                {
                    invalidaterect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(m_canvaswrapper, m_snappoint.BoundingRect));
                    invalidaterect.Inflate(2, 2);
                    RepaintStatic(invalidaterect); // remove old snappoint
                    m_snappoint = newsnap;
                }
                //if (m_commandType == eCommandType.move)
                //    Invalidate(invalidaterect);

                if (m_snappoint == null)
                    m_snappoint = newsnap;
            }
            //////////////////////////////////////////////////////////////////////////

            //UnitPoint mousepoint;
            if (m_snappoint != null)
                mousepoint = m_snappoint.SnapPoint;
            else
                mousepoint = GetMousePoint();
            
            if (m_newObject != null)
            {
                Rectangle invalidaterect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(m_canvaswrapper, m_newObject.GetBoundingRect(m_canvaswrapper)));
                invalidaterect.Inflate(2, 2);
                RepaintStatic(invalidaterect);

                m_newObject.OnMouseMove(m_canvaswrapper, mousepoint);
                RepaintObject(m_newObject);
            }
            if (m_snappoint != null)
                RepaintSnappoint(m_snappoint);
            /*
            if (m_moveHelper.HandleMouseMoveForMove(mousepoint))
                Refresh(); //Invalidate();

            RectangleF rNoderect = m_nodeMoveHelper.HandleMouseMoveForNode(mousepoint);

            if (rNoderect != RectangleF.Empty)
            {

                Rectangle invalidaterect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(m_canvaswrapper, rNoderect));
                RepaintStatic(invalidaterect);

                CanvasWrapper dc = new CanvasWrapper(this, Graphics.FromHwnd(Handle), ClientRectangle);
                dc.Graphics.Clip = new Region(ClientRectangle);
                //m_nodeMoveHelper.DrawOriginalObjects(dc, rNoderect);
                m_nodeMoveHelper.DrawObjects(dc, rNoderect);
                if (m_snappoint != null)
                    RepaintSnappoint(m_snappoint);

                dc.Graphics.Dispose();
                dc.Dispose();
            }
            */
        }


        //wyy.  make the page stable. 
    /*    protected override void OnMouseWheel(MouseEventArgs e)
        {
            UnitPoint p = GetMousePoint();
            float wheeldeltatick = 120;
            float zoomdelta = (1.25f * (Math.Abs(e.Delta) / wheeldeltatick));
            if (e.Delta < 0)
                m_model.Zoom = m_model.Zoom / zoomdelta;
            else
                m_model.Zoom = m_model.Zoom * zoomdelta;
            SetCenterScreen(ToScreen(p), true);
            DoInvalidate(true);
            base.OnMouseWheel(e);
        }
        */
        public void CommandSelectDrawTool(string drawobjectid)
        {
            CommandEscape();
            m_model.ClearSelectedObjects();
            m_commandType = eCommandType.draw;
            m_drawObjectId = drawobjectid;
            //UpdateCursor();
        }

        public void CommandSelectSymbolTool(string drawobjectid)
        {
            CommandEscape();
            m_model.ClearSelectedObjects();
            m_commandType = eCommandType.symbol;
            m_drawObjectId = drawobjectid;
        }

        public void CommandEscape()
        {
            bool dirty = (m_newObject != null) || (m_snappoint != null);
            m_newObject = null;
            m_snappoint = null;
            //if (m_editTool != null)
            //    m_editTool.Finished();
            //m_editTool = null;
            m_commandType = eCommandType.select;
            //m_moveHelper.HandleCancelMove();
            m_nodeMoveHelper.HandleCancelMove();
            DoInvalidate(dirty);
            //UpdateCursor();
        }
    }
}
