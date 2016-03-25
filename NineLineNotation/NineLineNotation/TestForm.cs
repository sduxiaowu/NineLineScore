using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;



        
namespace NineLineNotation
{
    
    public partial class TestForm : RibbonForm
    {
        public static int page_now = 1;//wyy
        public static int page_sum = 1;//wyy
        NotationPanel m_panel;
        static TestForm tf;

        public MyDllCall call = FunA;
        //djl 委托
        public delegate void MyDllCall(int a, int b);

        [DllImport("basicdll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Winapi)]
        public static extern int test(MyDllCall fa);
        
        
        public TestForm()
        {
            tf = this;
            //m_panel是乐谱页面
            m_panel = new NotationPanel();
            //m_canvas.Dock = DockStyle.Fill;
            m_panel.Location = new System.Drawing.Point(188, 8);//设置乐谱在整个页面中的位置
            m_panel.Anchor = AnchorStyles.Top;


            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            //以上为InitializeComponent();
            this.ribbon1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            DoRecover();

            //panel1为界面底层灰色部分
            //以上为DoRecover();
            panel1.Controls.Add(m_panel);
            panel1.BackColor = Color.Gray;
            //statusStrip1.BackColor = Color.Red;

            //界面底部页码栏
            toolStripStatusLabel1.Text = "  页码：" + page_now + " 页 " + page_now + "/" + page_sum + " ";//wyy
            toolStripStatusLabel1.BorderSides = ToolStripStatusLabelBorderSides.Right;

            InitLists();
            Initsymbol_list();
            //StartPosition = FormStartPosition.WindowsDefaultBounds;
            WindowState = FormWindowState.Maximized;
            

        }
        //画谱by djl 

        public void Paintt(int start,int end,int line,int strong){

            CanvasCtrl.M_canvas.m_model.ActiveLayer.Draw(CanvasCtrl.M_canvas.dcanvaswrapper, CanvasCtrl.M_canvas.drf,start,end,line,strong);
        }

        private Color GetRandomColor(Random r)
        {
            if (r == null)
            {
                r = new Random(DateTime.Now.Millisecond);
            }

            return Color.FromKnownColor((KnownColor)r.Next(1, 150));
        }
        private void Initsymbol_list()
        {

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestForm));
            Image[] images = new Image[13];
            //   RibbonProfessionalRenderer rend = new RibbonProfessionalRenderer();
            //    BackColor = rend.ColorTable.RibbonBackground;

            Random r = new Random();
            for (int i = 0; i < images.Length; i++)
            {
                Bitmap b = new Bitmap((System.Drawing.Image)(resources.GetObject("ribbonButton_s" + (i + 1) + ".SmallImage")));
                Bitmap c = new Bitmap((System.Drawing.Image)(resources.GetObject("ribbonButton_s" + (i + 1) + ".Image")));
                images[i] = c;

                RibbonButton btn = new RibbonButton();
               // btn.SmallImage = b;
                btn.Image = b;
                btn.Name = "symbols" + (i + 1);
                symbol_lst.Buttons.Add(btn);
            }

            RibbonButtonList lst2 = new RibbonButtonList();

            for (int i = 0; i < images.Length; i++)
            {
                RibbonButton btn = new RibbonButton();
                btn.Image = images[i];
                btn.Name = "symbols" + (i + 1);
                btn.Click += new System.EventHandler(OnToolSelect);
                lst2.Buttons.Add(btn);
            }
            symbol_lst.DropDownItems.Add(lst2);
            /*
                       RibbonButton[] buttons = new RibbonButton[30];
                       int square = 16;
                       int squares = 4;
                       int sqspace = 2;

                    for (int i = 0; i < buttons.Length; i++)
                       {
                           #region Create color squares
                           Bitmap colors = new Bitmap((System.Drawing.Image)(resources.GetObject("ribbonButton_s"+(i+1)+".Image")));
               
                           buttons[i] = new RibbonButton(colors);;
                           buttons[i].MaxSizeMode = RibbonElementSizeMode.Medium;
                           buttons[i].MinSizeMode = RibbonElementSizeMode.Medium;
                       }
                       RibbonButtonList blst = new RibbonButtonList(buttons);
                       blst.FlowToBottom = false;
                       blst.ItemsSizeInDropwDownMode = new Size(1, 10);
                       itemColors.DropDownItems.Insert(0, blst);
                       itemColors.DropDownResizable = true;

                       #endregion*/
        }
        private void InitLists()
        {
            Image[] images = new Image[255];
            RibbonProfessionalRenderer rend = new RibbonProfessionalRenderer();
            BackColor = rend.ColorTable.RibbonBackground;

            Random r = new Random();



            #region Color Squares
            using (GraphicsPath path = RibbonProfessionalRenderer.RoundRectangle(new Rectangle(3, 3, 26, 26), 4))
            {
                using (GraphicsPath outer = RibbonProfessionalRenderer.RoundRectangle(new Rectangle(0, 0, 32, 32), 4))
                {
                    for (int i = 0; i < images.Length; i++)
                    {

                        Bitmap b = new Bitmap(32, 32);


                        using (Graphics g = Graphics.FromImage(b))
                        {
                            g.SmoothingMode = SmoothingMode.AntiAlias;

                            using (SolidBrush br = new SolidBrush(Color.FromArgb(255, i * (255 / images.Length), 0)))
                            {
                                g.FillPath(br, path);
                            }

                            using (Pen p = new Pen(Color.White, 3))
                            {
                                g.DrawPath(p, path);
                            }

                            g.DrawPath(Pens.Wheat, path);

                            g.DrawString(Convert.ToString(i + 1), Font, Brushes.White, new Point(10, 10));
                        }

                        images[i] = b;

                        RibbonButton btn = new RibbonButton();
                        btn.Image = b;
                        lst.Buttons.Add(btn);
                    }
                }
            }

            //lst.DropDownItems.Add(new RibbonSeparator("Available styles"));
            RibbonButtonList lst2 = new RibbonButtonList();

            for (int i = 0; i < images.Length; i++)
            {
                RibbonButton btn = new RibbonButton();
                btn.Image = images[i];
                lst2.Buttons.Add(btn);
            }
            lst.DropDownItems.Add(lst2);
            lst.DropDownItems.Add(new RibbonButton("Save selection as a new quick style..."));
            lst.DropDownItems.Add(new RibbonButton("Erase Format"));
            lst.DropDownItems.Add(new RibbonButton("Apply style..."));
            #endregion

            #region Theme Colors

            RibbonButton[] buttons = new RibbonButton[30];
            int square = 16;
            int squares = 4;
            int sqspace = 2;

            for (int i = 0; i < buttons.Length; i++)
            {
                #region Create color squares
                Bitmap colors = new Bitmap((square + sqspace) * squares, square + 1);
                string[] colorss = new string[squares];
                using (Graphics g = Graphics.FromImage(colors))
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Color sqcolor = GetRandomColor(r);
                        colorss[j] = sqcolor.Name;
                        using (SolidBrush b = new SolidBrush(sqcolor))
                        {
                            g.FillRectangle(b, new Rectangle(j * (square + sqspace), 0, square, square));
                        }
                        g.DrawRectangle(Pens.Gray, new Rectangle(j * (square + sqspace), 0, square, square));
                    }
                }
                #endregion

                buttons[i] = new RibbonButton(colors);
                buttons[i].Text = string.Join(", ", colorss); ;
                buttons[i].MaxSizeMode = RibbonElementSizeMode.Medium;
                buttons[i].MinSizeMode = RibbonElementSizeMode.Medium;
            }
            RibbonButtonList blst = new RibbonButtonList(buttons);
            blst.FlowToBottom = false;
            blst.ItemsSizeInDropwDownMode = new Size(1, 10);
            itemColors.DropDownItems.Insert(0, blst);
            itemColors.DropDownResizable = true;

            #endregion

        }

        private void TestForm_Load(object sender, EventArgs e)
        {

        }

        private void ribbonOrbOptionButton1_Click(object sender, EventArgs e)
        {
            Close();
        }


        public void OnToolSelect(object sender, System.EventArgs e)
        {
            if (sender is RibbonButton) // from menu or toolbar
            {
               // Bitmap bmp = new Bitmap();
                Bitmap bmp = (Bitmap)((RibbonButton)sender).Image;
                Cursor cursor = new Cursor(bmp.GetHicon());
                this.Cursor = cursor; 

                String SenderName = ((RibbonButton)sender).Name;
                if (SenderName.Equals("Eraser"))
                {
                    if (CanvasCtrl.M_canvas != null)
                    {
                        CanvasCtrl.M_canvas.CommandEscape();
                        //this.toolStripStatusLabel1.Text = SenderText;
                        return;
                    }
                }
                if (SenderName.Equals("Pen"))
                {
                    if (CanvasCtrl.M_canvas != null)
                    {
                        CanvasCtrl.M_canvas.CommandSelectDrawTool(SenderName);
                    }
                }

                else if (SenderName.Substring(0, 6).Equals("symbol"))
                {
                    if (CanvasCtrl.M_canvas != null)
                    {
                        CanvasCtrl.M_canvas.CommandSelectSymbolTool(SenderName.Substring(6));
                    }
                }
            }
            if (sender is RibbonPanel)
            {
                this.toolStripStatusLabel1.Text = "haha";
            }
        }

        //djl 调用底层dll代码
        private void init_Click(object sender, EventArgs e)
        {
            this.label1.Text = "初始化成功";
            IntPtr i = init();
            string str = Marshal.PtrToStringAnsi(i);
            this.label1.Text = str;
        }
        [DllImport("basicdll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Winapi)]
        static extern IntPtr init();
        private void startrecord(object sender,EventArgs e) {

            IntPtr i = start();
            string str = Marshal.PtrToStringAnsi(i);
            this.label1.Text = str;
            test(call);
           
           
            
        }
        [DllImport("basicdll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Winapi)]
        static extern IntPtr start();


        static double start_point = 0;
        static double start_time=0;
        static double end_time=0;
        static int line_length=174;
        static int draw_end = 10;
        static int line_end = 0;
        static Dictionary<int, Thread> threadpool=new Dictionary<int,Thread>();
         
        //声明回调的函数
        public static void FunA(int a, int time)
        {
            tf.label1.Text = a + " " + time;
            int score = (a >> 8) % 256;
            int strong = a >> 16;
            Thread t;
            double little_time = (double)time / 40.0;

            if (start_point == 0) {

                start_point = little_time;
            }

            if (strong != 0)
            {
                start_time = little_time;
               int draw_start = draw_end;
                int big_line = line_end;
                int small_line = 0;
                small_line = 87 - score;
                t = new Thread(delegate() { threadpaint(big_line, small_line, draw_start,strong); });
                threadpool.Add(score,t);
                t.Start();
            }
            else
            {
                end_time = little_time;           
            //    int draw_end = (int) ( (end_time-start_point) % line_length);
                try
                {
                    t = threadpool[score];
                    threadpool.Remove(score);
                    t.Abort();
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message); 
                }
               
            }        
        }
        static void threadpaint(int big_line,int small_line,int draw_start,int strong) {

            while (true)
            {
                if (draw_start > line_length) {
                    big_line++;
                    draw_start = 10;
                    draw_end = 10;
                    line_end = big_line;
                }
                tf.Paintt(draw_start, draw_start + 2, big_line * 54 + small_line,strong);
                CanvasCtrl.M_canvas.Invalidate();
                draw_start += 2;
                draw_end = draw_start;
                Thread.Sleep(100);
            }
        }
        private void cancelrecord(object sender, EventArgs e)
        {
           
            this.label1.Text = "完成";

        }
    }
}